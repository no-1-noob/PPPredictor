﻿using PPPredictor.Data;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Interfaces;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    internal class PPPredictor<T> : IPPPredictor where T : PPCalculator, new()
    {
        internal Leaderboard leaderboardName;
        #region internal values
        private float _percentage;
        private PPPBeatMapInfo _currentBeatMapInfo = new PPPBeatMapInfo();
        private bool _rankGainRunning = false;
        private double _lastPPGainCall = 0;
        private bool _isActive = false;
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
                string mapPoolIcon = _leaderboardInfo.CurrentMapPool?.IconUrl ?? string.Empty;
                return !string.IsNullOrEmpty(mapPoolIcon) ? mapPoolIcon : _leaderboardInfo.LeaderboardIcon;
            }
        }
        public byte[] MapPoolIconData
        {
            get { return _leaderboardInfo.CurrentMapPool?.IconData; }
            set { _leaderboardInfo.CurrentMapPool.IconData = value; }
        }

        internal PPPLeaderboardInfo _leaderboardInfo;
        internal PPCalculator _ppCalculator;
        private GameplayModifiers _gameplayModifiers;

        public event EventHandler<bool> OnDataLoading;
        public event EventHandler<DisplaySessionInfo> OnDisplaySessionInfo;
        public event EventHandler<DisplayPPInfo> OnDisplayPPInfo;
        public event EventHandler OnMapPoolRefreshed;

        #region
        public PPPredictor(Leaderboard leaderBoard)
        {
            leaderboardName = leaderBoard;
            LoadInfos();
            _ppCalculator = new T() { _leaderboardInfo = _leaderboardInfo };

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
                Plugin.Log?.Error($"Get map-pool-options");
                return _leaderboardInfo.LsMapPools.Select(f => (object)f).ToList();
            }
        }
        public object CurrentMapPool
        {
            get => (object)_leaderboardInfo.CurrentMapPool;
            set
            {

                _leaderboardInfo.CurrentMapPool = (PPPMapPool)value;
                UpdateMapPoolDetails();
                Plugin.Log?.Error($"Set MapPool {value} Player: {_leaderboardInfo.CurrentMapPool.CurrentPlayer}");
                SetActive(true);
            }
        }
        #endregion
        public string PPSuffix
        {
            get => _leaderboardInfo.PpSuffix;
        }
        #endregion
        #region loadInfos
        internal void LoadInfos()
        {
            _leaderboardInfo = Plugin.ProfileInfo.LsLeaderboardInfo.Find(x => x.LeaderboardName == leaderboardName.ToString());
            if(_leaderboardInfo != null)
            {
                _leaderboardInfo.SetCurrentMapPool();
            }
            if (_leaderboardInfo == null)
            {
                _leaderboardInfo = new PPPLeaderboardInfo(leaderboardName);
                Plugin.ProfileInfo.LsLeaderboardInfo.Add(_leaderboardInfo);
            }
        }
        #endregion

        #region eventHandling

        public void ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController)
        {
            if (gameplaySetupViewController != null && gameplaySetupViewController.gameplayModifiers != null)
            {
                _gameplayModifiers = gameplaySetupViewController.gameplayModifiers;
                _currentBeatMapInfo.CurrentSelectionStars = _ppCalculator.ApplyModifierMultiplierToStars(_currentBeatMapInfo, _gameplayModifiers);
                _currentBeatMapInfo.MaxPP = -1;
                CalculatePP();
            }
        }

        public async void DifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            await UpdateCurrentBeatMapInfos(lvlSelectionNavigationCtrl, beatmap);
        }

        public async void DetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType)
        {
            if (contentType == StandardLevelDetailViewController.ContentType.OwnedAndReady)
            {
                await UpdateCurrentBeatMapInfos(lvlSelectionNavigationCtrl, lvlSelectionNavigationCtrl.selectedDifficultyBeatmap);
            }
        }

        private async Task UpdateCurrentBeatMapInfos(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            await UpdateCurrentBeatMapInfos(lvlSelectionNavigationCtrl.selectedBeatmapLevel as CustomBeatmapLevel, beatmap);
        }

        public async Task UpdateCurrentBeatMapInfos(CustomBeatmapLevel selectedBeatmapLevel, IDifficultyBeatmap beatmap)
        {
            _currentBeatMapInfo = new PPPBeatMapInfo(selectedBeatmapLevel, beatmap);
            await UpdateCurrentBeatMapInfos();
            CalculatePP();
        }

        private async Task UpdateCurrentBeatMapInfos()
        {
            _currentBeatMapInfo = await _ppCalculator.GetBeatMapInfoAsync(_currentBeatMapInfo);
            _currentBeatMapInfo.SelectedMapSearchString = _currentBeatMapInfo.SelectedCustomBeatmapLevel != null ? _ppCalculator.CreateSeachString(Hashing.GetCustomLevelHash(_currentBeatMapInfo.SelectedCustomBeatmapLevel), "SOLO" + _currentBeatMapInfo.Beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName, _currentBeatMapInfo.Beatmap.difficultyRank) : string.Empty;
            _currentBeatMapInfo.CurrentSelectionStars = GetModifiedStars(_gameplayModifiers);
            _currentBeatMapInfo.MaxPP = -1;
        }
        #endregion

        #region event sending
        private void IsDataLoading(bool isDataLoading)
        {
            if (_isActive) OnDataLoading?.Invoke(this, isDataLoading);
        }
        private void SendDisplayPPInfo(DisplayPPInfo displayPPInfo)
        {
            if(_isActive) OnDisplayPPInfo?.Invoke(this, displayPPInfo);
        }
        private void SendDisplaySessionInfo(DisplaySessionInfo displaySessionInfo)
        {
            if (_isActive) OnDisplaySessionInfo?.Invoke(this, displaySessionInfo);
        }
        private void SendMapPoolRefresh()
        {
            OnMapPoolRefreshed?.Invoke(this, null);
        }
        #endregion
        public double CalculatePPatPercentage(double currentStars, double percentage, GameplayModifiers gameplayModifiers, bool levelFailed = false)
        {
            if (levelFailed) currentStars = _ppCalculator.ApplyModifierMultiplierToStars(_currentBeatMapInfo, gameplayModifiers, levelFailed); //Recalculate stars for beatleader
            return _ppCalculator.CalculatePPatPercentage(currentStars, percentage, levelFailed);
        }

        public double CalculateMaxPP(double modifiedStars)
        {
            return _ppCalculator.CalculateMaxPP(modifiedStars);
        }

        public double GetModifiedStars(GameplayModifiers gameplayModifiers)
        {
            return _ppCalculator.ApplyModifierMultiplierToStars(_currentBeatMapInfo, gameplayModifiers);
        }

        public double CalculatePPGain(double pp)
        {
            return _ppCalculator.GetPlayerScorePPGain(_currentBeatMapInfo.SelectedMapSearchString, pp).GetDisplayPPValue();
        }

        public bool IsRanked()
        {
            return _currentBeatMapInfo.BaseStars > 0;
        }

        public async void CalculatePP()
        {
            if (_currentBeatMapInfo.MaxPP == -1) _currentBeatMapInfo.MaxPP = CalculateMaxPP(_currentBeatMapInfo.CurrentSelectionStars);
            double pp = CalculatePPatPercentage(_currentBeatMapInfo.CurrentSelectionStars, _percentage, _gameplayModifiers);
            PPGainResult ppGainResult = _ppCalculator.GetPlayerScorePPGain(_currentBeatMapInfo.SelectedMapSearchString, pp);
            double ppGains = _ppCalculator.Zeroizer(ppGainResult.GetDisplayPPValue());
            DisplayPPInfo ppDisplay = new DisplayPPInfo();
            if (_currentBeatMapInfo.MaxPP > 0 && pp >= _currentBeatMapInfo.MaxPP)
            {
                ppDisplay.PPRaw = $"<color=\"yellow\">{pp:F2}{PPSuffix}</color>";
            }
            else
            {
                ppDisplay.PPRaw = $"{pp:F2}{PPSuffix}";
            }
            ppDisplay.PPGain = $"{ppGains:+0.##;-0.##;0}{PPSuffix}";
            ppDisplay.PPGainDiffColor = DisplayHelper.GetDisplayColor(ppGains, false, true);

            RankGainResult rankGain = new RankGainResult(1, 2, 3, 4);
            DisplayRankGain(null, ppDisplay);
            if (_rankGainRunning)
            {
                _lastPPGainCall = ppGainResult.PpTotal;
                return;
            }

            if (_lastPPGainCall == 0)
            {
                _rankGainRunning = true;
                rankGain = await _ppCalculator.GetPlayerRankGain(ppGainResult.PpTotal);
                _rankGainRunning = false;
            }
            if (_lastPPGainCall > 0)
            {
                _rankGainRunning = true;
                rankGain = await _ppCalculator.GetPlayerRankGain(_lastPPGainCall);
                _rankGainRunning = false;
                _lastPPGainCall = 0;
            }
            DisplayRankGain(rankGain, ppDisplay);
        }

        private void DisplayRankGain(RankGainResult rankGainResult, DisplayPPInfo ppDisplay)
        {
            if (rankGainResult != null)
            {
                ppDisplay.PredictedRank = $"{rankGainResult.RankGlobal:N0}";
                ppDisplay.PredictedRankDiff = rankGainResult.RankGainGlobal.ToString("+#;-#;0");
                ppDisplay.PredictedRankDiffColor = DisplayHelper.GetDisplayColor(rankGainResult.RankGainGlobal, false);
                ppDisplay.PredictedCountryRank = $"{rankGainResult.RankCountry:N0}";
                ppDisplay.PredictedCountryRankDiff = rankGainResult.RankGainCountry.ToString("+#;-#;0");
                ppDisplay.PredictedCountryRankDiffColor = DisplayHelper.GetDisplayColor(rankGainResult.RankGainCountry, false);
            }
            else
            {
                ppDisplay.PredictedRank = "...";
                ppDisplay.PredictedRankDiff = "?";
                ppDisplay.PredictedRankDiffColor = DisplayHelper.GetDisplayColor(0, false);
                ppDisplay.PredictedCountryRank = "...";
                ppDisplay.PredictedCountryRankDiff = "?";
                ppDisplay.PredictedCountryRankDiffColor = DisplayHelper.GetDisplayColor(0, false);
            }
            SendDisplayPPInfo(ppDisplay);
        }

        private void DisplaySession()
        {
            DisplaySessionInfo sessionDisplay = new DisplaySessionInfo();
            if (_leaderboardInfo.CurrentMapPool.SessionPlayer != null && _leaderboardInfo.CurrentMapPool.CurrentPlayer != null)
            {
                if (Plugin.ProfileInfo.DisplaySessionValues)
                {
                    sessionDisplay.SessionRank = $"{_leaderboardInfo.CurrentMapPool.SessionPlayer.Rank}";
                    sessionDisplay.SessionCountryRank = $"{_leaderboardInfo.CurrentMapPool.SessionPlayer.CountryRank}";
                    sessionDisplay.SessionPP = $"{_leaderboardInfo.CurrentMapPool.SessionPlayer.Pp:F2}{PPSuffix}";
                }
                else
                {
                    sessionDisplay.SessionRank = $"{_leaderboardInfo.CurrentMapPool.CurrentPlayer.Rank}";
                    sessionDisplay.SessionCountryRank = $"{_leaderboardInfo.CurrentMapPool.CurrentPlayer.CountryRank}";
                    sessionDisplay.SessionPP = $"{_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp:F2}{PPSuffix}";
                }
                sessionDisplay.SessionCountryRankDiff = (_leaderboardInfo.CurrentMapPool.CurrentPlayer.CountryRank - _leaderboardInfo.CurrentMapPool.SessionPlayer.CountryRank).ToString("+#;-#;0");
                sessionDisplay.SessionCountryRankDiffColor = DisplayHelper.GetDisplayColor((_leaderboardInfo.CurrentMapPool.CurrentPlayer.CountryRank - _leaderboardInfo.CurrentMapPool.SessionPlayer.CountryRank), true);
                sessionDisplay.SessionRankDiff = (_leaderboardInfo.CurrentMapPool.CurrentPlayer.Rank - _leaderboardInfo.CurrentMapPool.SessionPlayer.Rank).ToString("+#;-#;0");
                sessionDisplay.SessionRankDiffColor = DisplayHelper.GetDisplayColor((_leaderboardInfo.CurrentMapPool.CurrentPlayer.Rank - _leaderboardInfo.CurrentMapPool.SessionPlayer.Rank), true);
                sessionDisplay.SessionPPDiff = $"{_ppCalculator.Zeroizer(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp - _leaderboardInfo.CurrentMapPool.SessionPlayer.Pp):+0.##;-0.##;0}{PPSuffix}";
                sessionDisplay.SessionPPDiffColor = DisplayHelper.GetDisplayColor((_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp - _leaderboardInfo.CurrentMapPool.SessionPlayer.Pp), false);
            }
            SendDisplaySessionInfo(sessionDisplay);
        }
        public async void RefreshCurrentData(int fetchLength, bool refreshStars = false)
        {
            await UpdateCurrentAndCheckResetSession(false);
            IsDataLoading(true);
            string userId = await GetUserInfo();
            await _ppCalculator.GetPlayerScores(userId, fetchLength);
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
            string userId = await GetUserInfo();
            PPPPlayer player = await _ppCalculator.GetProfile(userId);
            _leaderboardInfo.CurrentMapPool.CurrentPlayer = player;
            if (doResetSession || _leaderboardInfo.CurrentMapPool.SessionPlayer == null || NeedsResetSession())
            {
                Plugin.ProfileInfo.LastSessionReset = DateTime.Now;
                _leaderboardInfo.CurrentMapPool.SessionPlayer = player;
            }
            DisplaySession();
            IsDataLoading(false);
        }

        public async void ResetDisplay(bool resetAll)
        {
            await UpdateCurrentAndCheckResetSession(resetAll);
            IsDataLoading(true);
            string userId = await GetUserInfo();
            await _ppCalculator.GetPlayerScores(userId, 100);
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
            if (_leaderboardInfo.CurrentMapPool != null && _leaderboardInfo.CurrentMapPool.DtUtcLastRefresh < DateTime.UtcNow.AddDays(-1))
            {
                IsDataLoading(true);
                await _ppCalculator.UpdateMapPoolDetails(_leaderboardInfo.CurrentMapPool);
                _leaderboardInfo.CurrentMapPool.DtUtcLastRefresh = DateTime.UtcNow;
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
                        MapPoolIconData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.Error($"GetMapPoolIconData {MapPoolIcon}: {ex.Message}");
            }
        }
    }
}
