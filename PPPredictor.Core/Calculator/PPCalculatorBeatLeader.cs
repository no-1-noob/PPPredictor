using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.Curve;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Core.DataType.Score;
using PPPredictor.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;
using static PPPredictor.Core.DataType.LeaderBoard.BeatLeaderDataTypes;

namespace PPPredictor.Core.Calculator
{
    class PPCalculatorBeatLeader<BLAPI> : PPCalculator where BLAPI : IBeatLeaderAPI, new ()
    {
        private readonly BLAPI beatleaderapi;
        internal static float accumulationConstant = 0.965f;
        private const string AccRating = "AccRating";
        private const string PassRating = "PassRating";
        private const string TechRating = "TechRating";

        public PPCalculatorBeatLeader(Dictionary<string, PPPMapPool> dctMapPool, Settings settings) : base(dctMapPool, settings, Leaderboard.BeatLeader) 
        {
            beatleaderapi = new BLAPI();
        }

        internal override async Task<PPPPlayer> GetPlayerInfo(long userId, PPPMapPool mapPool)
        {
            try
            {
                BeatLeaderPlayer player = await beatleaderapi.GetPlayer(userId, GetLeaderboardContextId(mapPool.LeaderboardContext));
                if(mapPool != null && mapPool.MapPoolType == MapPoolType.Default)
                {
                    return new PPPPlayer(player);
                }
                else if (mapPool != null && mapPool.MapPoolType == MapPoolType.Custom)
                {
                    BeatLeaderPlayerEvents eventPlayer = player.eventsParticipating.Where(x => x.eventId == long.Parse(mapPool.Id)).FirstOrDefault();
                    if(eventPlayer != null)
                    {
                        return new PPPPlayer(eventPlayer);
                    }
                }
                return new PPPPlayer(true);
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorBeatLeader GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        internal override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage, PPPMapPool mapPool)
        {
            try
            {
                BeatLeaderPlayerList scoreSaberPlayerCollection = null;
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                if(mapPool != null && mapPool.MapPoolType == MapPoolType.Default)
                {
                    scoreSaberPlayerCollection = await beatleaderapi.GetPlayersInLeaderboard("pp", (int)fetchIndexPage, _leaderboardInfo.PlayerPerPages, "desc", GetLeaderboardContextId(mapPool.LeaderboardContext));
                }
                else if(mapPool != null)
                {
                    scoreSaberPlayerCollection = await beatleaderapi.GetPlayersInEventLeaderboard(long.Parse(mapPool.Id), "pp", (int)fetchIndexPage, _leaderboardInfo.PlayerPerPages, "desc");
                }
                foreach (var scoreSaberPlayer in scoreSaberPlayerCollection.data)
                {
                    lsPlayer.Add(new PPPPlayer(scoreSaberPlayer));
                }
                return lsPlayer;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorBeatLeader GetPlayers Error: {ex.Message}");
                return new List<PPPPlayer>();
            }
        }

        internal override async Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page, PPPMapPool mapPool)
        {
            try
            {
                long? id = null;
                if (!string.IsNullOrEmpty(mapPool.Id))
                {
                    id = long.Parse(mapPool.Id);
                }
                BeatLeaderPlayerScoreList beatLeaderPlayerScoreList = await beatleaderapi.GetPlayerScores(userId, "date", "desc", page, pageSize, GetLeaderboardContextId(mapPool.LeaderboardContext));
                return new PPPScoreCollection(beatLeaderPlayerScoreList);
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorBeatLeader GetRecentScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        internal override Task<PPPScoreCollection> GetAllScores(string userId, PPPMapPool mapPool)
        {
            return Task.FromResult(new PPPScoreCollection());
        }

        internal override async Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool)
        {
            try
            {
                if (!string.IsNullOrEmpty(beatMapInfo.CustomLevelHash))
                {
                    string searchString = CreateSeachString(beatMapInfo.CustomLevelHash, "SOLO" + beatMapInfo.BeatmapKey.serializedName, ParsingUtil.ParseDifficultyNameToInt(beatMapInfo.BeatmapKey.difficulty.ToString()));
                    if(mapPool.MapPoolType == MapPoolType.Custom && !mapPool.LsMapPoolEntries.Where(x => x.Searchstring == searchString).Any())
                    {
                        return new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(0)); //Currently selected map is not contained in selected MapPool
                    }
                    ShortScore cachedInfo = mapPool.LsLeaderboadInfo?.FirstOrDefault(x => x.Searchstring == searchString);
                    bool refetchInfo = cachedInfo != null && cachedInfo.FetchTime < DateTime.Now.AddDays(_settings.RefetchMapInfoAfterDays);
                    if (cachedInfo == null || refetchInfo)
                    {
                        if (refetchInfo) mapPool.LsLeaderboadInfo?.Remove(cachedInfo);
                        BeatLeaderSong song = await beatleaderapi.GetSongByHash(beatMapInfo.CustomLevelHash);
                        if (song != null)
                        {
                            BeatLeaderDifficulty diff = song.difficulties.FirstOrDefault(x => x.value == ParsingUtil.ParseDifficultyNameToInt(beatMapInfo.BeatmapKey.difficulty.ToString()) && x.modeName.ToUpper() == beatMapInfo.BeatmapKey.serializedName.ToUpper());
                            if (diff != null)
                            {
                                mapPool.LsLeaderboadInfo.Add(new ShortScore(searchString, new PPPStarRating(diff), DateTime.Now));
                                if (diff.stars.HasValue && diff.status == (int)BeatLeaderDifficultyStatus.ranked)
                                {
                                    return new PPPBeatMapInfo(beatMapInfo , new PPPStarRating(diff));
                                }
                            }
                        }
                    }
                    else
                    {
                        return new PPPBeatMapInfo(beatMapInfo, cachedInfo.StarRating);
                    }
                }
                return beatMapInfo;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorBeatLeader GetStarsForBeatmapAsync Error: {ex.Message}");
                return new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(-1));
            }
        }

        internal override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool, GameplayModifiers gameplayModifiers, bool levelFailed, bool levelPaused)
        {
            List<string> lsModifiers = ParseModifiers(gameplayModifiers);
            double accRating = beatMapInfo.BaseStarRating.AccRating;
            double passRating = beatMapInfo.BaseStarRating.PassRating;
            double techRating = beatMapInfo.BaseStarRating.TechRating;
            if (beatMapInfo.BaseStarRating.ModifiersRating != null)
            {
                foreach (string modifier in lsModifiers.Select(x => x.ToLower()))
                {
                    if (beatMapInfo.BaseStarRating.ModifiersRating.ContainsKey(modifier + AccRating))
                    {
                        accRating = beatMapInfo.BaseStarRating.ModifiersRating[modifier + AccRating];
                        passRating = beatMapInfo.BaseStarRating.ModifiersRating[modifier + PassRating];
                        techRating = beatMapInfo.BaseStarRating.ModifiersRating[modifier + TechRating];

                        break;
                    }
                }
            }
            beatMapInfo.ModifiedStarRating = new PPPStarRating(GenerateModifierMultiplier(lsModifiers, beatMapInfo, mapPool, levelFailed, levelPaused, beatMapInfo.BaseStarRating.ModifiersRating != null), accRating, passRating, techRating, beatMapInfo.BaseStarRating.RankedBeatLeader);
            return beatMapInfo;
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
                if (gameplayModifiers.energyType == GameplayModifiers.EnergyType.Battery) lsModifiers.Add("BE");
                if (gameplayModifiers.strictAngles) lsModifiers.Add("SA");
                if (gameplayModifiers.zenMode) lsModifiers.Add("ZM");
                return lsModifiers;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorBeatLeader ParseModifiers Error: {ex.Message}");
                return new List<string>();
            }
        }

        private double GenerateModifierMultiplier(List<string> lsModifier, PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool, bool levelFailed, bool levelPaused, bool ignoreSpeedMultiplier)
        {
            try
            {
                double multiplier = 1;
                if (mapPool.LeaderboardContext == LeaderboardContext.BeatLeaderSCPM && !(lsModifier.Contains("SC") && lsModifier.Contains("PM"))) return 0;
                foreach (string modifier in lsModifier)
                {
                    if (mapPool.LeaderboardContext == LeaderboardContext.BeatLeaderNoModifiers && !(modifier == "BE" || modifier == "IF")) return 0;
                    if (!levelFailed && modifier == "NF") continue; //Ignore nofail until the map is failed in gameplay
                    if (ignoreSpeedMultiplier && (modifier == "SF" || modifier == "SS" || modifier == "FS")) continue; //Ignore speed multies and use the precomputed values from backend
                    if(beatMapInfo.BaseStarRating.ModifierValues.TryGetValue(modifier.ToLower(), out double value))
                    {
                        if (mapPool.LeaderboardContext == LeaderboardContext.BeatLeaderGolf && value < 0) return 0;
                        multiplier += value;
                    }
                }
                return multiplier;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorBeatLeader GenerateModifierMultiplier Error: {ex.Message}");
                return -1;
            }
        }

        override internal async Task InternalUpdateMapPoolDetails(PPPMapPool mapPool)
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

        public override string CreateSeachString(string hash, BeatmapKey beatmapKey)
        {
            return $"{hash}_{ParsingUtil.ParseDifficultyNameToInt(beatmapKey.difficulty.ToString())}";
        }

        private long GetLeaderboardContextId(LeaderboardContext leaderboardContext)
        {
            switch (leaderboardContext)
            {
                case LeaderboardContext.BeatLeaderDefault: return 2;
                case LeaderboardContext.BeatLeaderNoModifiers: return 4;
                case LeaderboardContext.BeatLeaderNoPauses: return 8;
                case LeaderboardContext.BeatLeaderGolf: return 16;
                case LeaderboardContext.BeatLeaderSCPM: return 32;
                default: return 0;
            }
        }

        override public Task UpdateAvailableMapPools()
        {
            if (!_dctMapPool.ContainsKey("-1")) _dctMapPool.Add("-1", new PPPMapPool("-1", MapPoolType.Default, $"General", accumulationConstant, 0, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderDefault));
            if (!_dctMapPool.ContainsKey("-2")) _dctMapPool.Add("-2", new PPPMapPool("-2", MapPoolType.Default, $"No modifiers", accumulationConstant, 1, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderNoModifiers));
            if (!_dctMapPool.ContainsKey("-3")) _dctMapPool.Add("-3", new PPPMapPool("-3", MapPoolType.Default, $"No pauses", accumulationConstant, 2, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderNoPauses));
            if (!_dctMapPool.ContainsKey("-4")) _dctMapPool.Add("-4", new PPPMapPool("-4", MapPoolType.Default, $"Golf", accumulationConstant, 3, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderGolf));
            if (!_dctMapPool.ContainsKey("-5")) _dctMapPool.Add("-5", new PPPMapPool("-5", MapPoolType.Default, $"SCPM", accumulationConstant, 4, new BeatLeaderPPPCurve(), LeaderboardContext.BeatLeaderSCPM));
            return Task.CompletedTask;
        }

        internal override bool IsScoreSetOnCurrentMapPool(PPPMapPool mapPool, PPPScoreSetData score)
        {
            return (GetLeaderboardContextId(mapPool.LeaderboardContext) & score.context) > 0;
        }

        internal override List<PPPMapPoolShort> GetMapPools()
        {
            return _dctMapPool.Values.OrderBy(x => x.SortIndex).Select(x => (PPPMapPoolShort)x).ToList();
        }
    }
}
