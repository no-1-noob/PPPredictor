﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using PPPredictor.Utilities;
using SiraUtil.Web.SiraSync;
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Zenject;
using BS_Utils;
using BS_Utils.Gameplay;
using scoresaberapi;
using BeatSaberMarkupLanguage.Components.Settings;

namespace PPPredictor
{
    [ViewDefinition("PPPredictor.UI.Views.PPPredictorView.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\PPPredictorView.bsml")]
    internal class PPPredictorViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        LevelSelectionNavigationController levelSelectionNavController;
        private FloatingScreen floatingScreen;
        //private FloatingScreenMoverPointer

        #region displayValues
        private float _percentage;
        private string _ppDisplayRaw;
        private double _currentSelectionBasePP;

        private string _sessionRank;
        private string _sessionCountryRank;
        private string _sessionPP;

        private string _sessionCountryRankDiff;
        private string _sessionCountryRankDiffColor;
        private string _sessionRankDiff;
        private string _sessionRankDiffColor;
        private string _sessionPPDiff;
        private string _sessionPPDiffColor;
        #endregion

        #region internal values
        private string _selectedMapSearchString;
        #endregion

        public PPPredictorViewController(LevelSelectionNavigationController levelSelectionNavController)
        {
            this.levelSelectionNavController = levelSelectionNavController;
            Plugin.Log?.Info("PPPredictorViewController ctor");
        }

        private ISiraSyncService siraSyncService;

        public event PropertyChangedEventHandler PropertyChanged;

        [Inject]
        public void Construct(ISiraSyncService siraSyncService)
        {
            this.siraSyncService = siraSyncService;
            Plugin.Log?.Info("PPPredictorViewController Inject");
        }
        public async void Initialize()
        {
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            Plugin.Log?.Info(userInfo.platformUserId);
            Player player = await PPCalculator.getProfile(userInfo.platformUserId);
            if (Plugin.ProfileInfo.SessionPlayer == null)
            {
                Plugin.Log?.Info("set SessionPlayer");
                Plugin.ProfileInfo.SessionPlayer = player;
            }

            _ = PPCalculator.getPlayerScores(userInfo.platformUserId, 10);

            displaySession();

            levelSelectionNavController.didChangeDifficultyBeatmapEvent += OnDifficultyChanged;
            levelSelectionNavController.didChangeLevelDetailContentEvent += OnDetailContentChanged;
            floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(75, 100), true, Plugin.ProfileInfo.Position, new Quaternion(0, 0, 0, 0));
            floatingScreen.transform.eulerAngles = Plugin.ProfileInfo.EulerAngles;
            floatingScreen.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            floatingScreen.HandleSide = FloatingScreen.Side.Left;
            floatingScreen.HandleReleased += OnScreenHandleReleased;
            BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "PPPredictor.UI.Views.PPPredictorView.bsml"), floatingScreen.gameObject, this);
            displayInitialPercentages();
            displayPP();
        }

        public void Dispose()
        {
            floatingScreen.HandleReleased -= OnScreenHandleReleased;
            Plugin.Log?.Info("PPPredictorViewController Dispose");
        }

        #region UI Components
        [UIComponent("sliderFine")]
        private SliderSetting sliderFine;
        #endregion

        #region buttons
        [UIAction("refresh-profile-clicked")]
        private async void RefreshProfileClicked()
        {
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            Plugin.Log?.Info(userInfo.platformUserId);
            Player p = await PPCalculator.getProfile(userInfo.platformUserId);
            _ = PPCalculator.getPlayerScores(userInfo.platformUserId, 10);
            Plugin.ProfileInfo.CurrentPlayer = p;
            displaySession();
        }

        [UIAction("reset-session-clicked")]
        private async void ResetSessionClicked()
        {
            UserInfo userInfo = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            Plugin.Log?.Info(userInfo.platformUserId);
            Player p = await PPCalculator.getProfile(userInfo.platformUserId);
            Plugin.ProfileInfo.CurrentPlayer = p;
            Plugin.ProfileInfo.SessionPlayer = p;
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

        [UIValue("ppDisplayRaw")]
        private string PpDisplayRaw
        {
            set
            {
                _ppDisplayRaw = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PpDisplayRaw)));
            }
            get => _ppDisplayRaw;
        }

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

        private async void OnDifficultyChanged(LevelSelectionNavigationController _, IDifficultyBeatmap beatmap)
        {
            Plugin.Log?.Info($"DifficultyChanged: {beatmap}");
            Plugin.Log?.Info($"{beatmap.level.levelID}, {beatmap.difficultyRank}");
            _currentSelectionBasePP = await PPCalculator.calculateBasePPForBeatmapAsync(beatmap);
            _selectedMapSearchString = PPCalculator.createSeachString(_.selectedDifficultyBeatmap.level.levelID.Replace("custom_level_", ""), _.selectedDifficultyBeatmap.difficultyRank);
            //TODO: disable slider
            displayPP();
        }

        private async void OnDetailContentChanged(LevelSelectionNavigationController _, StandardLevelDetailViewController.ContentType contentType)
        {
            if(contentType == StandardLevelDetailViewController.ContentType.OwnedAndReady)
            {
                Plugin.Log?.Info($"OnDetailContentChanged: {contentType} {_.selectedDifficultyBeatmap}");
                Plugin.Log?.Info($"{_.selectedDifficultyBeatmap.level.levelID}, {_.selectedDifficultyBeatmap.difficultyRank}");
                _currentSelectionBasePP = await PPCalculator.calculateBasePPForBeatmapAsync(_.selectedDifficultyBeatmap);
                _selectedMapSearchString = PPCalculator.createSeachString(_.selectedDifficultyBeatmap.level.levelID.Replace("custom_level_", ""), _.selectedDifficultyBeatmap.difficultyRank);
                //TODO: disable slider
                displayPP();
            }
        }

        private void OnScreenHandleReleased(object sender, FloatingScreenHandleEventArgs args)
        {
            Plugin.ProfileInfo.Position = floatingScreen.transform.position;
            Plugin.ProfileInfo.EulerAngles = floatingScreen.transform.eulerAngles;
        }

        #region helper functions
        private void displayPP()
        {
            double pp = PPCalculator.calculatePPatPercentage(_currentSelectionBasePP, _percentage);
            double ppGains = PPCalculator.getPlayerScorePPGain(_selectedMapSearchString, pp);
            PpDisplayRaw = $"<b>{pp.ToString("F2")}pp</b> <i>[{(ppGains).ToString("+0.##;-0.##;0")}pp]</i>";
        }

        private void displaySession()
        {
            SessionRank = $"{Plugin.ProfileInfo.SessionPlayer.Rank.ToString()}";
            SessionCountryRank = $"{Plugin.ProfileInfo.SessionPlayer.CountryRank.ToString()}";
            SessionPP = $"{Plugin.ProfileInfo.SessionPlayer.Pp.ToString()}pp"; ;
            SessionCountryRankDiff = (Plugin.ProfileInfo.CurrentPlayer.CountryRank - Plugin.ProfileInfo.SessionPlayer.CountryRank).ToString("+#;-#;0");
            SessionCountryRankDiffColor = getDisplayColor((Plugin.ProfileInfo.CurrentPlayer.CountryRank - Plugin.ProfileInfo.SessionPlayer.CountryRank), true);
            SessionRankDiff = (Plugin.ProfileInfo.CurrentPlayer.Rank - Plugin.ProfileInfo.SessionPlayer.Rank).ToString("+#;-#;0");
            SessionRankDiffColor = getDisplayColor((Plugin.ProfileInfo.CurrentPlayer.Rank - Plugin.ProfileInfo.SessionPlayer.Rank), true);
            SessionPPDiff = $"{(Plugin.ProfileInfo.CurrentPlayer.Pp - Plugin.ProfileInfo.SessionPlayer.Pp).ToString("+0.##;-0.##;0")}pp";
            SessionPPDiffColor = getDisplayColor((Plugin.ProfileInfo.CurrentPlayer.Pp - Plugin.ProfileInfo.SessionPlayer.Pp), false);
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