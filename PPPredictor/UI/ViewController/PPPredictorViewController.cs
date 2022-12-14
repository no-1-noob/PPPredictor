using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.Parser;
using PPPredictor.Data;
using PPPredictor.Data.Curve;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace PPPredictor.UI.ViewController
{
    [ViewDefinition("PPPredictor.UI.Views.PPPredictorView.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\PPPredictorView.bsml")]
    public class PPPredictorViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private static readonly string githubUrl = "https://github.com/no-1-noob/PPPredictor/releases/latest";
        private readonly LevelSelectionNavigationController levelSelectionNavController;
        private readonly GameplaySetupViewController gameplaySetupViewController;
        private FloatingScreen floatingScreen;
        internal PPPredictorMgr ppPredictorMgr;
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
            gameplaySetupViewController.didChangeGameplayModifiersEvent += DidChangeGameplayModifiersEvent;
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

        internal void ApplySettings()
        {
            this.ppPredictorMgr.ResetPredictors();
            ResetDisplay(true);
        }

        public void OnScreenHandleReleased(object sender, FloatingScreenHandleEventArgs args)
        {
            Plugin.ProfileInfo.Position = floatingScreen.transform.position;
            Plugin.ProfileInfo.EulerAngles = floatingScreen.transform.eulerAngles;
        }

        private void DidChangeGameplayModifiersEvent()
        {
            this.ppPredictorMgr.ChangeGameplayModifiers(this.gameplaySetupViewController);
        }

        public void Dispose()
        {
            floatingScreen.HandleReleased -= OnScreenHandleReleased;
            levelSelectionNavController.didActivateEvent -= OnLevelSelectionActivated;
            levelSelectionNavController.didDeactivateEvent -= OnLevelSelectionDeactivated;
            gameplaySetupViewController.didChangeGameplayModifiersEvent -= DidChangeGameplayModifiersEvent;
            Plugin.pppViewController = null;
        }

        [UIParams]
        private readonly BSMLParserParams bsmlParserParams;

        [UIAction("#post-parse")]
        protected void PostParse()
        {
            ResetPosition();
            DisplayInitialPercentages();
            this.ppPredictorMgr.ResetDisplay(false);
            this.ppPredictorMgr.SetPropertyChangedEventHandler(PropertyChanged);
            CheckVersion();
        }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        #region UI Components
        [UIComponent("sliderFine")]
        private readonly SliderSetting sliderFine;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null
        [UIComponent("dropdown-map-pools")]
        private readonly DropDownListSetting dropDownMapPools;
        #endregion


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
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("show-update-clicked")]
        private void ShowUpdateClicked()
        {
            bsmlParserParams.EmitEvent("show-update-display-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("click-close-update-modal")]
        private void CloseUpdateClicked()
        {
            bsmlParserParams.EmitEvent("close-update-display-modal");
            if (HideUpdate)
            {
                Plugin.ProfileInfo.AcknowledgedVersion = NewVersion;
                NewVersion = string.Empty;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNewVersionAvailable)));
            }
            bsmlParserParams.EmitEvent("close-update-display-modal");
            bsmlParserParams.EmitEvent("open-menu-settings-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("click-close-github")]
        private void CloseGithubClicked()
        {
            bsmlParserParams.EmitEvent("close-github-notification");
            bsmlParserParams.EmitEvent("show-update-display-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("click-open-github")]
        private void OpenGithubClicked()
        {
            Process.Start(githubUrl);
            bsmlParserParams.EmitEvent("close-update-display-modal");
            bsmlParserParams.EmitEvent("show-github-notification");
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("arrow-prev-leaderboard-clicked")]
        private void ArrowPrevLeaderboardClicked()
        {
            this.ppPredictorMgr.CyclePredictors(-1);
            UpdateMapPoolChoices();
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("arrow-next-leaderboard-clicked")]
        private void ArrowNextLeaderboardClicked()
        {
            this.ppPredictorMgr.CyclePredictors(1);
            UpdateMapPoolChoices();
        }
#pragma warning restore IDE0051 // Remove unused private members
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
                ppPredictorMgr.SetPercentage(value);
                Plugin.ProfileInfo.LastPercentageSelected = ppPredictorMgr.CurrentPPPredictor.Percentage;
                ppPredictorMgr.CurrentPPPredictor.DisplayPP();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SliderFineValue)));
            }
        }
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("ppRaw")]
        private string PPRaw
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PPRaw;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("ppGain")]
        private string PPGain
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PPGain;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("ppGainDiffColor")]
        private string PPGainDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PPGainDiffColor;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        #region UI Values session
        [UIValue("sessionRank")]
        private string SessionRank
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionRank;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("sessionRankDiff")]
        private string SessionRankDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionRankDiff;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("sessionRankDiffColor")]
        private string SessionRankDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionRankDiffColor;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("sessionCountryRank")]
        private string SessionCountryRank
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionCountryRank;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("sessionCountryRankDiff")]
        private string SessionCountryRankDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionCountryRankDiff;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("sessionCountryRankDiffColor")]
        private string SessionCountryRankDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionCountryRankDiffColor;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("sessionPP")]
        private string SessionPP
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionPP;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("sessionPPDiff")]
        private string SessionPPDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionPPDiff;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("sessionPPDiffColor")]
        private string SessionPPDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.SessionPPDiffColor;
        }
#pragma warning restore IDE0051 // Remove unused private members
        #endregion
        #region UI Values Predicted Rank
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("predictedRank")]
        private string PredictedRank
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedRank;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("predictedRankDiff")]
        private string PredictedRankDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedRankDiff;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("predictedRankDiffColor")]
        private string PredictedRankDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedRankDiffColor;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("predictedCountryRank")]
        private string PredictedCountryRank
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedCountryRank;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("predictedCountryRankDiff")]
        private string PredictedCountryRankDiff
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedCountryRankDiff;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("predictedCountryRankDiffColor")]
        private string PredictedCountryRankDiffColor
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.PredictedCountryRankDiffColor;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        #endregion
        [UIValue("isDataLoading")]
        private bool IsDataLoading
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.IsDataLoading;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("isNoDataLoading")]
        private bool IsNoDataLoading
        {
            get => !this.ppPredictorMgr.CurrentPPPredictor.IsDataLoading;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("leaderBoardName")]
        internal string LeaderBoardName
        {
            get
            {
                Plugin.Log?.Error($"Get LeaderBoardName View Controller DropDownSettings? '{dropDownMapPools}'");
                dropDownMapPools?.UpdateChoices(); //Refresh map pools here...
                return this.ppPredictorMgr.CurrentPPPredictor.LeaderBoardName;
            }
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("isNoUserFound")]
        private bool IsNoUserFound
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.IsNoUserFound;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("isUserFound")]
        private bool IsUserFound
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.IsUserFound;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("isLeftArrowActive")]
        private bool IsLeftArrowActive
        {
            get => this.ppPredictorMgr.IsLeftArrowActive;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("isRightArrowActive")]
        private bool IsRightArrowActive
        {
            get => this.ppPredictorMgr.IsRightArrowActive;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("isMapPoolDropDownActive")]
        private bool IsMapPoolDropDownActive
        {
            get => this.ppPredictorMgr.IsMapPoolDropDownActive;
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIValue("isLeaderboardNavigationActive")]
        private bool IsLeaderboardNavigationActive
        {
            get => this.ppPredictorMgr.IsLeaderboardNavigationActive;
        }
#pragma warning restore IDE0051 // Remove unused private members
        #region update UI data
        [UIValue("newVersion")]
        private string NewVersion { get; set; }
        [UIValue("currentVersion")]
        private string CurrentVersion { get; set; }
        [UIValue("hide-update")]
        private bool HideUpdate { get; set; }
        [UIValue("isNewVersionAvailable")]
        private bool IsNewVersionAvailable { get => !string.IsNullOrEmpty(NewVersion); }
        #endregion

        #region MapPoolUI Stuff
        [UIValue("map-pool-options")]
        public List<object> MapPoolOptions
        {
            get 
            {
                Plugin.Log?.Error($"Get map-pool-options");
                return this.ppPredictorMgr.CurrentPPPredictor.MapPoolOptions;
            }
            set {
                Plugin.Log?.Error($"Set map-pool-options");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapPoolOptions)));
            }
        }
        [UIValue("current-map-pool")]
        public object CurrentMapPool
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.CurrentMapPool;
            set
            {
                Plugin.Log?.Error($"Set MapPool {value}");
                bool isCurrentMapPoolChanging = IsCurrentMapPoolChanging(value);
                this.ppPredictorMgr.CurrentPPPredictor.CurrentMapPool = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMapPool)));
                if(isCurrentMapPoolChanging)
                {
                    Plugin.Log?.Error($"Set MapPool Refresh {value}");
                    this.RefreshCurrentData(10);
                }
            }
        }
        #endregion
        private void OnDifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            this.ppPredictorMgr.DifficultyChanged(lvlSelectionNavigationCtrl, beatmap);
        }

        private void OnDetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType)
        {
            this.ppPredictorMgr.DetailContentChanged(lvlSelectionNavigationCtrl, contentType);
        }
        internal void ResetDisplay(bool v)
        {
            ResetPosition();
            DisplayInitialPercentages();
            this.ppPredictorMgr.ResetDisplay(v);
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

        internal bool IsCurrentMapPoolChanging(object value)
        {
            var currentPool = this.ppPredictorMgr.CurrentPPPredictor.CurrentMapPool as PPPMapPool;
            var newMapPool = value as PPPMapPool;
            return (currentPool != null && newMapPool != null && currentPool.Id != newMapPool.Id);
        }

        private void DisplayInitialPercentages()
        {
            SliderFineValue = Plugin.ProfileInfo.LastPercentageSelected;
            SliderCoarseValue = Plugin.ProfileInfo.LastPercentageSelected - Plugin.ProfileInfo.LastPercentageSelected % 10;
        }

        public void ResetPosition()
        {
            floatingScreen.ShowHandle = Plugin.ProfileInfo.WindowHandleEnabled;
            floatingScreen.transform.eulerAngles = Plugin.ProfileInfo.EulerAngles;
            floatingScreen.transform.position = Plugin.ProfileInfo.Position;
        }

        private async void CheckVersion()
        {
            if(Plugin.ProfileInfo.IsVersionCheckEnabled && (DateTime.Now - Plugin.ProfileInfo.DtLastVersionCheck).TotalHours >= 12)
            {
                CurrentVersion = typeof(Plugin).Assembly.GetName().Version.ToString();
                CurrentVersion = CurrentVersion.Substring(0, CurrentVersion.Length - 2);
                string acknowledgedVersion = Plugin.ProfileInfo.AcknowledgedVersion;
                NewVersion = await VersionChecker.VersionChecker.GetCurrentVersionAsync();
                if (string.IsNullOrEmpty(NewVersion) || NewVersion == CurrentVersion || NewVersion == acknowledgedVersion)
                {
                    NewVersion = string.Empty;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNewVersionAvailable)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentVersion)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewVersion)));
                Plugin.ProfileInfo.DtLastVersionCheck = DateTime.Now;
            }
        }

        private void UpdateMapPoolChoices()
        {
            Plugin.Log?.Error($"UpdateMapPoolChoices? '{dropDownMapPools}'");
            dropDownMapPools.values = this.ppPredictorMgr.CurrentPPPredictor.MapPoolOptions;
            dropDownMapPools.Value = this.ppPredictorMgr.CurrentPPPredictor.CurrentMapPool;
            dropDownMapPools.ApplyValue();
            dropDownMapPools?.UpdateChoices(); //Refresh map pools here...
        }
    }
}