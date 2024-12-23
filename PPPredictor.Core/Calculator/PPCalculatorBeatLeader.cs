using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.Curve;
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

        public PPCalculatorBeatLeader() : base() 
        {
            playerPerPages = 50;
            taskDelayValue = 1100;
            beatleaderapi = new BLAPI();
            //UpdateAvailableMapPools();
        }

        protected override async Task<PPPPlayer> GetPlayerInfo(long userId, PPPMapPool mapPool)
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

        protected override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage, PPPMapPool mapPool)
        {
            try
            {
                BeatLeaderPlayerList scoreSaberPlayerCollection = null;
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                if(mapPool != null && mapPool.MapPoolType == MapPoolType.Default)
                {
                    scoreSaberPlayerCollection = await beatleaderapi.GetPlayersInLeaderboard("pp", (int)fetchIndexPage, playerPerPages, "desc", GetLeaderboardContextId(mapPool.LeaderboardContext));
                }
                else if(mapPool != null)
                {
                    scoreSaberPlayerCollection = await beatleaderapi.GetPlayersInEventLeaderboard(long.Parse(mapPool.Id), "pp", (int)fetchIndexPage, playerPerPages, "desc");
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

        protected override async Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page, PPPMapPool mapPool)
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

        protected override Task<PPPScoreCollection> GetAllScores(string userId, string mapPoolId)
        {
            return Task.FromResult(new PPPScoreCollection());
        }

        public override async Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool)
        {
            try
            {
                if (beatMapInfo.SelectedCustomBeatmapLevel != null)
                {
                    string searchString = CreateSeachString(beatMapInfo.CustomLevelHash, "SOLO" + beatMapInfo.BeatmapKey.serializedName, ParsingUtil.ParseDifficultyNameToInt(beatMapInfo.BeatmapKey.difficulty.ToString()));
                    if(mapPool.MapPoolType == MapPoolType.Custom && !mapPool.LsMapPoolEntries.Where(x => x.Searchstring == searchString).Any())
                    {
                        return new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(0)); //Currently selected map is not contained in selected MapPool
                    }
                    ShortScore cachedInfo = _leaderboardInfo.DefaultMapPool.LsLeaderboadInfo?.FirstOrDefault(x => x.Searchstring == searchString);
                    bool refetchInfo = cachedInfo != null && cachedInfo.FetchTime < DateTime.Now.AddDays(Settings.RefetchMapInfoAfterDays);
                    if (cachedInfo == null || refetchInfo)
                    {
                        if (refetchInfo) _leaderboardInfo.DefaultMapPool.LsLeaderboadInfo?.Remove(cachedInfo);
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

        public override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool, GameplayModifiers gameplayModifiers, bool levelFailed, bool levelPaused)
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
                Logging.ErrorPrint($"PPCalculatorBeatLeader UpdateAvailableMapPools Error: {ex.Message}");
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
#warning IsScoreSetOnCurrentMapPool needed?
        //public override bool IsScoreSetOnCurrentMapPool(PPPWebSocketData score)
        //{
        //    return (GetLeaderboardContextId(_leaderboardInfo.CurrentMapPool.LeaderboardContext) & score.context) > 0;
        //}
    }
}
