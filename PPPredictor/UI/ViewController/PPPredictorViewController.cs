using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using PPPredictor.Utilities;
using SiraUtil.Web.SiraSync;
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Zenject;
using scoresaberapi;
using BeatSaberMarkupLanguage.Components.Settings;

namespace PPPredictor.UI.ViewController
{
    [ViewDefinition("PPPredictor.UI.Views.PPPredictorView.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\PPPredictorView.bsml")]
    public class PPPredictorViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        LevelSelectionNavigationController levelSelectionNavController;
        private FloatingScreen floatingScreen;
        //private FloatingScreenMoverPointer

        #region displayValues
        private float _percentage;
        private string _ppGainRaw = "";
        private string _ppGainWeighted = "";
        private string _ppGainDiffColor = "white";

        private double _currentSelectionBasePP;

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
        private double lastPPGainCall = 0;

        #endregion

        #region internal values
        private string _selectedMapSearchString;
        public bool _isDataLoading = false;
        #endregion

        public PPPredictorViewController(LevelSelectionNavigationController levelSelectionNavController)
        {
            this.levelSelectionNavController = levelSelectionNavController;
        }

        private ISiraSyncService siraSyncService;

        public event PropertyChangedEventHandler PropertyChanged;

        [Inject]
        public void Construct(ISiraSyncService siraSyncService)
        {
            this.siraSyncService = siraSyncService;
        }
        public async void Initialize()
        {
            Plugin.pppViewController = this;
            levelSelectionNavController.didChangeDifficultyBeatmapEvent += OnDifficultyChanged;
            levelSelectionNavController.didChangeLevelDetailContentEvent += OnDetailContentChanged;
            levelSelectionNavController.didActivateEvent += OnLevelSelectionActivated;
            levelSelectionNavController.didDeactivateEvent += OnLevelSelectionDeactivated;
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

        public void Dispose()
        {
            floatingScreen.HandleReleased -= OnScreenHandleReleased;
            levelSelectionNavController.didActivateEvent -= OnLevelSelectionActivated;
            levelSelectionNavController.didDeactivateEvent -= OnLevelSelectionDeactivated;
            Plugin.pppViewController = null;
        }

        [UIAction("#post-parse")]
        protected async void PostParse()
        {
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            IsDataLoading = true;
            Player player = await PPCalculator.getProfile(userInfo.platformUserId);
            Plugin.ProfileInfo.CurrentPlayer = player;
            if (Plugin.ProfileInfo.SessionPlayer == null)
            {
                Plugin.ProfileInfo.SessionPlayer = player;
            }
            displaySession();
            await PPCalculator.getPlayerScores(userInfo.platformUserId, 100);
            displayInitialPercentages();
            displayPP();
            IsDataLoading = false;
        }

        #region UI Components
        [UIComponent("sliderFine")]
        private SliderSetting sliderFine;
        #endregion

        #region buttons
        [UIAction("refresh-profile-clicked")]
        private async void RefreshProfileClicked()
        {
            refreshCurrentData(10);
        }

        [UIAction("reset-session-clicked")]
        private async void ResetSessionClicked()
        {
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            IsDataLoading = true;
            Player player = await PPCalculator.getProfile(userInfo.platformUserId);
            Plugin.ProfileInfo.CurrentPlayer = player;
            Plugin.ProfileInfo.SessionPlayer = player;
            IsDataLoading = false;
            displaySession();

        }
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
                displayPP();
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
                displayPP();
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

        private async void OnDifficultyChanged(LevelSelectionNavigationController _, IDifficultyBeatmap beatmap)
        {
            _currentSelectionBasePP = await PPCalculator.calculateBasePPForBeatmapAsync(beatmap);
            _selectedMapSearchString = PPCalculator.createSeachString(_.selectedDifficultyBeatmap.level.levelID.Replace("custom_level_", ""), _.selectedDifficultyBeatmap.difficultyRank);
            //TODO: disable slider
            displayPP();
        }

        private async void OnDetailContentChanged(LevelSelectionNavigationController _, StandardLevelDetailViewController.ContentType contentType)
        {
            if(contentType == StandardLevelDetailViewController.ContentType.OwnedAndReady)
            {
                _currentSelectionBasePP = await PPCalculator.calculateBasePPForBeatmapAsync(_.selectedDifficultyBeatmap);
                _selectedMapSearchString = PPCalculator.createSeachString(_.selectedDifficultyBeatmap.level.levelID.Replace("custom_level_", ""), _.selectedDifficultyBeatmap.difficultyRank);
                displayPP();
            }
        }

        private void OnLevelSelectionActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            togglePPPView(true);
        }
        private void OnLevelSelectionDeactivated(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            togglePPPView(false);
        }

        private void togglePPPView(bool active)
        {
            this.floatingScreen.gameObject.SetActive(active);
        }

        private void OnScreenHandleReleased(object sender, FloatingScreenHandleEventArgs args)
        {
            Plugin.ProfileInfo.Position = floatingScreen.transform.position;
            Plugin.ProfileInfo.EulerAngles = floatingScreen.transform.eulerAngles;
        }

        #region helper functions
        public async void refreshCurrentData(int fetchLength)
        {
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            IsDataLoading = true;
            Player player = await PPCalculator.getProfile(userInfo.platformUserId);
            Plugin.ProfileInfo.CurrentPlayer = player;
            if (Plugin.ProfileInfo.SessionPlayer == null)
            {
                Plugin.ProfileInfo.SessionPlayer = player;
            }
            await PPCalculator.getPlayerScores(userInfo.platformUserId, fetchLength);
            displaySession();
            displayPP();
            IsDataLoading = false;
        }
        public void resetDisplay()
        {
            floatingScreen.transform.eulerAngles = Plugin.ProfileInfo.EulerAngles;
            floatingScreen.transform.position = Plugin.ProfileInfo.Position;
            this.displayPP();
            this.displaySession();
        }
        private async void displayPP()
        {
            double pp = PPCalculator.calculatePPatPercentage(_currentSelectionBasePP, _percentage);
            PPGainResult ppGainResult = PPCalculator.getPlayerScorePPGain(_selectedMapSearchString, pp);
            double ppGains = PPCalculator.zeroizer(ppGainResult.PpGain);
            PPGainRaw = $"{pp.ToString("F2")}pp";
            PPGainWeighted = $"{ppGains.ToString("+0.##;-0.##;0")}pp";
            PPGainDiffColor = getDisplayColor(ppGains, false);

            RankGainResult rankGain = new RankGainResult(1, 2, 3, 4);
            displayRankGain(null);
            if (_rankGainRunning)
            {
                lastPPGainCall = ppGainResult.PpTotal;
                return;
            }
                
            if (lastPPGainCall == 0)
            {
                _rankGainRunning = true;
                rankGain = await PPCalculator.getPlayerRankGain(ppGainResult.PpTotal);
                _rankGainRunning = false;
            }
            if(lastPPGainCall > 0)
            {
                _rankGainRunning = true;
                rankGain = await PPCalculator.getPlayerRankGain(lastPPGainCall);
                _rankGainRunning = false;
                lastPPGainCall = 0;
            }
            displayRankGain(rankGain);

        }

        private void displaySession()
        {
            if(Plugin.ProfileInfo.SessionPlayer == null || Plugin.ProfileInfo.CurrentPlayer == null){
                SessionRank = SessionCountryRank = SessionPP = SessionCountryRankDiff = SessionRankDiff = SessionPPDiff = "-";
                SessionCountryRankDiffColor = SessionRankDiffColor = SessionPPDiffColor = "white";
            }
            else
            {
                SessionRank = $"{Plugin.ProfileInfo.SessionPlayer.Rank.ToString()}";
                SessionCountryRank = $"{Plugin.ProfileInfo.SessionPlayer.CountryRank.ToString()}";
                SessionPP = $"{Plugin.ProfileInfo.SessionPlayer.Pp.ToString()}pp"; ;
                SessionCountryRankDiff = (Plugin.ProfileInfo.CurrentPlayer.CountryRank - Plugin.ProfileInfo.SessionPlayer.CountryRank).ToString("+#;-#;0");
                SessionCountryRankDiffColor = getDisplayColor((Plugin.ProfileInfo.CurrentPlayer.CountryRank - Plugin.ProfileInfo.SessionPlayer.CountryRank), true);
                SessionRankDiff = (Plugin.ProfileInfo.CurrentPlayer.Rank - Plugin.ProfileInfo.SessionPlayer.Rank).ToString("+#;-#;0");
                SessionRankDiffColor = getDisplayColor((Plugin.ProfileInfo.CurrentPlayer.Rank - Plugin.ProfileInfo.SessionPlayer.Rank), true);
                SessionPPDiff = $"{(PPCalculator.zeroizer(Plugin.ProfileInfo.CurrentPlayer.Pp - Plugin.ProfileInfo.SessionPlayer.Pp)).ToString("+0.##;-0.##;0")}pp";
                SessionPPDiffColor = getDisplayColor((Plugin.ProfileInfo.CurrentPlayer.Pp - Plugin.ProfileInfo.SessionPlayer.Pp), false);
            }
        }

        private void displayRankGain(RankGainResult rankGainResult)
        {
            if (rankGainResult != null)
            {
                PredictedRank = $"{rankGainResult.RankGlobal.ToString("N0")}";
                PredictedRankDiff = rankGainResult.RankGainGlobal.ToString("+#;-#;0");
                PredictedRankDiffColor = getDisplayColor(rankGainResult.RankGainGlobal, false);
                PredictedCountryRank = $"{rankGainResult.RankCountry.ToString("N0")}";
                PredictedCountryRankDiff = rankGainResult.RankGainCountry.ToString("+#;-#;0");
                PredictedCountryRankDiffColor = getDisplayColor(rankGainResult.RankGainCountry, false);
            }
            else
            {
                PredictedRank = "...";
                PredictedRankDiff = "?";
                PredictedRankDiffColor = getDisplayColor(0, false);
                PredictedCountryRank = "...";
                PredictedCountryRankDiff = "?";
                PredictedCountryRankDiffColor = getDisplayColor(0, false);
            }
        }

        private string getDisplayColor(double value, bool invert)
        {
            if (invert) value = value * -1;
            if(value > 0)
            {
                return $"green";
            }
            else if (value < 0)
            {
                return $"red";
            }
            return "white";
        }

        private void displayInitialPercentages()
        {
            SliderFineValue = Plugin.ProfileInfo.LastPercentageSelected;
            SliderCoarseValue = Plugin.ProfileInfo.LastPercentageSelected - Plugin.ProfileInfo.LastPercentageSelected % 10;
        }
        #endregion
    }
}