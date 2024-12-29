using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.UI.ViewController
{
    [HotReload(RelativePathToLayout = @"SettingsMidViewController.bsml")]
    [ViewDefinition("PPPredictor.UI.Views.SettingsMidView.bsml")]
    class SettingsMidViewController : BSMLAutomaticViewController, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        private readonly List<object> scoringTypeOptions;
        private readonly List<object> generalPPGainCalculationOptions;
        private readonly List<object> counterDisplayTypeOptions;
        private readonly List<object> hitbloqMapPoolSortingOptions;
        private readonly List<object> menuPositionPresetOptions;

        private MenuPositionPreset _selectedMenuPositionPreset;

        public SettingsMidViewController()
        {
            scoringTypeOptions = new List<object>();
            foreach (CounterScoringType enumValue in Enum.GetValues(typeof(CounterScoringType)))
            {
                scoringTypeOptions.Add(enumValue.ToString());
            }
            generalPPGainCalculationOptions = new List<object>();
            foreach (PPGainCalculationType enumValue in Enum.GetValues(typeof(PPGainCalculationType)))
            {
                generalPPGainCalculationOptions.Add(enumValue.ToString());
            }
            counterDisplayTypeOptions = new List<object>();
            foreach (CounterDisplayType enumValue in Enum.GetValues(typeof(CounterDisplayType)))
            {
                counterDisplayTypeOptions.Add(EnumHelper.CounterDisplayTypeGetDisplayValue(enumValue));
            }
            hitbloqMapPoolSortingOptions = new List<object>();
            foreach (MapPoolSorting enumValue in Enum.GetValues(typeof(MapPoolSorting)))
            {
                hitbloqMapPoolSortingOptions.Add(enumValue.ToString());
            }
            menuPositionPresetOptions = new List<object>();
            foreach (MenuPositionPreset enumValue in Enum.GetValues(typeof(MenuPositionPreset)))
            {
                menuPositionPresetOptions.Add(enumValue.ToString());
            }
            _selectedMenuPositionPreset = Utilities.MenuPositionPreset.UnderScoreboard;
        }

        #region settings
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        [UIParams]
        private readonly BSMLParserParams bsmlParserParams;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null

#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("reset-settings")]
        private void ResetSettings()
        {
            bsmlParserParams.EmitEvent("open-reset-settings-modal");
        }
        [UIAction("reset-cache")]
        private void ResetCache()
        {
            bsmlParserParams.EmitEvent("open-reset-cache-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members

        [UIValue("version-check-enabled")]
        public bool VersionCheckEnabled
        {
            get => Plugin.ProfileInfo.IsVersionCheckEnabled;
            set
            {
                Plugin.ProfileInfo.IsVersionCheckEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VersionCheckEnabled)));
            }
        }

        [UIValue("predictor-switch-syncurl-enabled")]
        public bool PredictorSwitchSyncUrlEnabled
        {
            get => Plugin.ProfileInfo.IsPredictorSwitchBySyncUrlEnabled;
            set
            {
                Plugin.ProfileInfo.IsPredictorSwitchBySyncUrlEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PredictorSwitchSyncUrlEnabled)));
            }
        }

        [UIValue("general-pp-gain-calculation-options")]
        public List<object> GeneralPPGainCalculationOptions
        {
            get => this.generalPPGainCalculationOptions;
        }
        [UIValue("general-pp-gain-calculation")]
        public string GeneralPPGainCalculation
        {
            get => Plugin.ProfileInfo.PpGainCalculationType.ToString();
            set
            {
                Plugin.ProfileInfo.PpGainCalculationType = (PPGainCalculationType)Enum.Parse(typeof(PPGainCalculationType), value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GeneralPPGainCalculation)));
            }
        }

        [UIValue("general-raw-pp-loss-highlight-threshold")]
        public int GeneralRawPPLossHighlightThreshold
        {
            get => Plugin.ProfileInfo.RawPPLossHighlightThreshold;
            set
            {
                Plugin.ProfileInfo.RawPPLossHighlightThreshold = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GeneralRawPPLossHighlightThreshold)));
            }
        }

        [UIValue("hitbloq-mappool-sorting-options")]
        public List<object> HitbloqMapPoolSortingOptions
        {
            get => this.hitbloqMapPoolSortingOptions;
        }
        [UIValue("accsaber-enabled")]
        public bool AccSaberEnabled
        {
            get => Plugin.ProfileInfo.IsAccSaberEnabledManual && Plugin.ProfileInfo.IsScoreSaberEnabled;
            set
            {
                Plugin.ProfileInfo.IsAccSaberEnabledManual = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccSaberEnabled)));
            }
        }

        [UIValue("scoresaber-enabled")]
        public bool ScoreSaberEnabled
        {
            get => Plugin.ProfileInfo.IsScoreSaberEnabled;
        }

        [UIValue("hitbloq-mappool-sorting")]
        public string HitbloqMapPoolSorting
        {
            get => Plugin.ProfileInfo.HitbloqMapPoolSorting.ToString();
            set
            {
                Plugin.ProfileInfo.HitbloqMapPoolSorting = (MapPoolSorting)Enum.Parse(typeof(MapPoolSorting), value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HitbloqMapPoolSorting)));
            }
        }

        [UIValue("display-session-values")]
        public bool DisplaySessionValues
        {
            get => Plugin.ProfileInfo.DisplaySessionValues;
            set
            {
                Plugin.ProfileInfo.DisplaySessionValues = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplaySessionValues)));
            }
        }
        [UIValue("reset-session-hours")]
        public int ResetSessionHours
        {
            get => Plugin.ProfileInfo.ResetSessionHours;
            set
            {
                Plugin.ProfileInfo.ResetSessionHours = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResetSessionHours)));
            }
        }

        [UIValue("counter-display-type-options")]
        public List<object> CounterDisplayTypeOptions
        {
            get => this.counterDisplayTypeOptions;
        }
        [UIValue("counter-display-type")]
        public string CounterDisplayType
        {
            get => EnumHelper.CounterDisplayTypeGetDisplayValue(Plugin.ProfileInfo.CounterDisplayType);
            set
            {
                Plugin.ProfileInfo.CounterDisplayType = EnumHelper.DisplayValueToCounterDisplayType(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CounterDisplayType)));
            }
        }

        [UIValue("counter-use-icons")]
        public bool CounterUseIcons
        {
            get => Plugin.ProfileInfo.CounterUseIcons;
            set
            {
                Plugin.ProfileInfo.CounterUseIcons = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CounterUseIcons)));
            }
        }
        [UIValue("counter-use-custom-mappool-icons")]
        public bool CounterUseCustomMapPoolIcons
        {
            get => Plugin.ProfileInfo.CounterUseCustomMapPoolIcons;
            set
            {
                Plugin.ProfileInfo.CounterUseCustomMapPoolIcons = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CounterUseCustomMapPoolIcons)));
            }
        }
        [UIValue("counter-highlight-target-percentage")]
        public bool CounterHighlightTargetPercentage
        {
            get => Plugin.ProfileInfo.CounterHighlightTargetPercentage;
            set
            {
                Plugin.ProfileInfo.CounterHighlightTargetPercentage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CounterHighlightTargetPercentage)));
            }
        }
        [UIValue("counter-hide-when-unranked")]
        public bool CounterHideWhenUnranked
        {
            get => Plugin.ProfileInfo.CounterHideWhenUnranked;
            set
            {
                Plugin.ProfileInfo.CounterHideWhenUnranked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CounterHideWhenUnranked)));
            }
        }
        [UIValue("counter-scoring-type-options")]
        public List<object> CounterScoringTypeOptions
        {
            get => this.scoringTypeOptions;
        }
        [UIValue("counter-scoring-type")]
        public string CounterScoringType
        {
            get => Plugin.ProfileInfo.CounterScoringType.ToString();
            set
            {
                Plugin.ProfileInfo.CounterScoringType = (CounterScoringType)Enum.Parse(typeof(CounterScoringType), value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CounterScoringType)));
            }
        }
        [UIValue("counter-gain-silentmode")]
        public bool CounterGainSilentMode
        {
            get => Plugin.ProfileInfo.IsCounterGainSilentModeEnabled;
            set
            {
                Plugin.ProfileInfo.IsCounterGainSilentModeEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CounterGainSilentMode)));
            }
        }
        #endregion

        #region Menu Positioning
        [UIAction("apply-menu-position-preset")]
        private void ApplyMenuPositionPreset()
        {
            switch (_selectedMenuPositionPreset)
            {
                case Utilities.MenuPositionPreset.UnderScoreboard:
                    Plugin.ProfileInfo.Position = MenuPositionHelper.UnderScoreboardPosition;
                    Plugin.ProfileInfo.EulerAngles = MenuPositionHelper.UnderScoreboardEulerAngles;
                    break;
                case Utilities.MenuPositionPreset.RightOfScoreboard:
                    Plugin.ProfileInfo.Position = MenuPositionHelper.RightOfScoreboardPosition;
                    Plugin.ProfileInfo.EulerAngles = MenuPositionHelper.RightOfScoreboardEulerAngles;
                    break;
                default:
                    break;
            }
        }

        [UIValue("menu-position-preset-options")]
        public List<object> MenuPositionPresetOptions
        {
            get => this.menuPositionPresetOptions;
        }
        [UIValue("menu-position-preset")]
        public string MenuPositionPreset
        {
            get => _selectedMenuPositionPreset.ToString();
            set
            {
                _selectedMenuPositionPreset = (MenuPositionPreset)Enum.Parse(typeof(MenuPositionPreset), value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MenuPositionPreset)));
            }
        }
        #endregion

        #region stream overlay
        [UIValue("stream-overlay-port")]
        public string StreamOverlayPort
        {
            get => Plugin.ProfileInfo.StreamOverlayPort;
            set
            {
                Plugin.ProfileInfo.StreamOverlayPort = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StreamOverlayPort)));
            }
        }
        #endregion

#pragma warning disable IDE0051 // Remove unused private members
        #region modal actions
        [UIAction("confirm-reset-settings-modal")]
        private void ConfirmResetSettingsModal()
        {
            ProfileInfoMgr.ResetSettings();
            RefreshSettingsDisplay();
            bsmlParserParams.EmitEvent("close-reset-settings-modal");
        }
        [UIAction("confirm-reset-cache-modal")]
        private void ConfirmResetCacheModal()
        {
            ProfileInfoMgr.ResetCache();
            bsmlParserParams.EmitEvent("close-reset-cache-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members
        #endregion

        private void RefreshSettingsDisplay()
        {
            DisplaySessionValues = Plugin.ProfileInfo.DisplaySessionValues;
            ResetSessionHours = Plugin.ProfileInfo.ResetSessionHours;
            CounterDisplayType = EnumHelper.CounterDisplayTypeGetDisplayValue(Plugin.ProfileInfo.CounterDisplayType);
            CounterUseIcons = Plugin.ProfileInfo.CounterUseIcons;
            CounterUseCustomMapPoolIcons = Plugin.ProfileInfo.CounterUseCustomMapPoolIcons;
            CounterHighlightTargetPercentage = Plugin.ProfileInfo.CounterHighlightTargetPercentage;
            CounterHideWhenUnranked = Plugin.ProfileInfo.CounterHideWhenUnranked;
            CounterScoringType = Plugin.ProfileInfo.CounterScoringType.ToString();
            VersionCheckEnabled = Plugin.ProfileInfo.IsVersionCheckEnabled;
            PredictorSwitchSyncUrlEnabled = Plugin.ProfileInfo.IsPredictorSwitchBySyncUrlEnabled;
            HitbloqMapPoolSorting = Plugin.ProfileInfo.HitbloqMapPoolSorting.ToString();
            GeneralPPGainCalculation = Plugin.ProfileInfo.PpGainCalculationType.ToString();
            GeneralRawPPLossHighlightThreshold = Plugin.ProfileInfo.RawPPLossHighlightThreshold;
            CounterGainSilentMode = Plugin.ProfileInfo.IsCounterGainSilentModeEnabled;
            StreamOverlayPort = Plugin.ProfileInfo.StreamOverlayPort;
            AccSaberEnabled = Plugin.ProfileInfo.IsAccSaberEnabledManual && Plugin.ProfileInfo.IsScoreSaberEnabled;
        }
    }
}
