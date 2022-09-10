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
        private string _ppGainRaw = "";
        private string _ppGainWeighted = "";
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Percentage)));
            }
        }

        public string PPGainRaw
        {
            set
            {
                _ppGainRaw = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGainRaw)));
            }
            get => _ppGainRaw;
        }

        public string PPGainWeighted
        {
            set
            {
                _ppGainWeighted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGainWeighted)));
            }
            get => _ppGainWeighted;
        }

        public string PPGainDiffColor
        {
            set
            {
                _ppGainDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGainDiffColor)));
            }
            get => _ppGainDiffColor;
        }

        #region UI Values session
        public string SessionRank
        {
            set
            {
                _sessionRank = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRank)));
            }
            get => _sessionRank;
        }
        public string SessionRankDiff
        {
            set
            {
                _sessionRankDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiff)));
            }
            get => _sessionRankDiff;
        }
        public string SessionRankDiffColor
        {
            set
            {
                _sessionRankDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiffColor)));
            }
            get => _sessionRankDiffColor;
        }

        public string SessionCountryRank
        {
            set
            {
                _sessionCountryRank = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRank)));
            }
            get => _sessionCountryRank;
        }
        public string SessionCountryRankDiff
        {
            set
            {
                _sessionCountryRankDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiff)));
            }
            get => _sessionCountryRankDiff;
        }
        public string SessionCountryRankDiffColor
        {
            set
            {
                _sessionCountryRankDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiffColor)));
            }
            get => _sessionCountryRankDiffColor;
        }

        public string SessionPP
        {
            set
            {
                _sessionPP = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPP)));
            }
            get => _sessionPP;
        }
        public string SessionPPDiff
        {
            set
            {
                _sessionPPDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPPDiff)));
            }
            get => _sessionPPDiff;
        }
        public string SessionPPDiffColor
        {
            set
            {
                _sessionPPDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPPDiffColor)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRank)));
            }
            get => _predictedRank;
        }
        public string PredictedRankDiff
        {
            set
            {
                _predictedRankDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiff)));
            }
            get => _predictedRankDiff;
        }
        public string PredictedRankDiffColor
        {
            set
            {
                _predictedRankDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiffColor)));
            }
            get => _predictedRankDiffColor;
        }
        public string PredictedCountryRank
        {
            set
            {
                _predictedCountryRank = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRank)));
            }
            get => _predictedCountryRank;
        }
        public string PredictedCountryRankDiff
        {
            set
            {
                _predictedCountryRankDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiff)));
            }
            get => _predictedCountryRankDiff;
        }
        public string PredictedCountryRankDiffColor
        {
            set
            {
                _predictedCountryRankDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiffColor)));
            }
            get => _predictedCountryRankDiffColor;
        }
        #endregion

        public bool IsDataLoading
        {
            set
            {
                _isDataLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDataLoading)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNoDataLoading)));
            }
            get => _isDataLoading;
        }

        public bool IsNoDataLoading
        {
            get => !_isDataLoading;
        }
        #endregion

        #region loadInfos
        internal void LoadInfos()
        {
            //TODO: Put in the constructor?? Dont need extra classes??
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

        public async void DisplayPP()
        {
            double pp = _ppCalculator.CalculatePPatPercentage(_currentSelectionStars, _percentage);
            PPGainResult ppGainResult = _ppCalculator.GetPlayerScorePPGain(_selectedMapSearchString, pp);
            double ppGains = _ppCalculator.Zeroizer(ppGainResult.PpGain);
            PPGainRaw = $"{pp:F2}pp";
            PPGainWeighted = $"{ppGains:+0.##;-0.##;0}pp";
            PPGainDiffColor = DisplayHelper.GetDisplayColor(ppGains, false);

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
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            await _ppCalculator.GetPlayerScores(userInfo.platformUserId, fetchLength);
            DisplayPP();
            IsDataLoading = false;
        }

        public async Task UpdateCurrentAndCheckResetSession(bool doResetSession)
        {
            IsDataLoading = true;
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            PPPPlayer player = await _ppCalculator.GetProfile(userInfo.platformUserId);
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
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            await _ppCalculator.GetPlayerScores(userInfo.platformUserId, 100);
            DisplayPP();
            IsDataLoading = false;
        }

        private bool NeedsResetSession()
        {
            return
                Plugin.ProfileInfo.ResetSessionHours > 0
                && (DateTime.Now - Plugin.ProfileInfo.LastSessionReset).TotalHours > Plugin.ProfileInfo.ResetSessionHours;
        }
    }
}
