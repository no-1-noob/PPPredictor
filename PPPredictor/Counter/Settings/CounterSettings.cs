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
        public event PropertyChangedEventHandler PropertyChanged;

        public CounterSettings()
        {
            scoringTypeOptions = new List<object>();
            foreach (CounterScoringType enumValue in Enum.GetValues(typeof(CounterScoringType))) {
                scoringTypeOptions.Add(enumValue.ToString());
            }
        }

        [UIAction("#post-parse")]
        internal void PostParse()
        {
            // Code to run after BSML finishes
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
    }
}
