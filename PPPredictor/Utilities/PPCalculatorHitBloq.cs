using PPPredictor.Data;
using PPPredictor.Data.Curve;
using PPPredictor.Interfaces;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PPPredictor.Data.LeaderBoardDataTypes.HitBloqDataTypes;

namespace PPPredictor.Utilities
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
        public PPCalculatorHitBloq() : base()
        {
            playerPerPages = 10;
            hasGetAllScoresFunctionality = true;
            hasPPToRankFunctionality = true;
            hitbloqapi = new HBAPI();
            UpdateUserId();
            UpdateAvailableMapPools(); //TODO: implement for all?
        }

        public override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            beatMapInfo.ModifiedStarRating = new PPPStarRating(levelFailed ? 0 : beatMapInfo.BaseStarRating.Stars);
            return beatMapInfo;
        }

        public override async Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo)
        {
            try
            {
                if (beatMapInfo.SelectedCustomBeatmapLevel != null)
                {
                    string songHash = Hashing.GetCustomLevelHash(beatMapInfo.SelectedCustomBeatmapLevel);
                    string searchString = CreateSeachString(songHash, beatMapInfo.BeatmapKey);
                    ShortScore cachedInfo = _leaderboardInfo.CurrentMapPool.LsLeaderboadInfo?.FirstOrDefault(x => x.Searchstring.ToUpper() == searchString.ToUpper());
                    bool refetchInfo = cachedInfo != null && cachedInfo.FetchTime < DateTime.Now.AddDays(ProfileInfo.RefetchMapInfoAfterDays);
                    if (cachedInfo == null || refetchInfo)
                    {
                        if (refetchInfo) _leaderboardInfo.CurrentMapPool.LsLeaderboadInfo?.Remove(cachedInfo);
                        HitBloqLeaderboardInfo leaderboardInfo = await hitbloqapi.GetLeaderBoardInfo(searchString);
                        if (leaderboardInfo != null)
                        {
                            leaderboardInfo.star_rating.TryGetValue(_leaderboardInfo.CurrentMapPool.Id, out var stars);
                            _leaderboardInfo.CurrentMapPool.LsLeaderboadInfo.Add(new ShortScore(searchString, new PPPStarRating(stars), DateTime.Now));
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
                Plugin.ErrorPrint($"PPCalculatorHitBloq GetBeatMapInfoAsync Error: {ex.Message}");
                return new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(-1));
            }
        }

        protected override async Task<PPPPlayer> GetPlayerInfo(long userId)
        {
            try
            {
                if (_leaderboardInfo.CurrentMapPool.Id == unsetPoolId) return new PPPPlayer(true);
                HitBloqUser player = await hitbloqapi.GetHitBloqUserByPool(userId, _leaderboardInfo.CurrentMapPool.Id);
                return new PPPPlayer(player);
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPCalculatorHitBloq GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        protected override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage)
        {
            try
            {
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                HitBloqLadder ladder = await hitbloqapi.GetPlayerListForMapPool(fetchIndexPage, _leaderboardInfo.CurrentMapPool.Id);
                foreach (var hitBloqPlayer in ladder.ladder)
                {
                    lsPlayer.Add(new PPPPlayer(hitBloqPlayer));
                }
                return lsPlayer;
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPCalculatorHitBloq GetPlayers Error: {ex.Message}");
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
                Plugin.ErrorPrint($"PPCalculatorHitBloq GetPPToRank Error: {ex.Message}");
                return -1;
            }
        }

        protected override async Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page)
        {
            try
            {
                List<HitBloqScores> lsHitBloqSCores = await hitbloqapi.GetRecentScores(userId, _leaderboardInfo.CurrentMapPool.Id, page - 1);
                return new PPPScoreCollection(lsHitBloqSCores, page);
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPCalculatorHitBloq GetRecentScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        protected override async Task<PPPScoreCollection> GetAllScores(string userId)
        {
            try
            {
                if(_leaderboardInfo.CurrentMapPool.Id == unsetPoolId) return new PPPScoreCollection();
                List<HitBloqScores> lsHitBloqSCores = await hitbloqapi.GetAllScores(userId, _leaderboardInfo.CurrentMapPool.Id);
                return new PPPScoreCollection(lsHitBloqSCores, 0);
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPCalculatorHitBloq GetAllScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        public async void UpdateUserId()
        {
            try
            {
                string platformUserId = (await Plugin.GetUserInfoBS()).platformUserId;
                HitBloqUserId userId = await hitbloqapi.GetHitBloqUserIdByUserId(platformUserId);
                if (userId != null)
                {
                    _leaderboardInfo.CustomLeaderboardUserId = userId.id.ToString();
                }
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPCalculatorHitBloq UpdateUserId Error: {ex.Message}");
            }
        }

        public async void UpdateAvailableMapPools()
        {
            List<HitBloqMapPool> hbMapPool = await hitbloqapi.GetHitBloqMapPools();
            //check if this map pool is already in list
            foreach (HitBloqMapPool newMapPool in hbMapPool)
            {
                PPPMapPool oldPool = _leaderboardInfo.LsMapPools.Find(x => x.Id == newMapPool.id);
                if (oldPool != null && DateTime.UtcNow.AddDays(-1) < oldPool.DtUtcLastRefresh)
                {
                    continue; //Do not get Playlist if it has been updated less than a day ago.
                }
                if (oldPool == null)
                {
                    oldPool = new PPPMapPool(newMapPool.id, newMapPool.id, MapPoolType.Custom, newMapPool.title, 0, 0, CustomPPPCurve.CreateDummyPPPCurve(), newMapPool.image, newMapPool.popularity, newMapPool.download_url);
                    _leaderboardInfo.LsMapPools.Add(oldPool);
                }
            }
            switch (Plugin.ProfileInfo.HitbloqMapPoolSorting)
            {
                case MapPoolSorting.Alphabetical:
                    _leaderboardInfo.LsMapPools = _leaderboardInfo.LsMapPools.OrderBy(x => x.MapPoolType).ThenBy(x => x.MapPoolName).ToList();
                    break;
                case MapPoolSorting.Popularity:
                    _leaderboardInfo.LsMapPools = _leaderboardInfo.LsMapPools.OrderBy(x => x.MapPoolType).ThenByDescending(x => x.Popularity).ThenBy(x => x.MapPoolName).ToList();
                    break;
            }
            SendMapPoolRefreshed();
        }

        override public async Task UpdateMapPoolDetails(PPPMapPool mapPool)
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
            return $"{hash}|_{beatmapKey.difficulty}_Solo{beatmapKey.beatmapCharacteristic.serializedName}".Replace(Constants.OldDots, "");
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
                Plugin.ErrorPrint($"PPCalculatorHitBloq ParseHashDiffAndMode '{input}' Error: {ex} ");
            }
            return (string.Empty, string.Empty, string.Empty);
        }

        public override bool IsScoreSetOnCurrentMapPool(PPPWebSocketData score) { return true; }
    }
}
