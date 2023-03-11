using PPPredictor.Data;
using PPPredictor.Data.Curve;
using PPPredictor.OpenAPIs;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static PPPredictor.OpenAPIs.beatleaderapi;

namespace PPPredictor.Utilities
{
    public class PPCalculatorBeatLeader : PPCalculator
    {
        private readonly OpenAPIs.beatleaderapi beatleaderapi;
        internal static float accumulationConstant = 0.965f;

        public PPCalculatorBeatLeader() : base() 
        {
            playerPerPages = 50;
            beatleaderapi = new OpenAPIs.beatleaderapi();
            UpdateAvailableMapPools();
        }

        protected override async Task<PPPPlayer> GetPlayerInfo(long userId)
        {
            try
            {
                BeatLeaderPlayer player = await beatleaderapi.GetPlayer(userId);
                if(_leaderboardInfo.CurrentMapPool != null && _leaderboardInfo.CurrentMapPool.MapPoolType == MapPoolType.Default)
                {
                    return new PPPPlayer(player);
                }
                else if (_leaderboardInfo.CurrentMapPool != null && _leaderboardInfo.CurrentMapPool.MapPoolType == MapPoolType.Custom)
                {
                    BeatLeaderPlayerEvents eventPlayer = player.eventsParticipating.Where(x => x.eventId == long.Parse(_leaderboardInfo.CurrentMapPool.Id)).FirstOrDefault();
                    if(eventPlayer != null)
                    {
                        return new PPPPlayer(eventPlayer);
                    }
                }
                return new PPPPlayer(true);
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
                BeatLeaderPlayerList scoreSaberPlayerCollection = null;
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                if(_leaderboardInfo.CurrentMapPool != null && _leaderboardInfo.CurrentMapPool.MapPoolType == MapPoolType.Default)
                {
                    scoreSaberPlayerCollection = await beatleaderapi.GetPlayersInLeaderboard("pp", (int)fetchIndexPage, playerPerPages, "desc");
                }
                else if(_leaderboardInfo.CurrentMapPool != null)
                {
                    scoreSaberPlayerCollection = await beatleaderapi.GetPlayersInEventLeaderboard(long.Parse(_leaderboardInfo.CurrentMapPool.Id), "pp", (int)fetchIndexPage, playerPerPages, "desc");
                }
                Plugin.Log.Error($"GetPlayers index: {fetchIndexPage}");
                foreach (var scoreSaberPlayer in scoreSaberPlayerCollection.data)
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
                long? id = null;
                if (!string.IsNullOrEmpty(_leaderboardInfo.CurrentMapPool.Id))
                {
                    id = long.Parse(_leaderboardInfo.CurrentMapPool.Id);
                }
                BeatLeaderPlayerScoreList beatLeaderPlayerScoreList = await beatleaderapi.GetPlayerScores(userId, "date", "desc", page, pageSize, id);
                return new PPPScoreCollection(beatLeaderPlayerScoreList);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader GetRecentScores Error: {ex.Message}");
                return new PPPScoreCollection();
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
                    if(_leaderboardInfo.CurrentMapPool.MapPoolType == MapPoolType.Custom && !_leaderboardInfo.CurrentMapPool.LsMapPoolEntries.Where(x => x.Searchstring == searchString).Any())
                    {
                        return new PPPBeatMapInfo(0); //Currently selected map is not contained in selected MapPool
                    }
                    ShortScore cachedInfo = _leaderboardInfo.DefaultMapPool.LsLeaderboardScores?.FirstOrDefault(x => x.Searchstring == searchString);
                    bool refetchInfo = cachedInfo != null && cachedInfo.FetchTime < DateTime.Now.AddDays(-7);
                    if (cachedInfo == null || refetchInfo)
                    {
                        if (refetchInfo) _leaderboardInfo.DefaultMapPool.LsLeaderboardScores?.Remove(cachedInfo);
                        BeatLeaderSong song = await beatleaderapi.GetSongByHash(songHash);
                        if (song != null)
                        {
                            BeatLeaderDifficulty diff = song.difficulties.FirstOrDefault(x => x.value == beatmap.difficultyRank);
                            if (diff != null)
                            {
                                //Find or insert ModifierValueId
                                PPPModifierValues newModifierValues = new PPPModifierValues(diff.modifierValues);
                                int modifierValueId = _leaderboardInfo.LsModifierValues.FindIndex(x => x.Equals(newModifierValues));
                                if(modifierValueId == -1)
                                {
                                    modifierValueId = _leaderboardInfo.LsModifierValues.Select(x => x.Id).DefaultIfEmpty(-1).Max() + 1;
                                    _leaderboardInfo.LsModifierValues.Add(new PPPModifierValues(modifierValueId, diff.modifierValues));
                                }
                                _leaderboardInfo.CurrentMapPool.LsLeaderboardScores.Add(new ShortScore(searchString, diff.stars.GetValueOrDefault(), DateTime.Now, modifierValueId));
                                if (diff.stars.HasValue && diff.status == (int)BeatLeaderDifficultyStatus.ranked)
                                {
                                    return new PPPBeatMapInfo(diff.stars.Value, modifierValueId);
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

        public async void UpdateAvailableMapPools()
        {
            try
            {
                var events = await beatleaderapi.GetEvents();

                int sortIndex = 0;
                foreach (BeatLeaderEvent eventItem in events.data)
                {
                    PPPMapPool oldPool = _leaderboardInfo.LsMapPools.Find(x => x.Id == eventItem.id.ToString());
                    if (oldPool == null && eventItem.dtEndDate < DateTime.UtcNow)
                    {
                        continue; //Do not add expired events
                    }
                    else if (oldPool != null && eventItem.dtEndDate < DateTime.UtcNow)
                    {
                        //Remove expired events
                        _leaderboardInfo.LsMapPools.Remove(oldPool);
                        continue;
                    }
                    if (oldPool == null)
                    {
                        var mapPool = new PPPMapPool(eventItem.id.ToString(), eventItem.playListId.ToString(), MapPoolType.Custom, eventItem.name, accumulationConstant, sortIndex, new BeatLeaderPPPCurve(), string.Empty);
                        sortIndex++;
                        this._leaderboardInfo.LsMapPools.Add(mapPool);
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader UpdateAvailableMapPools Error: {ex.Message}");
                throw;
            }
        }

        override public async Task UpdateMapPoolDetails(PPPMapPool mapPool)
        {
            if (mapPool.MapPoolType != MapPoolType.Default)
            {
                mapPool.LsMapPoolEntries.Clear();
                BeatLeaderPlayList lsPlayList = await this.beatleaderapi.GetPlayList(long.Parse(mapPool.PlayListId));
                foreach (BeatLeaderPlayListSong song in lsPlayList.songs)
                {
                    foreach (BeatLeaderPlayListDifficulties diff in song.difficulties)
                    {
                        mapPool.LsMapPoolEntries.Add(new PPPMapPoolEntry(song, diff));
                    }
                }
            }
        }

        public override string CreateSeachString(string hash, IDifficultyBeatmap beatmap)
        {
            return $"{hash}_{beatmap.difficultyRank}";
        }
    }
}
