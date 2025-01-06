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
using static PPPredictor.Core.DataType.LeaderBoard.HitBloqDataTypes;

namespace PPPredictor.Core.Calculator
{
    class PPCalculatorHitBloq<HBAPI> : PPCalculator where HBAPI : IHitBloqAPI, new ()
    {
        private readonly HBAPI hitbloqapi;
        private readonly string unsetPoolId = "-1";
        //Dctionaries defined like here: https://github.com/DaFluffyPotato/hitbloq/blob/1e7bf18f92f1146bf8da2f24769aea072542b6e5/general.py#L24
        private static readonly Dictionary<string, string> dctDiffShorten = new Dictionary<string, string>{
            { "ExpertPlus", "ep" },
            { "Expert", "ex" },
            { "Hard", "h" },
            { "Normal", "n" },
            { "Easy", "e" }
        };
        private static readonly Dictionary<string, string> dctCharShorten = new Dictionary<string, string>
        {
            { "SoloStandard", "s" },
            { "Solo90Degree", "s90" },
            { "Solo360Degree", "s360" },
            { "SoloOneSaber", "s1s" },
            { "SoloNoArrows", "sna" },
            { "SoloStandardHD", "shd" },
            { "SoloLawless", "sll" },
            { "SoloInverseStandard", "sit" },
            { "SoloHorizontalStandard", "shs" },
            { "SoloVerticalStandard", "svs" },
        };
        private static Dictionary<string, string> DctCharLengthen { get => dctCharShorten.ToDictionary(x => x.Value, x => x.Key); }
        private static Dictionary<string, string> DctDiffLengthen { get => dctDiffShorten.ToDictionary(x => x.Value, x => x.Key); }
        public PPCalculatorHitBloq(Dictionary<string, PPPMapPool> dctMapPool, Settings settings) : base(dctMapPool, settings, Leaderboard.HitBloq)
        {
            hitbloqapi = new HBAPI();
        }

        internal override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            beatMapInfo.ModifiedStarRating = new PPPStarRating(levelFailed ? 0 : beatMapInfo.BaseStarRating.Stars);
            return beatMapInfo;
        }

        internal override async Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool)
        {
            try
            {
                if (!string.IsNullOrEmpty(beatMapInfo.CustomLevelHash))
                {
                    string searchString = CreateSeachString(beatMapInfo.CustomLevelHash, beatMapInfo.BeatmapKey);
                    ShortScore cachedInfo = mapPool.LsLeaderboadInfo?.FirstOrDefault(x => x.Searchstring.ToUpper() == searchString.ToUpper());
                    bool refetchInfo = cachedInfo != null && cachedInfo.FetchTime < DateTime.Now.AddDays(_settings.RefetchMapInfoAfterDays);
                    if (cachedInfo == null || refetchInfo)
                    {
                        if (refetchInfo) mapPool.LsLeaderboadInfo?.Remove(cachedInfo);
                        HitBloqLeaderboardInfo leaderboardInfo = await hitbloqapi.GetLeaderBoardInfo(searchString);
                        if (leaderboardInfo != null)
                        {
                            leaderboardInfo.star_rating.TryGetValue(mapPool.Id, out var stars);
                            mapPool.LsLeaderboadInfo.Add(new ShortScore(searchString, new PPPStarRating(stars), DateTime.Now));
                            return new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(stars));
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
                Logging.ErrorPrint($"PPCalculatorHitBloq GetBeatMapInfoAsync Error: {ex.Message}");
                return new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(-1));
            }
        }

        internal override async Task<PPPPlayer> GetPlayerInfo(long userId, PPPMapPool mapPool)
        {
            try
            {
                if (mapPool.Id == unsetPoolId) return new PPPPlayer(true);
                HitBloqUser player = await hitbloqapi.GetHitBloqUserByPool(userId, mapPool.Id);
                return new PPPPlayer(player);
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorHitBloq GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        internal override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage, PPPMapPool mapPool)
        {
            try
            {
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                HitBloqLadder ladder = await hitbloqapi.GetPlayerListForMapPool(fetchIndexPage, mapPool.Id);
                foreach (var hitBloqPlayer in ladder.ladder)
                {
                    lsPlayer.Add(new PPPPlayer(hitBloqPlayer));
                }
                return lsPlayer;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorHitBloq GetPlayers Error: {ex.Message}");
                return new List<PPPPlayer>();
            }
        }

        internal override async Task<int> GetPPToRank(string mappoolid, double pp)
        {
            if (string.IsNullOrEmpty(mappoolid)) return -1;
            try
            {
                HitBloqRankFromCr rank = await hitbloqapi.GetPlayerRankByCr(mappoolid, pp);
                return rank.rank;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorHitBloq GetPPToRank Error: {ex.Message}");
                return -1;
            }
        }

        internal override async Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page, PPPMapPool mapPool)
        {
            try
            {
                List<HitBloqScores> lsHitBloqSCores = await hitbloqapi.GetRecentScores(userId, mapPool.Id, page - 1);
                return new PPPScoreCollection(lsHitBloqSCores, page);
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorHitBloq GetRecentScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        internal override async Task<PPPScoreCollection> GetAllScores(string userId, PPPMapPool mapPool)
        {
            try
            {
                if(mapPool.Id == unsetPoolId) return new PPPScoreCollection();
                List<HitBloqScores> lsHitBloqSCores = await hitbloqapi.GetAllScores(userId, mapPool.Id);
                return new PPPScoreCollection(lsHitBloqSCores, 0);
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorHitBloq GetAllScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        public async Task<string> UpdateUserId()
        {
            try
            {
                string platformUserId = _settings.UserId;
                HitBloqUserId userId = await hitbloqapi.GetHitBloqUserIdByUserId(platformUserId);
                if (userId != null)
                {
                    return userId.id.ToString();
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorHitBloq UpdateUserId Error: {ex.Message}");
            }
            return null;
        }

        public override async Task UpdateAvailableMapPools()
        {
            var defaultMapPool = new PPPMapPool(MapPoolType.Default, $"☞ Select a map pool ☜", 0, 0, new CustomPPPCurve(new List<(double, double)>(), CurveType.Linear, 0));
            if (!_dctMapPool.ContainsKey(defaultMapPool.Id))  _dctMapPool.Add(defaultMapPool.Id, defaultMapPool);
            List<HitBloqMapPool> hbMapPool = await hitbloqapi.GetHitBloqMapPools();
            string customUserId = await UpdateUserId();
            //check if this map pool is already in list
            foreach (HitBloqMapPool newMapPool in hbMapPool)
            {
                if(_dctMapPool.TryGetValue(newMapPool.id, out PPPMapPool mapPool))
                {
                    if (DateTime.UtcNow.AddDays(-1) < mapPool.DtUtcLastRefresh)
                    {
                        continue; //Do not get Playlist if it has been updated less than a day ago.
                    }
                }
                else
                {
                    PPPMapPool insertMapPool = new PPPMapPool(newMapPool.id, newMapPool.id, MapPoolType.Custom, newMapPool.title, 0, 0, CustomPPPCurve.CreateDummyPPPCurve(), newMapPool.image, newMapPool.popularity, newMapPool.download_url);
                    insertMapPool.CustomLeaderboardUserId = customUserId;
                    if (!_dctMapPool.ContainsKey(insertMapPool.Id)) _dctMapPool.Add(insertMapPool.Id, insertMapPool);
                }
            }
            SendMapPoolRefreshed();
        }

        internal override List<PPPMapPoolShort> GetMapPools()
        {
            switch (_settings.HitbloqMapPoolSorting)
            {
                case MapPoolSorting.Alphabetical:
                    return _dctMapPool.Values.OrderBy(x => x.MapPoolType).ThenBy(x => x.MapPoolName).Select(x => (PPPMapPoolShort)x).ToList();
                case MapPoolSorting.Popularity:
                    return _dctMapPool.Values.OrderBy(x => x.MapPoolType).ThenByDescending(x => x.Popularity).ThenBy(x => x.MapPoolName).Select(x => (PPPMapPoolShort)x).ToList();
                default:
                    return new List<PPPMapPoolShort>();
            }
        }

        override internal async Task InternalUpdateMapPoolDetails(PPPMapPool mapPool)
        {
            if (mapPool.MapPoolType != MapPoolType.Default) //Filter out default 
            {
                HitBloqMapPoolDetails mapPoolDetails = await this.hitbloqapi.GetHitBloqMapPoolDetails(mapPool.Id, 0);
                mapPool.AccumulationConstant = mapPoolDetails.accumulation_constant;
                if(mapPoolDetails.cr_curve != null)
                {
                    mapPool.Curve = new CustomPPPCurve(mapPoolDetails.cr_curve);
                }
            }
        }

        public override string CreateSeachString(string hash, BeatmapKey beatmapKey)
        {
            return $"{hash}|_{beatmapKey.difficulty}_Solo{beatmapKey.serializedName}".Replace(Constants.OldDots, "");
        }

        public static (string, string, string) ParseHashDiffAndMode(string input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    string[] arrEntries = input.Split('_');
                    if (arrEntries.Count() == 3)
                    {
                        string hash = arrEntries[0];
                        string diff = DctDiffLengthen[arrEntries[1]];
                        string mode = DctCharLengthen[arrEntries[2]];
                        return (hash, diff, mode);
                    }
                    return (string.Empty, string.Empty, string.Empty);
                }

            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorHitBloq ParseHashDiffAndMode '{input}' Error: {ex} ");
            }
            return (string.Empty, string.Empty, string.Empty);
        }
        internal override bool IsScoreSetOnCurrentMapPool(PPPMapPool mapPool, PPPScoreSetData score) { return true; }
    }
}
