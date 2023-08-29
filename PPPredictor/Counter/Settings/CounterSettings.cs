using BeatSaberMarkupLanguage.Attributes;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PPPredictor.Counter.Settings
{
    internal class CounterSettings : INotifyPropertyChanged
    {
        private readonly List<object> scoringTypeOptions;
        private readonly List<object> counterDisplayTypeOptions;
        public event PropertyChangedEventHandler PropertyChanged;

        public CounterSettings()
        {
            scoringTypeOptions = new List<object>();
            foreach (CounterScoringType enumValue in Enum.GetValues(typeof(CounterScoringType))) {
                scoringTypeOptions.Add(enumValue.ToString());
            }
            counterDisplayTypeOptions = new List<object>();
            foreach (CounterDisplayType enumValue in Enum.GetValues(typeof(CounterDisplayType)))
            {
                counterDisplayTypeOptions.Add(EnumHelper.CounterDisplayTypeGetDisplayValue(enumValue));
            }
        }

        [UIAction("#post-parse")]
        internal void PostParse()
        {
            // Code to run after BSML finishes
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
    }
}
