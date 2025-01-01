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
using PPPredictor.Core.DataType.MapPool;

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
                if(currentMapPool != null)
                {
                    Plugin.ProfileInfo.MapPoolSelection[leaderboardName.ToString()] = currentMapPool.Id;
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

        public void ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController)
        {
            if (gameplaySetupViewController != null && gameplaySetupViewController.gameplayModifiers != null)
            {
                _gameplayModifiers = gameplaySetupViewController.gameplayModifiers;
                _currentBeatMapInfo = instance.ApplyModifiersToBeatmapInfo(leaderboardName, currentMapPool.Id, _currentBeatMapInfo, Converter.Converter.ConvertGameplayModifiers(_gameplayModifiers));
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
            _currentBeatMapInfo = await instance.GetBeatMapInfoAsync(leaderboardName, currentMapPool.Id, _currentBeatMapInfo);
            _currentBeatMapInfo.SelectedMapSearchString = !string.IsNullOrEmpty(_currentBeatMapInfo.CustomLevelHash) ? PPCalculator.CreateSeachString(_currentBeatMapInfo.CustomLevelHash, "SOLO" + _currentBeatMapInfo.BeatmapKey.serializedName, Core.ParsingUtil.ParseDifficultyNameToInt(_currentBeatMapInfo.BeatmapKey.difficulty.ToString())) : string.Empty;
            _currentBeatMapInfo.OldDotsEnabled = IsOldDotsActive(_currentBeatMapInfo.BeatmapKey);
            _currentBeatMapInfo = GetModifiedBeatMapInfo(_gameplayModifiers);
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
        public double CalculatePPatPercentage(double percentage, PPPBeatMapInfo beatMapInfo, bool levelFailed = false, bool levelPaused = false)
        {
            var v = instance.CalculatePPatPercentage(leaderboardName, currentMapPool.Id, beatMapInfo, percentage, levelFailed, levelPaused);
            return v;
        }

        public double CalculateMaxPP()
        {
            var v = instance.CalculateMaxPP(leaderboardName, currentMapPool.Id, _currentBeatMapInfo);
            return v;
        }

        public PPPBeatMapInfo GetModifiedBeatMapInfo(GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            PPPBeatMapInfo pppBeatMapInfo = instance.ApplyModifiersToBeatmapInfo(leaderboardName, currentMapPool.Id, _currentBeatMapInfo, Converter.Converter.ConvertGameplayModifiers(gameplayModifiers), levelFailed, levelPaused);
            return pppBeatMapInfo;
        }

        public bool IsOldDotsActive(Core.DataType.BeatSaberEncapsulation.BeatmapKey beatmapKey)
        {
            return beatmapKey?.serializedName?.Contains(Core.Constants.OldDots) ?? false;
        }

        public double CalculatePPGain(double pp)
        {
            PPGainResult ppGainResult = instance.GetPlayerScorePPGain(leaderboardName, currentMapPool.Id, _currentBeatMapInfo.SelectedMapSearchString, pp);
            return ppGainResult.PpDisplayValue;
        }

        public bool IsRanked()
        {
            try
            {
                return _currentBeatMapInfo.BaseStarRating.IsRanked() && (_leaderboardInfo.HasOldDotRanking || !_currentBeatMapInfo.OldDotsEnabled);
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"IsRanked {ex.Message}");
                return false;
            }
        }

        public double? GetPersonalBest()
        {
            return instance.GetPersonalBest(leaderboardName, currentMapPool.Id, _currentBeatMapInfo.SelectedMapSearchString);
        }

        internal bool IsCurrentMapPoolChanging(object value)
        {
            
            var currentPool = CurrentMapPool as PPPMapPoolShort;
            var newMapPool = value as PPPMapPoolShort;
            return (currentPool == null || (currentPool != null && newMapPool != null && currentPool.Id != newMapPool.Id));
        }

        public void CalculatePP()
        {
            if(currentMapPool == null) return;
            if (_currentBeatMapInfo.MaxPP == -1) _currentBeatMapInfo.MaxPP = CalculateMaxPP();
            double pp = CalculatePPatPercentage(_percentage, _currentBeatMapInfo);
            _ppGainResult = instance.GetPlayerScorePPGain(leaderboardName, currentMapPool.Id, _currentBeatMapInfo.SelectedMapSearchString, pp);
            double ppGains = PPCalculator.Zeroizer(_ppGainResult.PpDisplayValue);
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
                rankGain = await instance.GetPlayerRankGain(leaderboardName, currentMapPool.Id, _ppGainResult.PpTotal);
                _rankGainRunning = false;
            }
            if (_lastPPGainCall > 0)
            {
                _rankGainRunning = true;
                rankGain = await instance.GetPlayerRankGain(leaderboardName, currentMapPool.Id, _lastPPGainCall);
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

        private async Task DisplaySession(bool doResetSession)
        {
            (PPPPlayer sessionPlayer, PPPPlayer currentPlayer) = await instance.UpdatePlayer(leaderboardName, currentMapPool.Id, doResetSession);
            DisplaySessionInfo sessionDisplay = new DisplaySessionInfo();
            if (sessionPlayer != null && currentPlayer != null)
            {
                if (Plugin.ProfileInfo.DisplaySessionValues)
                {
                    sessionDisplay.SessionRank = $"{sessionPlayer.Rank:N0}";
                    sessionDisplay.SessionCountryRank = $"{sessionPlayer.CountryRank:N0}";
                    sessionDisplay.SessionPP = $"{sessionPlayer.Pp:F2}{_leaderboardInfo.PpSuffix}";
                }
                else
                {
                    sessionDisplay.SessionRank = $"{currentPlayer.Rank:N0}";
                    sessionDisplay.SessionCountryRank = $"{currentPlayer.CountryRank:N0}";
                    sessionDisplay.SessionPP = $"{currentPlayer.Pp:F2}{_leaderboardInfo.PpSuffix}";
                }
                sessionDisplay.SessionCountryRankDiff = (currentPlayer.CountryRank - sessionPlayer.CountryRank).ToString("+#;-#;0");
                sessionDisplay.SessionCountryRankDiffColor = _leaderboardInfo.IsCountryRankEnabled ? DisplayHelper.GetDisplayColor((currentPlayer.CountryRank - sessionPlayer.CountryRank), true) : DisplayHelper.ColorCountryRankDisabled;
                sessionDisplay.SessionRankDiff = (currentPlayer.Rank - sessionPlayer.Rank).ToString("+#;-#;0");
                sessionDisplay.SessionRankDiffColor = DisplayHelper.GetDisplayColor((currentPlayer.Rank - sessionPlayer.Rank), true);
                sessionDisplay.SessionPPDiff = $"{PPCalculator.Zeroizer(currentPlayer.Pp - sessionPlayer.Pp):+0.##;-0.##;0}{_leaderboardInfo.PpSuffix}";
                sessionDisplay.SessionPPDiffColor = DisplayHelper.GetDisplayColor((currentPlayer.Pp - sessionPlayer.Pp), false);
            }
            sessionDisplay.CountryRankFontColor = _leaderboardInfo.IsCountryRankEnabled ? DisplayHelper.ColorWhite : DisplayHelper.ColorCountryRankDisabled;
            SendDisplaySessionInfo(sessionDisplay);
        }

        public void ScoreSet(PPPScoreSetData data)
        {
            if(instance.IsScoreSetOnCurrentMapPool(leaderboardName, currentMapPool.Id, data)) 
                RefreshCurrentData(1, false, true);
        }

        public async void RefreshCurrentData(int fetchLength, bool refreshStars = false, bool fetchOnePage = false)
        {
            await UpdateCurrentAndCheckResetSession(false);
            IsDataLoading(true);
            await instance.GetPlayerScores(leaderboardName, currentMapPool.Id, fetchLength, _leaderboardInfo.LargePageSize, fetchOnePage);
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
            await DisplaySession(doResetSession);
            IsDataLoading(false);
        }

        public async void ResetDisplay(bool resetAll)
        {
            await UpdateCurrentAndCheckResetSession(resetAll);
            IsDataLoading(true);
            await instance.GetPlayerScores(leaderboardName, currentMapPool.Id, 100, 100);
            CalculatePP();
            IsDataLoading(false);
        }

        public void SetActive(bool setActive)
        {
            _isActive = setActive;
            _ = DisplaySession(false);
            CalculatePP();
        }

        private async void UpdateMapPoolDetails()
        {
            if (currentMapPool != null)
            {
                IsDataLoading(true);
                await instance.UpdateMapPoolDetails(leaderboardName, currentMapPool.Id);
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
