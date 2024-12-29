using PPPredictor.Core;
using PPPredictor.Core.Calculator;
using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.LeaderBoard;
using PPPredictor.Data;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Interfaces;
using PPPredictor.Converter;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Utilities
{
    internal class PPPredictor : IPPPredictor
    {
        internal Leaderboard leaderboardName;
        #region internal values
        private float _percentage;
        private PPPBeatMapInfo _currentBeatMapInfo = new PPPBeatMapInfo();
        private bool _rankGainRunning = false;
        private double _lastPPGainCall = 0;
        private bool _isActive = false;
        private int _loadingCounter = 0;
        private DisplayPPInfo _ppDisplay = new DisplayPPInfo();
        private PPGainResult _ppGainResult = new PPGainResult();
        private Timer _rankTimer;
        private readonly Instance instance;
#warning set selected map pool
        private string selectedMapPoolId;
        private PPPMapPoolShort currentMapPool;
        private List<PPPMapPoolShort> lsMapPools = new List<PPPMapPoolShort>();
        #endregion

        public string LeaderBoardName
        {
            get
            {
                return _leaderboardInfo.LeaderboardName;
            }
        }
        public string LeaderBoardIcon
        {
            get { return _leaderboardInfo.LeaderboardIcon; }
        }
        public string MapPoolIcon
        {
            get
            {
                string mapPoolIcon = currentMapPool?.IconUrl ?? string.Empty;
                return !string.IsNullOrEmpty(mapPoolIcon) ? mapPoolIcon : _leaderboardInfo.LeaderboardIcon;
            }
        }
        public byte[] MapPoolIconData
        {
            get { return currentMapPool?.IconData; }
            set { currentMapPool.IconData = value; }
        }

        internal PPPLeaderboardInfo _leaderboardInfo;
        private GameplayModifiers _gameplayModifiers;

        public event EventHandler<bool> OnDataLoading;
        public event EventHandler<DisplaySessionInfo> OnDisplaySessionInfo;
        public event EventHandler<DisplayPPInfo> OnDisplayPPInfo;
        public event EventHandler OnMapPoolRefreshed;

        #region
        public PPPredictor(Leaderboard leaderBoard, Instance instance)
        {
            leaderboardName = leaderBoard;
            this.instance = instance;
            _leaderboardInfo = new PPPLeaderboardInfo(leaderBoard);
            lsMapPools = this.instance.GetMapPools(leaderboardName);
            //_ppCalculator.OnMapPoolRefreshed += PPCalculator_OnMapPoolRefreshed;


            _rankTimer = new System.Timers.Timer(500);
            _rankTimer.Elapsed += OnRankTimerElapsed;
            _rankTimer.AutoReset = false;
            _rankTimer.Enabled = false;

        }
        #endregion

        #region getter/setter
        public float Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
            }
        }
        #region MapPools
        public List<object> MapPoolOptions
        {
            get
            {
                return lsMapPools.Select(f => (object)f).ToList();
            }
        }
        public object CurrentMapPool
        {
            get => (object)currentMapPool;
            set
            {
                bool isCurrentMapPoolChanging = IsCurrentMapPoolChanging(value);
                currentMapPool = (PPPMapPoolShort)value;
                UpdateMapPoolDetails();
                if (isCurrentMapPoolChanging)
                {
                    this.RefreshCurrentData(10, true);
                }
                SetActive(true);
            }
        }
        #endregion
        public string PPSuffix
        {
            get => _leaderboardInfo.PpSuffix;
        }
        #endregion

        #region eventHandling

        public async Task ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController)
        {
            if (gameplaySetupViewController != null && gameplaySetupViewController.gameplayModifiers != null)
            {
                _gameplayModifiers = gameplaySetupViewController.gameplayModifiers;
                _currentBeatMapInfo = await instance.ApplyModifiersToBeatmapInfo(leaderboardName, selectedMapPoolId, _currentBeatMapInfo, Converter.Converter.ConvertGameplayModifiers(_gameplayModifiers));
                _currentBeatMapInfo.MaxPP = -1;
                CalculatePP();
            }
        }

        public async void DifficultyChanged(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmapKey)
        {
            await UpdateCurrentBeatMapInfos(selectedBeatmapLevel, beatmapKey);
        }

        public async Task UpdateCurrentBeatMapInfos(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmapKey)
        {
            var v = Converter.Converter.ConvertBeatmapKey(beatmapKey);
            _currentBeatMapInfo = new PPPBeatMapInfo(Hashing.GetCustomLevelHash(selectedBeatmapLevel), Converter.Converter.ConvertBeatmapKey(beatmapKey));
            await UpdateCurrentBeatMapInfos();
            CalculatePP();
        }

        private async Task UpdateCurrentBeatMapInfos()
        {
            _currentBeatMapInfo = await instance.GetBeatMapInfoAsync(leaderboardName, selectedMapPoolId, _currentBeatMapInfo);
            _currentBeatMapInfo.SelectedMapSearchString = !string.IsNullOrEmpty(_currentBeatMapInfo.CustomLevelHash) ? PPCalculator.CreateSeachString(_currentBeatMapInfo.CustomLevelHash, "SOLO" + _currentBeatMapInfo.BeatmapKey.serializedName, Core.ParsingUtil.ParseDifficultyNameToInt(_currentBeatMapInfo.BeatmapKey.difficulty.ToString())) : string.Empty;
            _currentBeatMapInfo.OldDotsEnabled = IsOldDotsActive(_currentBeatMapInfo.BeatmapKey);
            _currentBeatMapInfo = await GetModifiedBeatMapInfo(_gameplayModifiers);
            _currentBeatMapInfo.MaxPP = -1;
        }
        #endregion

        #region event sending
        private void IsDataLoading(bool isDataLoading)
        {
            _loadingCounter += isDataLoading ? +1 : -1;
            if (_isActive && ((isDataLoading && _loadingCounter == 1) || (!isDataLoading && _loadingCounter == 0))) OnDataLoading?.Invoke(this, isDataLoading);
        }
        private void SendDisplayPPInfo(DisplayPPInfo displayPPInfo)
        {
            if(_isActive) OnDisplayPPInfo?.Invoke(this, displayPPInfo);
        }
        private void SendDisplaySessionInfo(DisplaySessionInfo displaySessionInfo)
        {
            if (_isActive) OnDisplaySessionInfo?.Invoke(this, displaySessionInfo);
        }
        private void PPCalculator_OnMapPoolRefreshed(object sender, EventArgs e)
        {
            OnMapPoolRefreshed?.Invoke(this, null);
        }
        #endregion
        public async Task<double> CalculatePPatPercentage(double percentage, PPPBeatMapInfo beatMapInfo, bool levelFailed = false, bool levelPaused = false)
        {
            var v = await instance.CalculatePPatPercentage(leaderboardName, selectedMapPoolId, beatMapInfo, percentage, levelFailed, levelPaused);
            return v;
        }

        public async Task<double> CalculateMaxPP()
        {
            var v = await instance.CalculateMaxPP(leaderboardName, selectedMapPoolId, _currentBeatMapInfo);
            return v;
        }

        public async Task<PPPBeatMapInfo> GetModifiedBeatMapInfo(GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            PPPBeatMapInfo pppBeatMapInfo = await instance.ApplyModifiersToBeatmapInfo(leaderboardName, selectedMapPoolId, _currentBeatMapInfo, Converter.Converter.ConvertGameplayModifiers(gameplayModifiers), levelFailed, levelPaused);
            return pppBeatMapInfo;
        }

        public bool IsOldDotsActive(Core.DataType.BeatSaberEncapsulation.BeatmapKey beatmapKey)
        {
            return beatmapKey.serializedName.Contains(Core.Constants.OldDots);
        }

        public async Task<double> CalculatePPGain(double pp)
        {
            PPGainResult ppGainResult = await instance.GetPlayerScorePPGain(leaderboardName, selectedMapPoolId, _currentBeatMapInfo.SelectedMapSearchString, pp);
            return ppGainResult.GetDisplayPPValue();
        }

        public async Task<bool> IsRanked()
        {
            return _currentBeatMapInfo.BaseStarRating.IsRanked() && (await instance.HasOldDotRanking(leaderboardName, selectedMapPoolId) || !_currentBeatMapInfo.OldDotsEnabled);
        }

        public async Task<double?> GetPersonalBest()
        {
            return await instance.GetPersonalBest(leaderboardName, selectedMapPoolId, _currentBeatMapInfo.SelectedMapSearchString);
        }

        internal bool IsCurrentMapPoolChanging(object value)
        {
            var currentPool = CurrentMapPool as PPPMapPoolShort;
            var newMapPool = value as PPPMapPoolShort;
            return (currentPool != null && newMapPool != null && currentPool.Id != newMapPool.Id);
        }

        public async void CalculatePP()
        {
            if (_currentBeatMapInfo.MaxPP == -1) _currentBeatMapInfo.MaxPP = await CalculateMaxPP();
            double pp = await CalculatePPatPercentage(_percentage, _currentBeatMapInfo);
            _ppGainResult = await instance.GetPlayerScorePPGain(leaderboardName, selectedMapPoolId, _currentBeatMapInfo.SelectedMapSearchString, pp);
            double ppGains = PPCalculator.Zeroizer(_ppGainResult.GetDisplayPPValue());
            _ppDisplay = new DisplayPPInfo();
            if (_currentBeatMapInfo.MaxPP > 0 && pp >= _currentBeatMapInfo.MaxPP)
            {
                _ppDisplay.PPRaw = $"<color=\"yellow\">{pp:F2}{PPSuffix}</color>";
            }
            else
            {
                _ppDisplay.PPRaw = $"{pp:F2}{PPSuffix}";
            }
            _ppDisplay.PPGain = $"{ppGains:+0.##;-0.##;0}{PPSuffix}";
            _ppDisplay.PPGainDiffColor = DisplayHelper.GetDisplayColor(ppGains, false, true);

            DisplayRankGain(null, _ppDisplay);
            //Restart rank calculation timer
            _rankTimer.Stop();
            _rankTimer.Start();
        }

        private async void OnRankTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RankGainResult rankGain = new RankGainResult(1, 2, 3, 4);
            if (_rankGainRunning)
            {
                _lastPPGainCall = _ppGainResult.PpTotal;
                return;
            }
            if (_lastPPGainCall == 0)
            {
                _rankGainRunning = true;
                rankGain = await instance.GetPlayerRankGain(leaderboardName, selectedMapPoolId, _ppGainResult.PpTotal);
                _rankGainRunning = false;
            }
            if (_lastPPGainCall > 0)
            {
                _rankGainRunning = true;
                rankGain = await instance.GetPlayerRankGain(leaderboardName, selectedMapPoolId, _lastPPGainCall);
                _rankGainRunning = false;
                _lastPPGainCall = 0;
            }
            DisplayRankGain(rankGain, _ppDisplay);
        }

        private void DisplayRankGain(RankGainResult rankGainResult, DisplayPPInfo ppDisplay)
        {
            if (rankGainResult != null)
            {
                ppDisplay.PredictedRankDiffColor = DisplayHelper.GetDisplayColor(rankGainResult.RankGainGlobal, false);
#warning leaderboard info muss weg?
                ppDisplay.PredictedCountryRankDiffColor = _leaderboardInfo.IsCountryRankEnabled ? DisplayHelper.GetDisplayColor(rankGainResult.RankGainCountry, false) : DisplayHelper.ColorCountryRankDisabled;
                if (rankGainResult.IsRankGainCanceledByLimit)
                {
                    ppDisplay.PredictedRank = $"<{rankGainResult.RankGlobal:N0}";
                    ppDisplay.PredictedRankDiff = rankGainResult.RankGainGlobal.ToString(">+#;<-#;0");
                    ppDisplay.PredictedCountryRank = $"<{rankGainResult.RankCountry:N0}";
                    ppDisplay.PredictedCountryRankDiff = rankGainResult.RankGainCountry.ToString(">+#;<-#;0");
                }
                else
                {
                    ppDisplay.PredictedRank = $"{rankGainResult.RankGlobal:N0}";
                    ppDisplay.PredictedRankDiff = rankGainResult.RankGainGlobal.ToString("+#;-#;0");
                    ppDisplay.PredictedCountryRank = $"{rankGainResult.RankCountry:N0}";
                    ppDisplay.PredictedCountryRankDiff = rankGainResult.RankGainCountry.ToString("+#;-#;0");
                }
            }
            else
            {
                ppDisplay.PredictedRank = "...";
                ppDisplay.PredictedRankDiff = "?";
                ppDisplay.PredictedRankDiffColor = DisplayHelper.GetDisplayColor(0, false);
                ppDisplay.PredictedCountryRank = "...";
                ppDisplay.PredictedCountryRankDiff = "?";
                ppDisplay.PredictedCountryRankDiffColor = _leaderboardInfo.IsCountryRankEnabled ? DisplayHelper.GetDisplayColor(0, false) : DisplayHelper.ColorCountryRankDisabled;
            }
            SendDisplayPPInfo(ppDisplay);
        }

        private void DisplaySession()
        {
            DisplaySessionInfo sessionDisplay = new DisplaySessionInfo();
            if (currentMapPool.SessionPlayer != null && currentMapPool.CurrentPlayer != null)
            {
                if (Plugin.ProfileInfo.DisplaySessionValues)
                {
                    sessionDisplay.SessionRank = $"{currentMapPool.SessionPlayer.Rank:N0}";
                    sessionDisplay.SessionCountryRank = $"{currentMapPool.SessionPlayer.CountryRank:N0}";
                    sessionDisplay.SessionPP = $"{currentMapPool.SessionPlayer.Pp:F2}{PPSuffix}";
                }
                else
                {
                    sessionDisplay.SessionRank = $"{currentMapPool.CurrentPlayer.Rank:N0}";
                    sessionDisplay.SessionCountryRank = $"{currentMapPool.CurrentPlayer.CountryRank:N0}";
                    sessionDisplay.SessionPP = $"{currentMapPool.CurrentPlayer.Pp:F2}{PPSuffix}";
                }
                sessionDisplay.SessionCountryRankDiff = (currentMapPool.CurrentPlayer.CountryRank - currentMapPool.SessionPlayer.CountryRank).ToString("+#;-#;0");
                sessionDisplay.SessionCountryRankDiffColor = _leaderboardInfo.IsCountryRankEnabled ? DisplayHelper.GetDisplayColor((currentMapPool.CurrentPlayer.CountryRank - currentMapPool.SessionPlayer.CountryRank), true) : DisplayHelper.ColorCountryRankDisabled;
                sessionDisplay.SessionRankDiff = (currentMapPool.CurrentPlayer.Rank - currentMapPool.SessionPlayer.Rank).ToString("+#;-#;0");
                sessionDisplay.SessionRankDiffColor = DisplayHelper.GetDisplayColor((currentMapPool.CurrentPlayer.Rank - currentMapPool.SessionPlayer.Rank), true);
                sessionDisplay.SessionPPDiff = $"{PPCalculator.Zeroizer(currentMapPool.CurrentPlayer.Pp - currentMapPool.SessionPlayer.Pp):+0.##;-0.##;0}{PPSuffix}";
                sessionDisplay.SessionPPDiffColor = DisplayHelper.GetDisplayColor((currentMapPool.CurrentPlayer.Pp - currentMapPool.SessionPlayer.Pp), false);
            }
            sessionDisplay.CountryRankFontColor = _leaderboardInfo.IsCountryRankEnabled ? DisplayHelper.ColorWhite : DisplayHelper.ColorCountryRankDisabled;
            SendDisplaySessionInfo(sessionDisplay);
        }

        public async Task ScoreSet(PPPScoreSetData data)
        {
            if(await instance.IsScoreSetOnCurrentMapPool(leaderboardName, selectedMapPoolId, data)) 
                RefreshCurrentData(1, false, true);
        }

        public async void RefreshCurrentData(int fetchLength, bool refreshStars = false, bool fetchOnePage = false)
        {
            await UpdateCurrentAndCheckResetSession(false);
            IsDataLoading(true);
            await instance.GetPlayerScores(leaderboardName, selectedMapPoolId, fetchLength, _leaderboardInfo.LargePageSize, fetchOnePage);
            if (refreshStars) //MapPool change to a pool that has never been selected before;
            {
                await UpdateCurrentBeatMapInfos();
            }
            CalculatePP();
            IsDataLoading(false);
        }

        public async Task UpdateCurrentAndCheckResetSession(bool doResetSession)
        {
            IsDataLoading(true);
            PPPPlayer player = await instance.GetProfile(leaderboardName, selectedMapPoolId);
            currentMapPool.CurrentPlayer = player;
            if (doResetSession || currentMapPool.SessionPlayer == null || NeedsResetSession())
            {
                Plugin.ProfileInfo.LastSessionReset = DateTime.Now;
                currentMapPool.SessionPlayer = player;
            }
            DisplaySession();
            IsDataLoading(false);
        }

        public async void ResetDisplay(bool resetAll)
        {
            await UpdateCurrentAndCheckResetSession(resetAll);
            IsDataLoading(true);
            await instance.GetPlayerScores(leaderboardName, selectedMapPoolId, 100, 100);
            CalculatePP();
            IsDataLoading(false);
        }

        private bool NeedsResetSession()
        {
            return
                Plugin.ProfileInfo.ResetSessionHours > 0
                && ((DateTime.Now - Plugin.ProfileInfo.LastSessionReset).TotalHours > Plugin.ProfileInfo.ResetSessionHours
                || (DateTime.Now - Plugin.ProfileInfo.LastSessionReset).TotalMinutes < 1); //Parallel reset of multiple scoreboards
        }

        public void SetActive(bool setActive)
        {
            _isActive = setActive;
            DisplaySession();
            CalculatePP();
        }

        private async Task<string> GetUserInfo()
        {
            if (string.IsNullOrEmpty(_leaderboardInfo.CustomLeaderboardUserId))
            {
                return (await Plugin.GetUserInfoBS()).platformUserId;
            }
            else
            {
                return _leaderboardInfo.CustomLeaderboardUserId;
            }
        }

        private async void UpdateMapPoolDetails()
        {
            if (currentMapPool != null && currentMapPool.DtUtcLastRefresh < DateTime.UtcNow.AddDays(-1))
            {
                IsDataLoading(true);
                await instance.UpdateMapPoolDetails(leaderboardName, selectedMapPoolId);
                currentMapPool.DtUtcLastRefresh = DateTime.UtcNow;
                IsDataLoading(false);
            }
        }

        public async Task GetMapPoolIconData()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(MapPoolIcon))
                    {
                        byte[] rawData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        byte[] resizedData = DisplayHelper.ResizeImage(rawData, 128, 128);
                        MapPoolIconData = resizedData;
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"GetMapPoolIconData {MapPoolIcon} Error: {ex.Message}");
            }
        }

        public async Task<PPPMapPoolShort> FindPoolWithSyncURL(string syncUrl)
        {
            return await instance.FindPoolWithSyncURL(leaderboardName, syncUrl);
        }
    }
}
