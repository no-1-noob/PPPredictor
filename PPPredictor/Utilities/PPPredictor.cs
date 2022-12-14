using PPPredictor.Data;
using PPPredictor.Interfaces;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    internal class PPPredictor<T> : IPPPredictor where T : PPCalculator, new()
    {
        internal Leaderboard leaderboardName;
        #region displayValues
        private float _percentage;
        private string _ppRaw = "";
        private string _ppGain = "";
        private string _ppGainDiffColor = "white";

        private double _currentSelectionBaseStars;
        private double _currentSelectionStars;

        private string _sessionRank = "";
        private string _sessionCountryRank = "";
        private string _sessionPP = "";
        private string _sessionCountryRankDiff = "";
        private string _sessionCountryRankDiffColor = "white";
        private string _sessionRankDiff = "";
        private string _sessionRankDiffColor = "white";
        private string _sessionPPDiff = "";
        private string _sessionPPDiffColor = "white";

        private string _predictedRank = "";
        private string _predictedRankDiff = "";
        private string _predictedRankDiffColor = "white";
        private string _predictedCountryRank = "";
        private string _predictedCountryRankDiff = "";
        private string _predictedCountryRankDiffColor = "white";

        private bool _rankGainRunning = false;
        private double _lastPPGainCall = 0;

        #endregion

        #region internal values
        private string _selectedMapSearchString;
        private bool _isDataLoading = false;
        private bool _isActive = false;
        private bool _isUserFound = true;
        #endregion

        internal PPPLeaderboardInfo _leaderboardInfo;
        internal PPCalculator _ppCalculator;
        private GameplayModifiers _gameplayModifiers;
        internal PropertyChangedEventHandler PropertyChanged;

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
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Percentage)));
            }
        }

        public string PPRaw
        {
            set
            {
                _ppRaw = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPRaw)));
            }
            get => _ppRaw;
        }

        public string PPGain
        {
            set
            {
                _ppGain = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGain)));
            }
            get => _ppGain;
        }

        public string PPGainDiffColor
        {
            set
            {
                _ppGainDiffColor = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGainDiffColor)));
            }
            get => _ppGainDiffColor;
        }

        #region UI Values session
        public string SessionRank
        {
            set
            {
                _sessionRank = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRank)));
            }
            get => _sessionRank;
        }
        public string SessionRankDiff
        {
            set
            {
                _sessionRankDiff = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiff)));
            }
            get => _sessionRankDiff;
        }
        public string SessionRankDiffColor
        {
            set
            {
                _sessionRankDiffColor = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiffColor)));
            }
            get => _sessionRankDiffColor;
        }

        public string SessionCountryRank
        {
            set
            {
                _sessionCountryRank = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRank)));
            }
            get => _sessionCountryRank;
        }
        public string SessionCountryRankDiff
        {
            set
            {
                _sessionCountryRankDiff = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiff)));
            }
            get => _sessionCountryRankDiff;
        }
        public string SessionCountryRankDiffColor
        {
            set
            {
                _sessionCountryRankDiffColor = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiffColor)));
            }
            get => _sessionCountryRankDiffColor;
        }

        public string SessionPP
        {
            set
            {
                _sessionPP = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPP)));
            }
            get => _sessionPP;
        }
        public string SessionPPDiff
        {
            set
            {
                _sessionPPDiff = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPPDiff)));
            }
            get => _sessionPPDiff;
        }
        public string SessionPPDiffColor
        {
            set
            {
                _sessionPPDiffColor = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPPDiffColor)));
            }
            get => _sessionPPDiffColor;
        }
        #endregion
        #region UI Values Predicted Rank

        public string PredictedRank
        {
            set
            {
                _predictedRank = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRank)));
            }
            get => _predictedRank;
        }
        public string PredictedRankDiff
        {
            set
            {
                _predictedRankDiff = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiff)));
            }
            get => _predictedRankDiff;
        }
        public string PredictedRankDiffColor
        {
            set
            {
                _predictedRankDiffColor = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiffColor)));
            }
            get => _predictedRankDiffColor;
        }
        public string PredictedCountryRank
        {
            set
            {
                _predictedCountryRank = value;
                if(_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRank)));
            }
            get => _predictedCountryRank;
        }
        public string PredictedCountryRankDiff
        {
            set
            {
                _predictedCountryRankDiff = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiff)));
            }
            get => _predictedCountryRankDiff;
        }
        public string PredictedCountryRankDiffColor
        {
            set
            {
                _predictedCountryRankDiffColor = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiffColor)));
            }
            get => _predictedCountryRankDiffColor;
        }
        #endregion

        public bool IsDataLoading
        {
            set
            {
                _isDataLoading = value;
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDataLoading)));
                if (_isActive) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNoDataLoading)));
            }
            get => _isDataLoading;
        }

        public bool IsNoDataLoading
        {
            get => !_isDataLoading;
        }

        public string LeaderBoardName
        {
            get => _leaderboardInfo.LeaderboardName;
        }

        public bool IsUserFound
        {
            set
            {
                _isUserFound = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUserFound)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNoUserFound)));
            }
            get => _isUserFound;
        }
        public bool IsNoUserFound
        {
            get => !_isUserFound;
        }
        #endregion

        #region loadInfos
        internal void LoadInfos()
        {
            _leaderboardInfo = Plugin.ProfileInfo.LsLeaderboardInfo.Find(x => x.LeaderboardName == leaderboardName.ToString());
            if(_leaderboardInfo == null)
            {
                _leaderboardInfo = new PPPLeaderboardInfo(leaderboardName);
                Plugin.ProfileInfo.LsLeaderboardInfo.Add(_leaderboardInfo);
            }
        }
        #endregion

        #region eventHandling
        public void SetPropertyChangedEventHandler(PropertyChangedEventHandler propertyChanged)
        {
            PropertyChanged = propertyChanged;
        }

        public void ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController)
        {
            if (gameplaySetupViewController != null && gameplaySetupViewController.gameplayModifiers != null)
            {
                _gameplayModifiers = gameplaySetupViewController.gameplayModifiers;
                _currentSelectionStars = _ppCalculator.ApplyModifierMultiplierToStars(_currentSelectionBaseStars, _gameplayModifiers);
                DisplayPP();
            }
        }

        public async void DifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            _currentSelectionBaseStars = await _ppCalculator.GetStarsForBeatmapAsync(lvlSelectionNavigationCtrl, beatmap);
            _selectedMapSearchString = lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel ? _ppCalculator.CreateSeachString(Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel), lvlSelectionNavigationCtrl.selectedDifficultyBeatmap.difficultyRank) : string.Empty;
            _currentSelectionStars = _ppCalculator.ApplyModifierMultiplierToStars(_currentSelectionBaseStars, _gameplayModifiers);
            DisplayPP();
        }

        public async void DetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType)
        {
            if (contentType == StandardLevelDetailViewController.ContentType.OwnedAndReady)
            {
                _currentSelectionBaseStars = await _ppCalculator.GetStarsForBeatmapAsync(lvlSelectionNavigationCtrl, lvlSelectionNavigationCtrl.selectedDifficultyBeatmap);
                _selectedMapSearchString = lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel ? _ppCalculator.CreateSeachString(Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel), lvlSelectionNavigationCtrl.selectedDifficultyBeatmap.difficultyRank) : string.Empty;
                _currentSelectionStars = _ppCalculator.ApplyModifierMultiplierToStars(_currentSelectionBaseStars, _gameplayModifiers);
                DisplayPP();
            }
        }
        #endregion
        public double CalculatePPatPercentage(double percentage, bool levelFailed = false)
        {
            double stars = _currentSelectionStars;
            if (levelFailed) stars = _ppCalculator.ApplyModifierMultiplierToStars(_currentSelectionBaseStars, _gameplayModifiers, levelFailed); //Recalculate stars for beatleader
            return _ppCalculator.CalculatePPatPercentage(stars, percentage, levelFailed);
        }

        public double CalculatePPGain(double pp)
        {
            return _ppCalculator.GetPlayerScorePPGain(_selectedMapSearchString, pp).GetDisplayPPValue();
        }

        public bool IsRanked()
        {
            return _currentSelectionBaseStars > 0;
        }

        public async void DisplayPP()
        {
            double pp = CalculatePPatPercentage(_percentage);
            PPGainResult ppGainResult = _ppCalculator.GetPlayerScorePPGain(_selectedMapSearchString, pp);
            double ppGains = _ppCalculator.Zeroizer(ppGainResult.GetDisplayPPValue());
            PPRaw = $"{pp:F2}pp";
            PPGain = $"{ppGains:+0.##;-0.##;0}pp";
            PPGainDiffColor = DisplayHelper.GetDisplayColor(ppGains, false, true);

            RankGainResult rankGain = new RankGainResult(1, 2, 3, 4);
            DisplayRankGain(null);
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
            DisplayRankGain(rankGain);
        }

        private void DisplayRankGain(RankGainResult rankGainResult)
        {
            if (rankGainResult != null)
            {
                PredictedRank = $"{rankGainResult.RankGlobal:N0}";
                PredictedRankDiff = rankGainResult.RankGainGlobal.ToString("+#;-#;0");
                PredictedRankDiffColor = DisplayHelper.GetDisplayColor(rankGainResult.RankGainGlobal, false);
                PredictedCountryRank = $"{rankGainResult.RankCountry:N0}";
                PredictedCountryRankDiff = rankGainResult.RankGainCountry.ToString("+#;-#;0");
                PredictedCountryRankDiffColor = DisplayHelper.GetDisplayColor(rankGainResult.RankGainCountry, false);
            }
            else
            {
                PredictedRank = "...";
                PredictedRankDiff = "?";
                PredictedRankDiffColor = DisplayHelper.GetDisplayColor(0, false);
                PredictedCountryRank = "...";
                PredictedCountryRankDiff = "?";
                PredictedCountryRankDiffColor = DisplayHelper.GetDisplayColor(0, false);
            }
        }

        private void DisplaySession()
        {
            if (_leaderboardInfo.SessionPlayer == null || _leaderboardInfo.CurrentPlayer == null)
            {
                SessionRank = SessionCountryRank = SessionPP = SessionCountryRankDiff = SessionRankDiff = SessionPPDiff = "-";
                SessionCountryRankDiffColor = SessionRankDiffColor = SessionPPDiffColor = "white";
            }
            else
            {
                if (Plugin.ProfileInfo.DisplaySessionValues)
                {
                    SessionRank = $"{_leaderboardInfo.SessionPlayer.Rank}";
                    SessionCountryRank = $"{_leaderboardInfo.SessionPlayer.CountryRank}";
                    SessionPP = $"{_leaderboardInfo.SessionPlayer.Pp:F2}pp";
                }
                else
                {
                    SessionRank = $"{_leaderboardInfo.CurrentPlayer.Rank}";
                    SessionCountryRank = $"{_leaderboardInfo.CurrentPlayer.CountryRank}";
                    SessionPP = $"{_leaderboardInfo.CurrentPlayer.Pp:F2}pp";
                }
                SessionCountryRankDiff = (_leaderboardInfo.CurrentPlayer.CountryRank - _leaderboardInfo.SessionPlayer.CountryRank).ToString("+#;-#;0");
                SessionCountryRankDiffColor = DisplayHelper.GetDisplayColor((_leaderboardInfo.CurrentPlayer.CountryRank - _leaderboardInfo.SessionPlayer.CountryRank), true);
                SessionRankDiff = (_leaderboardInfo.CurrentPlayer.Rank - _leaderboardInfo.SessionPlayer.Rank).ToString("+#;-#;0");
                SessionRankDiffColor = DisplayHelper.GetDisplayColor((_leaderboardInfo.CurrentPlayer.Rank - _leaderboardInfo.SessionPlayer.Rank), true);
                SessionPPDiff = $"{_ppCalculator.Zeroizer(_leaderboardInfo.CurrentPlayer.Pp - _leaderboardInfo.SessionPlayer.Pp):+0.##;-0.##;0}pp";
                SessionPPDiffColor = DisplayHelper.GetDisplayColor((_leaderboardInfo.CurrentPlayer.Pp - _leaderboardInfo.SessionPlayer.Pp), false);
            }
        }
        public async void RefreshCurrentData(int fetchLength)
        {
            await UpdateCurrentAndCheckResetSession(false);
            IsDataLoading = true;
            UserInfo userInfo = await GetUserInfo();
            await _ppCalculator.GetPlayerScores(userInfo.platformUserId, fetchLength);
            DisplayPP();
            IsDataLoading = false;
        }

        public async Task UpdateCurrentAndCheckResetSession(bool doResetSession)
        {
            IsDataLoading = true;
            UserInfo userInfo = await GetUserInfo();
            PPPPlayer player = await _ppCalculator.GetProfile(userInfo.platformUserId);
            if (player.IsErrorUser)
            {
                IsUserFound = false;
            }
            else
            {
                IsUserFound = true;
            }
            _leaderboardInfo.CurrentPlayer = player;
            if (doResetSession || _leaderboardInfo.SessionPlayer == null || NeedsResetSession())
            {
                Plugin.ProfileInfo.LastSessionReset = DateTime.Now;
                _leaderboardInfo.SessionPlayer = player;
            }
            DisplaySession();
            IsDataLoading = false;
        }

        public async void ResetDisplay(bool resetAll)
        {
            await UpdateCurrentAndCheckResetSession(resetAll);
            IsDataLoading = true;
            UserInfo userInfo = await GetUserInfo();
            await _ppCalculator.GetPlayerScores(userInfo.platformUserId, 100);
            DisplayPP();
            IsDataLoading = false;
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
            DisplayPP();
            if (_isActive) RefreshAllDisplayValues();
        }

        private void RefreshAllDisplayValues()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LeaderBoardName)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Percentage)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPRaw)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGain)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGainDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRank)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRank)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPP)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPPDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPPDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRank)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRank)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDataLoading)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNoDataLoading)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUserFound)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNoUserFound)));
        }

        private async Task<UserInfo> GetUserInfo()
        {
            return await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
        }
    }
}
