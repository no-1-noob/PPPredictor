using BeatSaberMarkupLanguage.Attributes;
using PPPredictor.Utilities;

namespace PPPredictor.Data.DisplayInfos
{
    internal class DisplayPPInfo : DisplayInfoTypeSelector
    {
        [UIValue("displayPPInfo_PPRaw")]
        internal string PPRaw { get; set; } = string.Empty;
        [UIValue("displayPPInfo_PPGain")]
        internal string PPGain { get; set; } = string.Empty;
        [UIValue("displayPPInfo_PPGainDiffColor")]
        internal string PPGainDiffColor { get; set; } = string.Empty;
        [UIValue("displayPPInfo_PredictedRank")]
        internal string PredictedRank { get; set; } = string.Empty;
        [UIValue("displayPPInfo_PredictedRankDiff")]
        internal string PredictedRankDiff { get; set; } = string.Empty;
        [UIValue("displayPPInfo_PredictedRankDiffColor")]
        internal string PredictedRankDiffColor { get; set; } = string.Empty;
        [UIValue("displayPPInfo_PredictedCountryRank")]
        internal string PredictedCountryRank { get; set; } = string.Empty;
        [UIValue("displayPPInfo_PredictedCountryRankDiff")]
        internal string PredictedCountryRankDiff { get; set; } = string.Empty;
        [UIValue("displayPPInfo_PredictedCountryRankDiffColor")]
        internal string PredictedCountryRankDiffColor { get; set; } = string.Empty;
    }
}
