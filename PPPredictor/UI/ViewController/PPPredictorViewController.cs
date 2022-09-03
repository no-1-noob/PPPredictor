using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.FloatingScreen;
using PPPredictor.Data;
using PPPredictor.Utilities;
using scoresaberapi;
using SiraUtil.Web.SiraSync;
using SongCore.Utilities;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace PPPredictor.UI.ViewController
{
    [ViewDefinition("PPPredictor.UI.Views.PPPredictorView.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\PPPredictorView.bsml")]
    public class PPPredictorViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly LevelSelectionNavigationController levelSelectionNavController;
        private readonly GameplaySetupViewController gameplaySetupViewController;
        private FloatingScreen floatingScreen;
        private GameplayModifiers _gameplayModifiers;
        public event PropertyChangedEventHandler PropertyChanged;

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

        public PPPredictorViewController(LevelSelectionNavigationController levelSelectionNavController, GameplaySetupViewController gameplaySetupViewController)
        {
            this.levelSelectionNavController = levelSelectionNavController;
            this.gameplaySetupViewController = gameplaySetupViewController;
        }

        [Inject]
        public void Construct()
        {
        }
        public void Initialize()
        {
            Plugin.pppViewController = this;
            levelSelectionNavController.didChangeDifficultyBeatmapEvent += OnDifficultyChanged;
            levelSelectionNavController.didChangeLevelDetailContentEvent += OnDetailContentChanged;
            levelSelectionNavController.didActivateEvent += OnLevelSelectionActivated;
            levelSelectionNavController.didDeactivateEvent += OnLevelSelectionDeactivated;
            gameplaySetupViewController.didChangeGameplayModifiersEvent += didChangeGameplayModifiersEvent;
            floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(75, 100), true, Plugin.ProfileInfo.Position, new Quaternion(0, 0, 0, 0));
            floatingScreen.gameObject.name = "BSMLFloatingScreen_PPPredictor";
            floatingScreen.gameObject.SetActive(false);
            floatingScreen.ShowHandle = Plugin.ProfileInfo.WindowHandleEnabled;
            floatingScreen.transform.eulerAngles = Plugin.ProfileInfo.EulerAngles;
            floatingScreen.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            floatingScreen.HandleSide = FloatingScreen.Side.Left;
            floatingScreen.HandleReleased += OnScreenHandleReleased;
            BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "PPPredictor.UI.Views.PPPredictorView.bsml"), floatingScreen.gameObject, this);
        }

        private void didChangeGameplayModifiersEvent()
        {
            if (gameplaySetupViewController != null && gameplaySetupViewController.gameplayModifiers != null)
            {
                _gameplayModifiers = gameplaySetupViewController.gameplayModifiers;
                _currentSelectionStars = Plugin.PPCalculator.ApplyModifierMultiplierToStars(_currentSelectionBaseStars, _gameplayModifiers);
                DisplayPP();
            }
        }

        public void Dispose()
        {
            floatingScreen.HandleReleased -= OnScreenHandleReleased;
            levelSelectionNavController.didActivateEvent -= OnLevelSelectionActivated;
            levelSelectionNavController.didDeactivateEvent -= OnLevelSelectionDeactivated;
            gameplaySetupViewController.didChangeGameplayModifiersEvent -= didChangeGameplayModifiersEvent;
            Plugin.pppViewController = null;
        }

        [UIAction("#post-parse")]
        protected void PostParse()
        {
            ResetDisplay(false);
        }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        #region UI Components
        [UIComponent("sliderFine")]
        private readonly SliderSetting sliderFine;
        #endregion
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null

        #region buttons
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("refresh-profile-clicked")]
        private void RefreshProfileClicked()
        {
            RefreshCurrentData(10);
        }
#pragma warning restore IDE0051 // Remove unused private members

#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("reset-session-clicked")]
        private async void ResetSessionClicked()
        {
            await UpdateCurrentAndCheckResetSession(true);
        }
#pragma warning restore IDE0051 // Remove unused private members
        #endregion

        [UIValue("sliderCoarseValue")]
        private float SliderCoarseValue
        {
            get => ((float)Math.Floor(_percentage - _percentage % 10));
            set
            {
                sliderFine.slider.minValue = value;
                sliderFine.slider.maxValue = value + 10;
                SliderFineValue = (value) + (_percentage % 10);
                DisplayPP();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SliderCoarseValue)));
            }
        }
        [UIValue("sliderFineValue")]
        private float SliderFineValue
        {
            get => _percentage;
            set
            {
                _percentage = value;
                Plugin.ProfileInfo.LastPercentageSelected = _percentage;
                DisplayPP();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SliderFineValue)));
            }
        }

        [UIValue("ppGainRaw")]
        private string PPGainRaw
        {
            set
            {
                _ppGainRaw = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGainRaw)));
            }
            get => _ppGainRaw;
        }

        [UIValue("ppGainWeighted")]
        private string PPGainWeighted
        {
            set
            {
                _ppGainWeighted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGainWeighted)));
            }
            get => _ppGainWeighted;
        }

        [UIValue("ppGainDiffColor")]
        private string PPGainDiffColor
        {
            set
            {
                _ppGainDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGainDiffColor)));
            }
            get => _ppGainDiffColor;
        }

        #region UI Values session
        [UIValue("sessionRank")]
        private string SessionRank
        {
            set
            {
                _sessionRank = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRank)));
            }
            get => _sessionRank;
        }
        [UIValue("sessionRankDiff")]
        private string SessionRankDiff
        {
            set
            {
                _sessionRankDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiff)));
            }
            get => _sessionRankDiff;
        }
        [UIValue("sessionRankDiffColor")]
        private string SessionRankDiffColor
        {
            set
            {
                _sessionRankDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiffColor)));
            }
            get => _sessionRankDiffColor;
        }

        [UIValue("sessionCountryRank")]
        private string SessionCountryRank
        {
            set
            {
                _sessionCountryRank = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRank)));
            }
            get => _sessionCountryRank;
        }
        [UIValue("sessionCountryRankDiff")]
        private string SessionCountryRankDiff
        {
            set
            {
                _sessionCountryRankDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiff)));
            }
            get => _sessionCountryRankDiff;
        }
        [UIValue("sessionCountryRankDiffColor")]
        private string SessionCountryRankDiffColor
        {
            set
            {
                _sessionCountryRankDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiffColor)));
            }
            get => _sessionCountryRankDiffColor;
        }

        [UIValue("sessionPP")]
        private string SessionPP
        {
            set
            {
                _sessionPP = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPP)));
            }
            get => _sessionPP;
        }
        [UIValue("sessionPPDiff")]
        private string SessionPPDiff
        {
            set
            {
                _sessionPPDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPPDiff)));
            }
            get => _sessionPPDiff;
        }
        [UIValue("sessionPPDiffColor")]
        private string SessionPPDiffColor
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

        [UIValue("predictedRank")]
        private string PredictedRank
        {
            set
            {
                _predictedRank = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRank)));
            }
            get => _predictedRank;
        }
        [UIValue("predictedRankDiff")]
        private string PredictedRankDiff
        {
            set
            {
                _predictedRankDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiff)));
            }
            get => _predictedRankDiff;
        }
        [UIValue("predictedRankDiffColor")]
        private string PredictedRankDiffColor
        {
            set
            {
                _predictedRankDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiffColor)));
            }
            get => _predictedRankDiffColor;
        }
        [UIValue("predictedCountryRank")]
        private string PredictedCountryRank
        {
            set
            {
                _predictedCountryRank = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRank)));
            }
            get => _predictedCountryRank;
        }
        [UIValue("predictedCountryRankDiff")]
        private string PredictedCountryRankDiff
        {
            set
            {
                _predictedCountryRankDiff = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiff)));
            }
            get => _predictedCountryRankDiff;
        }
        [UIValue("predictedCountryRankDiffColor")]
        private string PredictedCountryRankDiffColor
        {
            set
            {
                _predictedCountryRankDiffColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiffColor)));
            }
            get => _predictedCountryRankDiffColor;
        }
        #endregion

        [UIValue("isDataLoading")]
        private bool IsDataLoading
        {
            set
            {
                _isDataLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDataLoading)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNoDataLoading)));
            }
            get => _isDataLoading;
        }

        [UIValue("isNoDataLoading")]
        private bool IsNoDataLoading
        {
            get => !_isDataLoading;
        }

        private async void OnDifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            _currentSelectionBaseStars = await Plugin.PPCalculator.GetStarsForBeatmapAsync(lvlSelectionNavigationCtrl, beatmap);
            _selectedMapSearchString = lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel ? Plugin.PPCalculator.CreateSeachString(Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel), lvlSelectionNavigationCtrl.selectedDifficultyBeatmap.difficultyRank) : string.Empty;
            _currentSelectionStars = Plugin.PPCalculator.ApplyModifierMultiplierToStars(_currentSelectionBaseStars, _gameplayModifiers);
            DisplayPP();
        }

        private async void OnDetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType)
        {
            if (contentType == StandardLevelDetailViewController.ContentType.OwnedAndReady)
            {
                _currentSelectionBaseStars = await Plugin.PPCalculator.GetStarsForBeatmapAsync(lvlSelectionNavigationCtrl, lvlSelectionNavigationCtrl.selectedDifficultyBeatmap);
                _selectedMapSearchString = lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel ? Plugin.PPCalculator.CreateSeachString(Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel), lvlSelectionNavigationCtrl.selectedDifficultyBeatmap.difficultyRank) : string.Empty;
                _currentSelectionStars = Plugin.PPCalculator.ApplyModifierMultiplierToStars(_currentSelectionBaseStars, _gameplayModifiers);
                DisplayPP();
            }
        }

        private void OnLevelSelectionActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            TogglePPPView(true);
        }
        private void OnLevelSelectionDeactivated(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            TogglePPPView(false);
        }

        private void TogglePPPView(bool active)
        {
            this.floatingScreen.gameObject.SetActive(active);
        }

        private void OnScreenHandleReleased(object sender, FloatingScreenHandleEventArgs args)
        {
            Plugin.ProfileInfo.Position = floatingScreen.transform.position;
            Plugin.ProfileInfo.EulerAngles = floatingScreen.transform.eulerAngles;
        }

        #region helper functions

        private async Task UpdateCurrentAndCheckResetSession(bool doResetSession)
        {
            IsDataLoading = true;
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            PPPPlayer player = await Plugin.PPCalculator.GetProfile(userInfo.platformUserId);
            Plugin.ProfileInfo.CurrentPlayer = player;
            if (doResetSession || Plugin.ProfileInfo.SessionPlayer == null || NeedsResetSession())
            {
                Plugin.ProfileInfo.LastSessionReset = DateTime.Now;
                Plugin.ProfileInfo.SessionPlayer = player;
            }
            DisplaySession();
            IsDataLoading = false;
        }

        private bool NeedsResetSession()
        {
            return
                Plugin.ProfileInfo.ResetSessionHours > 0
                && (DateTime.Now - Plugin.ProfileInfo.LastSessionReset).TotalHours > Plugin.ProfileInfo.ResetSessionHours;
        }
        public async void RefreshCurrentData(int fetchLength)
        {
            await UpdateCurrentAndCheckResetSession(false);
            IsDataLoading = true;
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            await Plugin.PPCalculator.GetPlayerScores(userInfo.platformUserId, fetchLength);
            DisplayPP();
            IsDataLoading = false;
        }

        public async void ResetDisplay(bool resetAll)
        {
            floatingScreen.transform.eulerAngles = Plugin.ProfileInfo.EulerAngles;
            floatingScreen.transform.position = Plugin.ProfileInfo.Position;
            await UpdateCurrentAndCheckResetSession(resetAll);
            IsDataLoading = true;
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            await Plugin.PPCalculator.GetPlayerScores(userInfo.platformUserId, 100);
            DisplayInitialPercentages();
            DisplayPP();
            IsDataLoading = false;
        }
        private async void DisplayPP()
        {
            double pp = Plugin.PPCalculator.CalculatePPatPercentage(_currentSelectionStars, _percentage);
            PPGainResult ppGainResult = Plugin.PPCalculator.GetPlayerScorePPGain(_selectedMapSearchString, pp);
            double ppGains = Plugin.PPCalculator.Zeroizer(ppGainResult.PpGain);
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
                rankGain = await Plugin.PPCalculator.GetPlayerRankGain(ppGainResult.PpTotal);
                _rankGainRunning = false;
            }
            if (_lastPPGainCall > 0)
            {
                _rankGainRunning = true;
                rankGain = await Plugin.PPCalculator.GetPlayerRankGain(_lastPPGainCall);
                _rankGainRunning = false;
                _lastPPGainCall = 0;
            }
            DisplayRankGain(rankGain);

        }

        private void DisplaySession()
        {
            if (Plugin.ProfileInfo.SessionPlayer == null || Plugin.ProfileInfo.CurrentPlayer == null)
            {
                SessionRank = SessionCountryRank = SessionPP = SessionCountryRankDiff = SessionRankDiff = SessionPPDiff = "-";
                SessionCountryRankDiffColor = SessionRankDiffColor = SessionPPDiffColor = "white";
            }
            else
            {
                if (Plugin.ProfileInfo.DisplaySessionValues)
                {
                    SessionRank = $"{Plugin.ProfileInfo.SessionPlayer.Rank}";
                    SessionCountryRank = $"{Plugin.ProfileInfo.SessionPlayer.CountryRank}";
                    SessionPP = $"{Plugin.ProfileInfo.SessionPlayer.Pp:F2}pp";
                }
                else
                {
                    SessionRank = $"{Plugin.ProfileInfo.CurrentPlayer.Rank}";
                    SessionCountryRank = $"{Plugin.ProfileInfo.CurrentPlayer.CountryRank}";
                    SessionPP = $"{Plugin.ProfileInfo.CurrentPlayer.Pp:F2}pp";
                }
                SessionCountryRankDiff = (Plugin.ProfileInfo.CurrentPlayer.CountryRank - Plugin.ProfileInfo.SessionPlayer.CountryRank).ToString("+#;-#;0");
                SessionCountryRankDiffColor = DisplayHelper.GetDisplayColor((Plugin.ProfileInfo.CurrentPlayer.CountryRank - Plugin.ProfileInfo.SessionPlayer.CountryRank), true);
                SessionRankDiff = (Plugin.ProfileInfo.CurrentPlayer.Rank - Plugin.ProfileInfo.SessionPlayer.Rank).ToString("+#;-#;0");
                SessionRankDiffColor = DisplayHelper.GetDisplayColor((Plugin.ProfileInfo.CurrentPlayer.Rank - Plugin.ProfileInfo.SessionPlayer.Rank), true);
                SessionPPDiff = $"{Plugin.PPCalculator.Zeroizer(Plugin.ProfileInfo.CurrentPlayer.Pp - Plugin.ProfileInfo.SessionPlayer.Pp):+0.##;-0.##;0}pp";
                SessionPPDiffColor = DisplayHelper.GetDisplayColor((Plugin.ProfileInfo.CurrentPlayer.Pp - Plugin.ProfileInfo.SessionPlayer.Pp), false);
            }
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

        private void DisplayInitialPercentages()
        {
            SliderFineValue = Plugin.ProfileInfo.LastPercentageSelected;
            SliderCoarseValue = Plugin.ProfileInfo.LastPercentageSelected - Plugin.ProfileInfo.LastPercentageSelected % 10;
        }
        #endregion
    }
}