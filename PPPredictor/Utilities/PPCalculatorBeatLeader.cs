using beatleaderapi;
using PPPredictor.Data;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    public class PPCalculatorBeatLeader : PPCalculator
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly beatleaderapi.beatleaderapi beatLeaderClient;
        private readonly double ppCalcWeight = 42;

        public PPCalculatorBeatLeader() : base() 
        {
            beatLeaderClient = new beatleaderapi.beatleaderapi("https://api.beatleader.xyz/", httpClient);
        }

        protected override async Task<PPPPlayer> GetPlayerInfo(long userId)
        {
            try
            {
                var playerInfo = beatLeaderClient.PlayerAsync(userId.ToString(), false);
                var beatLeaderPlayer = await playerInfo;
                return new PPPPlayer(beatLeaderPlayer);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        protected override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage)
        {
            try
            {
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                PlayerResponseWithStatsResponseWithMetadata scoreSaberPlayerCollection = await beatLeaderClient.PlayersAsync("pp", (int)fetchIndexPage, 50, null, "desc", null, null, null, null, null, null, null, null, null, null);
                foreach (var scoreSaberPlayer in scoreSaberPlayerCollection.Data)
                {
                    lsPlayer.Add(new PPPPlayer(scoreSaberPlayer));
                }
                return lsPlayer;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader GetPlayers Error: {ex.Message}");
                return new List<PPPPlayer>();
            }
        }

        protected override async Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page)
        {
            try
            {
                ScoreResponseWithMyScoreResponseWithMetadata scoreSaberCollection = await beatLeaderClient.ScoresAsync(userId, "date", "desc", page, pageSize, null, null, null, null, null);
                return new PPPScoreCollection(scoreSaberCollection);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader GetRecentScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        public override double CalculatePPatPercentage(double star, double percentage, bool levelFailed)
        {
            try
            {
                if (star <= 0) return 0;
                double l = 1.0 - (0.03 * ((star - 0.5) - 3) / 11);
                double n = percentage / 100.0;
                n = Math.Min(n, l - 0.0000000000000001); //Stop just before the calculation crashes, lets hope this works
                double a = 0.96 * l;
                double f = 1.2 - (0.6 * ((star - 0.5) / 14));
                return (star + 0.5) * ppCalcWeight * Math.Pow((Math.Log(l / (l - n)) / (Math.Log(l / (l - a)))), f);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader CalculatePPatPercentage Error: {ex.Message}");
                return -1;
            }
        }

        public override async Task<PPPBeatMapInfo> GetBeatMapInfoAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            try
            {
                if (lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel)
                {
                    string songHash = Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel);
                    string searchString = CreateSeachString(songHash, "SOLO" + beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName, beatmap.difficultyRank);
                    ShortScore cachedInfo = _leaderboardInfo.LsLeaderboardScores?.FirstOrDefault(x => x.Searchstring == searchString);
                    bool refetchInfo = cachedInfo != null && cachedInfo.FetchTime < DateTime.Now.AddDays(-7);
                    if (cachedInfo == null || refetchInfo)
                    {
                        if (refetchInfo) _leaderboardInfo.LsLeaderboardScores?.Remove(cachedInfo);
                        Song song = await beatLeaderClient.Hash2Async(songHash);
                        if (song != null)
                        {
                            DifficultyDescription diff = song.Difficulties.FirstOrDefault(x => x.Value == beatmap.difficultyRank);
                            if (diff != null)
                            {
                                //Find or insert ModifierValueId
                                PPPModifierValues newModifierValues = new PPPModifierValues(diff.ModifierValues);
                                int modifierValueId = _leaderboardInfo.LsModifierValues.FindIndex(x => x.Equals(newModifierValues));
                                if(modifierValueId == -1)
                                {
                                    modifierValueId = _leaderboardInfo.LsModifierValues.Select(x => x.Id).DefaultIfEmpty(-1).Max() + 1;
                                    _leaderboardInfo.LsModifierValues.Add(new PPPModifierValues(modifierValueId, diff.ModifierValues));
                                }
                                _leaderboardInfo.LsLeaderboardScores.Add(new ShortScore(searchString, diff.Stars.GetValueOrDefault(), DateTime.Now, modifierValueId));
                                if (diff.Stars.HasValue && (int)diff.Status == (int)BeatLeaderDifficultyStatus.ranked)
                                {
                                    return new PPPBeatMapInfo(diff.Stars.Value, modifierValueId);
                                }
                            }
                        }
                    }
                    else
                    {
                        return new PPPBeatMapInfo(cachedInfo.Stars, cachedInfo.ModifierValuesId);
                    }
                }
                return new PPPBeatMapInfo();
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader GetStarsForBeatmapAsync Error: {ex.Message}");
                return new PPPBeatMapInfo(-1, -1);
            }
        }

        public override double ApplyModifierMultiplierToStars(PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed)
        {
            List<string> lsModifiers = ParseModifiers(gameplayModifiers);
            return beatMapInfo.BaseStars * GenerateModifierMultiplier(lsModifiers, beatMapInfo.ModifierValueId, levelFailed);
        }

        private List<string> ParseModifiers(GameplayModifiers gameplayModifiers)
        {
            try
            {
                List<string> lsModifiers = new List<string>();
                if (gameplayModifiers.disappearingArrows) lsModifiers.Add("DA");
                if (gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Faster) lsModifiers.Add("FS");
                if (gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Slower) lsModifiers.Add("SS");
                if (gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.SuperFast) lsModifiers.Add("SF");
                if (gameplayModifiers.ghostNotes) lsModifiers.Add("GN");
                if (gameplayModifiers.noArrows) lsModifiers.Add("NA");
                if (gameplayModifiers.noBombs) lsModifiers.Add("NB");
                if (gameplayModifiers.noFailOn0Energy) lsModifiers.Add("NF");
                if (gameplayModifiers.enabledObstacleType == GameplayModifiers.EnabledObstacleType.NoObstacles) lsModifiers.Add("NO");
                if (gameplayModifiers.proMode) lsModifiers.Add("PM");
                if (gameplayModifiers.smallCubes) lsModifiers.Add("SC");
                if (gameplayModifiers.instaFail) lsModifiers.Add("IF");
                //if (gameplayModifiers.FOURLIFES??) lsModifiers.Add("BE");
                if (gameplayModifiers.strictAngles) lsModifiers.Add("SA");
                if (gameplayModifiers.zenMode) lsModifiers.Add("ZM");
                return lsModifiers;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader ParseModifiers Error: {ex.Message}");
                return new List<string>();
            }
        }

        private double GenerateModifierMultiplier(List<string> lsModifier, int modifierValueId, bool levelFailed)
        {
            try
            {
                double multiplier = 1;
                foreach (string modifier in lsModifier)
                {
                    if (!levelFailed && modifier == "NF") continue; //Ignore nofail until the map is failed in gameplay
                    multiplier += _leaderboardInfo.LsModifierValues[modifierValueId].DctModifierValues[modifier];
                }
                return multiplier;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader GenerateModifierMultiplier Error: {ex.Message}");
                return -1;
            }
        }
    }
}
