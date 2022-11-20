using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PPPredictor.UI.ViewController
{
    [HotReload(RelativePathToLayout = @"SettingsMidViewController.bsml")]
    [ViewDefinition("PPPredictor.UI.Views.SettingsMidView.bsml")]
    class SettingsMidViewController : BSMLAutomaticViewController, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly List<object> scoringTypeOptions;

        public SettingsMidViewController()
        {
            scoringTypeOptions = new List<object>();
            foreach (CounterScoringType enumValue in Enum.GetValues(typeof(CounterScoringType)))
            {
                scoringTypeOptions.Add(enumValue.ToString());
            }
        }

        #region settings
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        [UIParams]
        private readonly BSMLParserParams bsmlParserParams;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null

#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("reset-data")]
        private void ResetData()
        {
            bsmlParserParams.EmitEvent("open-reset-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members

        [UIValue("window-handle-enabled")]
        public bool WindowHandleEnabled
        {
            get => Plugin.ProfileInfo.WindowHandleEnabled;
            set
            {
                Plugin.ProfileInfo.WindowHandleEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowHandleEnabled)));
            }
        }

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

        [UIValue("scoresaber-enabled")]
        public bool ScoreSaberEnabled
        {
            get => Plugin.ProfileInfo.IsScoreSaberEnabled;
            set
            {
                Plugin.ProfileInfo.IsScoreSaberEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScoreSaberEnabled)));
            }
        }
        [UIValue("beatleader-enabled")]
        public bool BeatLeaderEnabled
        {
            get => Plugin.ProfileInfo.IsBeatLeaderEnabled;
            set
            {
                Plugin.ProfileInfo.IsBeatLeaderEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BeatLeaderEnabled)));
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
        [UIValue("counter-show-gain")]
        public bool CounterShowGain
        {
            get => Plugin.ProfileInfo.CounterShowGain;
            set
            {
                Plugin.ProfileInfo.CounterShowGain = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CounterShowGain)));
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
        #endregion

#pragma warning disable IDE0051 // Remove unused private members
        #region modal actions
        [UIAction("confirm-reset-modal")]
        private void ConfirmResetModal()
        {
            ProfileInfoMgr.ResetProfile();
            RefreshSettingsDisplay();
            bsmlParserParams.EmitEvent("close-reset-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members
        #endregion

        private void RefreshSettingsDisplay()
        {
            WindowHandleEnabled = Plugin.ProfileInfo.WindowHandleEnabled;
            DisplaySessionValues = Plugin.ProfileInfo.DisplaySessionValues;
            ResetSessionHours = Plugin.ProfileInfo.ResetSessionHours;
            CounterShowGain = Plugin.ProfileInfo.CounterShowGain;
            CounterUseIcons = Plugin.ProfileInfo.CounterUseIcons;
            CounterHighlightTargetPercentage = Plugin.ProfileInfo.CounterHighlightTargetPercentage;
            CounterHideWhenUnranked = Plugin.ProfileInfo.CounterHideWhenUnranked;
            CounterScoringType = Plugin.ProfileInfo.CounterScoringType.ToString();
            VersionCheckEnabled = Plugin.ProfileInfo.IsVersionCheckEnabled;
            ScoreSaberEnabled = Plugin.ProfileInfo.IsScoreSaberEnabled;
            BeatLeaderEnabled = Plugin.ProfileInfo.IsBeatLeaderEnabled;
        }
    }
}
