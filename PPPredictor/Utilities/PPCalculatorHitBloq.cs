using PPPredictor.Data;
using PPPredictor.Data.Curve;
using PPPredictor.OpenAPIs;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    public class PPCalculatorHitBloq : PPCalculator
    {
        private readonly hitbloqapi hitbloqapi;
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
        private static Dictionary<string, string> dctCharLengthen { get => dctCharShorten.ToDictionary(x => x.Value, x => x.Key); }
        private static Dictionary<string, string> dctDiffLengthen { get => dctDiffShorten.ToDictionary(x => x.Value, x => x.Key); }
        public PPCalculatorHitBloq() : base()
        {
            playerPerPages = 10;
            hitbloqapi = new hitbloqapi();
            UpdateUserId();
            UpdateAvailableMapPools(); //TODO: implement for all?
        }

        public override double ApplyModifierMultiplierToStars(PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed = false)
        {
            return levelFailed ? 0 : beatMapInfo.BaseStars;
        }

        public override async Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo)
        {
            try
            {
                if (beatMapInfo.SelectedCustomBeatmapLevel != null)
                {
                    string songHash = Hashing.GetCustomLevelHash(beatMapInfo.SelectedCustomBeatmapLevel);
                    string searchString = CreateSeachString(songHash, beatMapInfo.Beatmap);
                    if (_leaderboardInfo.CurrentMapPool.MapPoolType == MapPoolType.Custom && !_leaderboardInfo.CurrentMapPool.LsMapPoolEntries.Where(x => x.Searchstring == searchString).Any())
                    {
                        return new PPPBeatMapInfo(beatMapInfo, 0); //Currently selected map is not contained in selected MapPool
                    }
                    ShortScore cachedInfo = _leaderboardInfo.CurrentMapPool.LsLeaderboardScores?.FirstOrDefault(x => x.Searchstring.ToUpper() == searchString.ToUpper());
                    bool refetchInfo = cachedInfo != null && cachedInfo.FetchTime < DateTime.Now.AddDays(-7);
                    if (cachedInfo == null || refetchInfo)
                    {
                        if (refetchInfo) _leaderboardInfo.CurrentMapPool.LsLeaderboardScores?.Remove(cachedInfo);
                        HitBloqLeaderboardInfo leaderboardInfo = await hitbloqapi.GetLeaderBoardInfo(searchString);
                        if (leaderboardInfo != null)
                        {
                            if (leaderboardInfo.star_rating.TryGetValue(_leaderboardInfo.CurrentMapPool.Id, out var stars))
                            {
                                Plugin.Log?.Error($"GetStarsForBeatmapAsync Step GetNew Stars from backend: {stars}");
                                _leaderboardInfo.CurrentMapPool.LsLeaderboardScores.Add(new ShortScore(searchString, stars, DateTime.Now));
                                return new PPPBeatMapInfo(beatMapInfo, stars);
                            }
                        }
                    }
                    else
                    {
                        return new PPPBeatMapInfo(beatMapInfo, cachedInfo.Stars);
                    }
                }
                return beatMapInfo;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader PPCalculatorHitBloq Error: {ex.Message}");
                return new PPPBeatMapInfo(beatMapInfo, -1);
            }
        }

        protected override async Task<PPPPlayer> GetPlayerInfo(long userId)
        {
            try
            {
                HitBloqUser player = await hitbloqapi.GetHitBloqUserByPool(userId, _leaderboardInfo.CurrentMapPool.Id);
                return new PPPPlayer(player);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorHitBloq GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        protected override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage)
        {
            try
            {
                Plugin.Log?.Error($"PPCalculatorHitBloq GetPlayers {fetchIndexPage}");
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                HitBloqLadder ladder = await hitbloqapi.GetPlayerListForMapPool(fetchIndexPage, _leaderboardInfo.CurrentMapPool.Id);
                Plugin.Log.Error($"GetPlayers index: {fetchIndexPage}");
                foreach (var hitBloqPlayer in ladder.ladder)
                {
                    lsPlayer.Add(new PPPPlayer(hitBloqPlayer));
                }
                return lsPlayer;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorHitBloq GetPlayers Error: {ex.Message}");
                return new List<PPPPlayer>();
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
                Plugin.Log?.Error($"PPCalculatorHitBloq GetRecentScores Error: {ex.Message}");
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
                Plugin.Log?.Error($"PPCalculatorHitBloq UpdateUserId Error: {ex.Message}");
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
                    oldPool = new PPPMapPool(newMapPool.id, newMapPool.id, MapPoolType.Custom, newMapPool.title, 0, 0, CustomPPPCurve.DummyPPPCurve(), newMapPool.image, newMapPool.popularity);
                    _leaderboardInfo.LsMapPools.Add(oldPool);
                }
            }
            _leaderboardInfo.LsMapPools = _leaderboardInfo.LsMapPools.OrderBy(x => x.MapPoolType).ThenByDescending(x => x.Popularity).ThenBy(x => x.MapPoolName).ToList();
        }

        override public async Task UpdateMapPoolDetails(PPPMapPool mapPool)
        {
            mapPool.LsMapPoolEntries.Clear();
            bool needMoreData = true;
            int page = 0;
            while (needMoreData)
            {
                if (mapPool.MapPoolType != MapPoolType.Default) //Filter out default 
                {
                    HitBloqMapPoolDetails mapPoolDetails = await this.hitbloqapi.GetHitBloqMapPoolDetails(mapPool.Id, page);
                    mapPool.AccumulationConstant = mapPoolDetails.accumulation_constant;
                    mapPool.Curve = new CustomPPPCurve(mapPoolDetails.cr_curve);
                    if (mapPoolDetails.leaderboard_id_list.Count == 0)
                    {
                        needMoreData = false;
                    }
                    foreach (string songSearchString in mapPoolDetails.leaderboard_id_list)
                    {
                        mapPool.LsMapPoolEntries.Add(new PPPMapPoolEntry(songSearchString));
                    }
                    page++;
                }
                else
                {
                    needMoreData = false;
                }
            }
        }

        public override string CreateSeachString(string hash, IDifficultyBeatmap beatmap)
        {
            return $"{hash}|_{beatmap.difficulty}_Solo{beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName}";
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
                        string diff = dctDiffLengthen[arrEntries[1]];
                        string mode = dctCharLengthen[arrEntries[2]];
                        return (hash, diff, mode);
                    }
                    return (string.Empty, string.Empty, string.Empty);
                }

            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorHitBloq ParseHashDiffAndMode '{input}' Error: {ex} ");
            }
            return (string.Empty, string.Empty, string.Empty);
        }
    }
}
