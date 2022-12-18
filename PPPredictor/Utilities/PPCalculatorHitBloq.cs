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
    class PPCalculatorHitBloq : PPCalculator
    {
        private readonly hitbloqapi hitbloqapi;
        //Dctionaries defined like here: https://github.com/DaFluffyPotato/hitbloq/blob/1e7bf18f92f1146bf8da2f24769aea072542b6e5/general.py#L24
        private readonly Dictionary<string, string> dctDiffShorten = new Dictionary<string, string>{
            { "ExpertPlus", "ep" },
            { "Expert", "ex" },
            { "Hard", "h" },
            { "Normal", "n" },
            { "Easy", "e" }
        };
        private readonly Dictionary<string, string> dctCharShorten = new Dictionary<string, string>
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
        public PPCalculatorHitBloq() : base()
        {
            playerPerPages = 10;
            hitbloqapi = new hitbloqapi();
            UpdateUserId();
            UpdateAvailableMapPools(); //TODO: implement for all?
        }

        public override double ApplyModifierMultiplierToStars(double baseStars, GameplayModifiers gameplayModifiers, bool levelFailed = false)
        {
            return baseStars;
        }

        public override async Task<double> GetStarsForBeatmapAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            try
            {
                if (lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel)
                {
                    string songHash = Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel);
                    string searchString = CreateSeachString(songHash, beatmap);
                    if (_leaderboardInfo.CurrentMapPool.MapPoolType == MapPoolType.Custom && !_leaderboardInfo.CurrentMapPool.LsMapPoolEntries.Where(x => x.Searchstring == searchString).Any())
                    {
                        return 0; //Currently selected map is not contained in selected MapPool
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
                                return stars;
                            }
                        }
                    }
                    else
                    {
                        return cachedInfo.Stars;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorBeatLeader PPCalculatorHitBloq Error: {ex.Message}");
                return -1;
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
                string platformUserId = (await BS_Utils.Gameplay.GetUserInfo.GetUserAsync()).platformUserId;
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
            Plugin.Log?.Error($"PPCalculatorHitBloq UpdateAvailableMapPools");
            List<HitBloqMapPool> hbMapPool = await hitbloqapi.GetHitBloqMapPools();
            //check if this map pool is already in list
            foreach (HitBloqMapPool newMapPool in hbMapPool)
            {
                PPPMapPool oldPool = _leaderboardInfo.LsMapPools.Find(x => x.Id == newMapPool.id);
                if (newMapPool.id != "poodles") continue; //Ignore all other than poodles for now
                Plugin.Log?.Error($"PPCalculatorHitBloq UpdateAvailableMapPools Step1 {newMapPool.id}");
                if (oldPool != null && DateTime.UtcNow.AddDays(-1) < oldPool.DtUtcLastRefresh)
                {
                    continue; //Do not get Playlist if it has been updated less than a day ago.
                }
                await UpdateMapPoolPlaylist(newMapPool, oldPool);
            }
        }

        private async Task UpdateMapPoolPlaylist(HitBloqMapPool newMapPool, PPPMapPool oldPool)
        {
            if(oldPool != null) oldPool.LsMapPoolEntries.Clear();
            bool needMoreData = true;
            int page = 0;
            while (needMoreData)
            {
                Plugin.Log?.Error($"PPCalculatorHitBloq UpdateMapPoolPlaylist {newMapPool.title} - {page}");
                HitBloqMapPoolDetails mapPoolDetails = await this.hitbloqapi.GetHitBloqMapPoolDetails(newMapPool.id, page);
                if (oldPool == null)
                {
                    oldPool = new PPPMapPool(newMapPool.id, newMapPool.id, MapPoolType.Custom, newMapPool.title, mapPoolDetails.accumulation_constant, 0, new CustomPPPCurve(mapPoolDetails.cr_curve));
                    _leaderboardInfo.LsMapPools.Add(oldPool);
                }
                if(mapPoolDetails.leaderboard_id_list.Count == 0)
                {
                    needMoreData = false;
                }
                foreach (string songSearchString in mapPoolDetails.leaderboard_id_list)
                {
                    oldPool.LsMapPoolEntries.Add(new PPPMapPoolEntry(songSearchString));
                }
                page++;
            }
        }

        public override string CreateSeachString(string hash, IDifficultyBeatmap beatmap)
        {
            return $"{hash}|_{beatmap.difficulty}_Solo{beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName}";
        }

        public override string ParseMapSearchStringForGetPlayerScorePPGain(string searchString)
        {
            try
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    string hash = searchString.Split('|')[0];
                    string diffAndChar = searchString.Split('|')[1];
                    string diff = dctDiffShorten[diffAndChar.Split('_')[1]];
                    string chara = dctCharShorten[diffAndChar.Split('_')[2]];
                    string retVal = $"{hash}_{diff}_{chara}".ToUpper();
                    return retVal;
                }
                
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorHitBloq ParseMapSearchStringForGetPlayerScorePPGain '{searchString}' Error: {ex} ");
            }
            return string.Empty;
        }
    }
}
