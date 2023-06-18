using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Zenject;
using HarmonyLib;
using System.Threading.Tasks;

namespace PPPredictor.UI.ViewController
{
    [ViewDefinition("PPPredictor.UI.Views.PPPredictorView.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\PPPredictorView.bsml")]
    class PPPredictorViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private static readonly string githubUrl = "https://github.com/no-1-noob/PPPredictor/releases/latest";
        private FloatingScreen floatingScreen;
#pragma warning disable CS0649
        [Inject] private readonly PPPredictorMgr ppPredictorMgr;
#pragma warning restore CS0649
        public event PropertyChangedEventHandler PropertyChanged;
        private DisplaySessionInfo displaySessionInfo;
        private DisplayPPInfo displayPPInfo;
        private bool _isDataLoading;
        private bool _isScreenMoving = false;

        public PPPredictorViewController() {}

        [Inject]
        public void Construct()
        {
        }
        public void Initialize()
        {
            Plugin.pppViewController = this;
            displaySessionInfo = new DisplaySessionInfo();
            displayPPInfo= new DisplayPPInfo();
            floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(75, 100), true, Plugin.ProfileInfo.Position, new Quaternion(0, 0, 0, 0));
            floatingScreen.gameObject.name = "BSMLFloatingScreen_PPPredictor";
            floatingScreen.gameObject.SetActive(false);
            floatingScreen.transform.eulerAngles = Plugin.ProfileInfo.EulerAngles;
            floatingScreen.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            floatingScreen.handle.transform.localScale = new Vector2(60, 70);
            floatingScreen.handle.transform.localPosition = new Vector3(0, 0, .1f);
            floatingScreen.handle.transform.localRotation = Quaternion.identity;
            floatingScreen.handle.hideFlags = HideFlags.HideInHierarchy;
            floatingScreen.ShowHandle = _isScreenMoving;
            MeshRenderer floatingScreenMeshRenderer = floatingScreen.handle.GetComponent<MeshRenderer>();
            if(floatingScreenMeshRenderer) floatingScreenMeshRenderer.enabled = false; //Make it invisible ;)

            floatingScreen.HandleReleased += OnScreenHandleReleased;
            BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "PPPredictor.UI.Views.PPPredictorView.bsml"), floatingScreen.gameObject, this);
            ppPredictorMgr.ViewActivated += PpPredictorMgr_ViewActivated;
            ppPredictorMgr.OnDataLoading += PpPredictorMgr_OnDataLoading;
            ppPredictorMgr.OnDisplayPPInfo += PpPredictorMgr_OnDisplayPPInfo;
            ppPredictorMgr.OnDisplaySessionInfo += PpPredictorMgr_OnDisplaySessionInfo;
            ppPredictorMgr.OnMapPoolRefreshed += PpPredictorMgr_OnMapPoolRefreshed;
        }

        private void PpPredictorMgr_OnMapPoolRefreshed(object sender, EventArgs e)
        {
            UpdateAllDisplay();
        }

        private void PpPredictorMgr_OnDisplaySessionInfo(object sender, DisplaySessionInfo displaySessionInfo)
        {
            this.displaySessionInfo = displaySessionInfo;
            UpdateSessionDisplay();
        }

        private void PpPredictorMgr_OnDisplayPPInfo(object sender, DisplayPPInfo displayPPInfo)
        {
            this.displayPPInfo = displayPPInfo;
            UpdatePPDisplay();
        }

        private void PpPredictorMgr_OnDataLoading(object sender, bool isDataLoading)
        {
            this._isDataLoading = isDataLoading;
            UpdateLoadingDisplay();
        }

        internal void ApplySettings()
        {
            this.ppPredictorMgr.ResetPredictors();
            ResetDisplay(false);
        }

        public void OnScreenHandleReleased(object sender, FloatingScreenHandleEventArgs args)
        {
            Plugin.ProfileInfo.Position = floatingScreen.transform.position;
            Plugin.ProfileInfo.EulerAngles = floatingScreen.transform.eulerAngles;
        }

        public void Dispose()
        {
            floatingScreen.HandleReleased -= OnScreenHandleReleased;
            ppPredictorMgr.ViewActivated -= PpPredictorMgr_ViewActivated;
            if (tabSelector) tabSelector.textSegmentedControl.didSelectCellEvent -= OnSelectedCellEventChanged;
            Plugin.pppViewController = null;
        }
#pragma warning disable CS0649
        [UIParams]
        private readonly BSMLParserParams bsmlParserParams;
#pragma warning restore CS0649

        [UIAction("#post-parse")]
        protected void PostParse()
        {
            ResetPosition();
            DisplayInitialPercentages();
            this.ppPredictorMgr.ResetDisplay(false);
            CheckVersion();
            if (tabSelector)
            {
                tabSelector.textSegmentedControl.didSelectCellEvent += OnSelectedCellEventChanged;
            }
        }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        #region UI Components
        [UIComponent("sliderFine")]
        private readonly SliderSetting sliderFine;
        [UIComponent("tabSelector")]
        private readonly TabSelector tabSelector;
        [UIComponent("dropdown-map-pools")]
        private readonly DropDownListSetting dropDownMapPools;
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
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("arrow-next-leaderboard-clicked")]
        private void ArrowNextLeaderboardClicked()
        {
            this.ppPredictorMgr.CyclePredictors(1);
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
                ppPredictorMgr.CurrentPPPredictor.CalculatePP();
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
                ppPredictorMgr.CurrentPPPredictor.CalculatePP();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SliderFineValue)));
            }
        }
        [UIValue("ppRaw")]
        private string PPRaw
        {
            get => displayPPInfo.PPRaw;
        }
        [UIValue("ppGain")]
        private string PPGain
        {
            get => displayPPInfo.PPGain;
        }
        [UIValue("ppGainDiffColor")]
        private string PPGainDiffColor
        {
            get => displayPPInfo.PPGainDiffColor;
        }
        #region UI Values session
        [UIValue("sessionRank")]
        private string SessionRank
        {
            get => displaySessionInfo.SessionRank;
        }
        [UIValue("sessionRankDiff")]
        private string SessionRankDiff
        {
            get => displaySessionInfo.SessionRankDiff;
        }
        [UIValue("sessionRankDiffColor")]
        private string SessionRankDiffColor
        {
            get => displaySessionInfo.SessionRankDiffColor;
        }
        [UIValue("sessionCountryRank")]
        private string SessionCountryRank
        {
            get => displaySessionInfo.SessionCountryRank;
        }
        [UIValue("sessionCountryRankDiff")]
        private string SessionCountryRankDiff
        {
            get => displaySessionInfo.SessionCountryRankDiff;
        }
        [UIValue("sessionCountryRankDiffColor")]
        private string SessionCountryRankDiffColor
        {
            get => displaySessionInfo.SessionCountryRankDiffColor;
        }
        [UIValue("countryRankFontColor")]
        private string CountryRankFontColor
        {
            get => displaySessionInfo.CountryRankFontColor;
        }
        [UIValue("sessionPP")]
        private string SessionPP
        {
            get => displaySessionInfo.SessionPP;
        }
        [UIValue("sessionPPDiff")]
        private string SessionPPDiff
        {
            get => displaySessionInfo.SessionPPDiff;
        }
        [UIValue("sessionPPDiffColor")]
        private string SessionPPDiffColor
        {
            get => displaySessionInfo.SessionPPDiffColor;
        }
        #endregion
        #region UI Values Predicted Rank
        [UIValue("predictedRank")]
        private string PredictedRank
        {
            get => displayPPInfo.PredictedRank;
        }
        [UIValue("predictedRankDiff")]
        private string PredictedRankDiff
        {
            get => displayPPInfo.PredictedRankDiff;
        }
        [UIValue("predictedRankDiffColor")]
        private string PredictedRankDiffColor
        {
            get => displayPPInfo.PredictedRankDiffColor;
        }
        [UIValue("predictedCountryRank")]
        private string PredictedCountryRank
        {
            get => displayPPInfo.PredictedCountryRank;
        }
        [UIValue("predictedCountryRankDiff")]
        private string PredictedCountryRankDiff
        {
            get => displayPPInfo.PredictedCountryRankDiff;
        }
        [UIValue("predictedCountryRankDiffColor")]
        private string PredictedCountryRankDiffColor
        {
            get => displayPPInfo.PredictedCountryRankDiffColor;
        }
        #endregion
        [UIValue("isDataLoading")]
        private bool IsDataLoading
        {
            get => _isDataLoading;
        }
        [UIValue("isNoDataLoading")]
        private bool IsNoDataLoading
        {
            get => !(_isDataLoading || _isScreenMoving);
        }
        [UIValue("leaderBoardName")]
        internal string LeaderBoardName
        {
            get
            {
                dropDownMapPools?.UpdateChoices(); //Refresh map pools here...
                return this.ppPredictorMgr.CurrentPPPredictor.LeaderBoardName;
            }
        }
        [UIValue("leaderBoardIcon")]
        internal string LeaderBoardIcon
        {
            get { return this.ppPredictorMgr.CurrentPPPredictor.LeaderBoardIcon; }
        }
        [UIValue("isLeftArrowActive")]
        private bool IsLeftArrowActive
        {
            get => this.ppPredictorMgr.IsLeftArrowActive && !_isScreenMoving;
        }
        [UIValue("isRightArrowActive")]
        private bool IsRightArrowActive
        {
            get => this.ppPredictorMgr.IsRightArrowActive && !_isScreenMoving;
        }
        [UIValue("isMapPoolDropDownActive")]
        private bool IsMapPoolDropDownActive
        {
            get => this.ppPredictorMgr.IsMapPoolDropDownActive && !_isScreenMoving;
        }
        [UIValue("isLeaderboardNavigationActive")]
        private bool IsLeaderboardNavigationActive
        {
            get => this.ppPredictorMgr.IsLeaderboardNavigationActive;
        }
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
                return this.ppPredictorMgr.CurrentPPPredictor.MapPoolOptions;
            }
            set 
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapPoolOptions)));
            }
        }
        [UIValue("current-map-pool")]
        public object CurrentMapPool
        {
            get => this.ppPredictorMgr.CurrentPPPredictor.CurrentMapPool;
            set
            {
                this.ppPredictorMgr.CurrentPPPredictor.CurrentMapPool = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMapPool)));
            }
        }
        #endregion

        #region moving panel
        [UIAction("move-panel-clicked")]
        private void MovePanelClicked()
        {
            _isScreenMoving = !_isScreenMoving;
            floatingScreen.ShowHandle = _isScreenMoving;
            UpdateLoadingDisplay();
            UpdateLeaderBoardDisplay();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MovePanelIcon)));
        }
        [UIValue("movePanelIcon")]
        private string MovePanelIcon
        {
            get => _isScreenMoving ? "🔓" : "🔒";
        }
        #endregion
        internal void ResetDisplay(bool v)
        {
            ResetPosition();
            DisplayInitialPercentages();
            this.ppPredictorMgr.ResetDisplay(v);
        }
        internal void RefreshCurrentData(int count, bool refreshStars = false)
        {
            this.ppPredictorMgr.RefreshCurrentData(count, refreshStars);
        }

        private void PpPredictorMgr_ViewActivated(object sender, bool active)
        {
            RefreshTabSelection();
            floatingScreen.gameObject.SetActive(active);
        }

        private async void RefreshTabSelection()
        {
            await Task.Delay(100);
            if(tabSelector != null)
            {
                tabSelector.textSegmentedControl.SelectCellWithNumber(Plugin.ProfileInfo.SelectedTab);
                AccessTools.Method(typeof(TabSelector), "TabSelected").Invoke(tabSelector, new object[] { tabSelector.textSegmentedControl, Plugin.ProfileInfo.SelectedTab });
            }
        }

        private void OnSelectedCellEventChanged(SegmentedControl seg, int index)
        {
            Plugin.ProfileInfo.SelectedTab = index;
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

        private async void CheckVersion()
        {
            if(Plugin.ProfileInfo.IsVersionCheckEnabled && (DateTime.Now - Plugin.ProfileInfo.DtLastVersionCheck).TotalHours >= 12)
            {
                CurrentVersion = typeof(Plugin).Assembly.GetName().Version.ToString();
                CurrentVersion = $"{CurrentVersion.Substring(0, CurrentVersion.Length - 2)}{Plugin.Beta}";
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
            dropDownMapPools.values = this.ppPredictorMgr.CurrentPPPredictor.MapPoolOptions;
            dropDownMapPools.Value = this.ppPredictorMgr.CurrentPPPredictor.CurrentMapPool;
            dropDownMapPools.ApplyValue();
            dropDownMapPools?.UpdateChoices(); //Refresh map pools here...
        }

        private void UpdateSessionDisplay()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRank)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionRankDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRank)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionCountryRankDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CountryRankFontColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPP)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPPDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SessionPPDiffColor)));
        }

        private void UpdatePPDisplay()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPRaw)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGain)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PPGainDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRank)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedRankDiffColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRank)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiff)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictedCountryRankDiffColor)));
        }

        private void UpdateLeaderBoardDisplay()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LeaderBoardName)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LeaderBoardIcon)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapPoolOptions)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMapPool)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLeftArrowActive)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRightArrowActive)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLeaderboardNavigationActive)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMapPoolDropDownActive)));
        }

        private void UpdateLoadingDisplay()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDataLoading)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNoDataLoading)));
        }

        private void UpdateAllDisplay()
        {
            UpdateMapPoolChoices();
            UpdateLeaderBoardDisplay();
            UpdateSessionDisplay();
            UpdatePPDisplay();
            
        }
    }
}