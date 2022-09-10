using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.FloatingScreen;
using PPPredictor.Utilities;
using System;
using System.ComponentModel;
using System.Reflection;
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
        private PPPredictorMgr ppPredictorMgr;
        public event PropertyChangedEventHandler PropertyChanged;

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
            ppPredictorMgr = new PPPredictorMgr();
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

        public void OnScreenHandleReleased(object sender, FloatingScreenHandleEventArgs args)
        {
            Plugin.ProfileInfo.Position = floatingScreen.transform.position;
            Plugin.ProfileInfo.EulerAngles = floatingScreen.transform.eulerAngles;
        }

        private void didChangeGameplayModifiersEvent()
        {
            this.ppPredictorMgr.ChangeGameplayModifiers(this.gameplaySetupViewController);
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
            ResetPosition();
            DisplayInitialPercentages();
            this.ppPredictorMgr.CurrentPPPredictor.ResetDisplay(false);
            this.ppPredictorMgr.SetPropertyChangedEventHandler(PropertyChanged);
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
            this.ppPredictorMgr.RefreshCurrentData(10);
        }
#pragma warning restore IDE0051 // Remove unused private members

#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("reset-session-clicked")]
        private void ResetSessionClicked()
        {
            this.ppPredictorMgr.UpdateCurrentAndCheckResetSession(true);
        }
#pragma warning restore IDE0051 // Remove unused private members
        [UIAction("cycle-leaderboard-clicked")]
        private void CycleLeaderboardClicked()
        {
            this.ppPredictorMgr.CyclePredictors();
        }
        #endregion

        [UIValue("sliderCoarseValue")]
        private float SliderCoarseValue
        {
            get => ((float)Math.Floor(ppPredictorMgr.CurrentPPPredictor.Percentage - ppPredictorMgr.CurrentPPPredictor.Percentage % 10));
            set
            {
                sliderFine.slider.minValue = value;
                sliderFine.slider.maxValue = value + 10;
                SliderFineValue = (value) + (ppPredictorMgr.CurrentPPPredictor.Percentage % 10);
                ppPredictorMgr.CurrentPPPredictor.DisplayPP();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SliderCoarseValue)));
            }
        }
        [UIValue("sliderFineValue")]
        private float SliderFineValue
        {
            get => ppPredictorMgr.CurrentPPPredictor.Percentage;
            set
            {
                ppPredictorMgr.CurrentPPPredictor.Percentage = value;
                Plugin.ProfileInfo.LastPercentageSelected = ppPredictorMgr.CurrentPPPredictor.Percentage;
                ppPredictorMgr.CurrentPPPredictor.DisplayPP();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SliderFineValue)));
            }
        }

        [UIValue("ppGainRaw")]
        private string PPGainRaw
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PPGainRaw;
        }

        [UIValue("ppGainWeighted")]
        private string PPGainWeighted
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PPGainWeighted;
        }

        [UIValue("ppGainDiffColor")]
        private string PPGainDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PPGainDiffColor;
        }

        #region UI Values session
        [UIValue("sessionRank")]
        private string SessionRank
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionRank;
        }
        [UIValue("sessionRankDiff")]
        private string SessionRankDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionRankDiff;
        }
        [UIValue("sessionRankDiffColor")]
        private string SessionRankDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionRankDiffColor;
        }

        [UIValue("sessionCountryRank")]
        private string SessionCountryRank
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionCountryRank;
        }
        [UIValue("sessionCountryRankDiff")]
        private string SessionCountryRankDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionCountryRankDiff;
        }
        [UIValue("sessionCountryRankDiffColor")]
        private string SessionCountryRankDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionCountryRankDiffColor;
        }

        [UIValue("sessionPP")]
        private string SessionPP
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionPP;
        }
        [UIValue("sessionPPDiff")]
        private string SessionPPDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionPPDiff;
        }
        [UIValue("sessionPPDiffColor")]
        private string SessionPPDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionPPDiffColor;
        }
        #endregion
        #region UI Values Predicted Rank

        [UIValue("predictedRank")]
        private string PredictedRank
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedRank;
        }
        [UIValue("predictedRankDiff")]
        private string PredictedRankDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedRankDiff;
        }
        [UIValue("predictedRankDiffColor")]
        private string PredictedRankDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedRankDiffColor;
        }
        [UIValue("predictedCountryRank")]
        private string PredictedCountryRank
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedCountryRank;
        }
        [UIValue("predictedCountryRankDiff")]
        private string PredictedCountryRankDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedCountryRankDiff;
        }
        [UIValue("predictedCountryRankDiffColor")]
        private string PredictedCountryRankDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedCountryRankDiffColor;
        }
        #endregion

        [UIValue("isDataLoading")]
        private bool IsDataLoading
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.IsDataLoading;
        }

        [UIValue("isNoDataLoading")]
        private bool IsNoDataLoading
        {
            get => !this.ppPredictorMgr.CurrentPPPredictor.IsDataLoading;
        }

        private async void OnDifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            this.ppPredictorMgr.CurrentPPPredictor.DifficultyChanged(lvlSelectionNavigationCtrl, beatmap);
        }

        private async void OnDetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType)
        {
            this.ppPredictorMgr.CurrentPPPredictor.DetailContentChanged(lvlSelectionNavigationCtrl, contentType);
        }
        internal void ResetDisplay(bool v)
        {
            ResetPosition();
            DisplayInitialPercentages();
            this.ppPredictorMgr.CurrentPPPredictor.ResetDisplay(v);
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
            floatingScreen.gameObject.SetActive(active);
        }
        internal void RefreshCurrentData(int count)
        {
            this.ppPredictorMgr.RefreshCurrentData(count);
        }

        private void DisplayInitialPercentages()
        {
            SliderFineValue = Plugin.ProfileInfo.LastPercentageSelected;
            SliderCoarseValue = Plugin.ProfileInfo.LastPercentageSelected - Plugin.ProfileInfo.LastPercentageSelected % 10;
        }

        public void ResetPosition()
        {
            floatingScreen.transform.eulerAngles = Plugin.ProfileInfo.EulerAngles;
            floatingScreen.transform.position = Plugin.ProfileInfo.Position;
        }
    }
}