using BeatSaberMarkupLanguage.Attributes;
using PPPredictor.Utilities;

namespace PPPredictor.Data.DisplayInfos
{
    class DisplaySessionInfo : DisplayInfoTypeSelector
    {
        [UIValue("displayPPInfo_SessionRank")]
        internal string SessionRank { get; set; } = "-";
        [UIValue("displayPPInfo_SessionRankDiff")]
        internal string SessionRankDiff { get; set; } = "-";
        [UIValue("displayPPInfo_SessionRankDiffColor")]
        internal string SessionRankDiffColor { get; set; } = DisplayHelper.ColorWhite;
        [UIValue("displayPPInfo_SessionCountryRank")]
        internal string SessionCountryRank { get; set; } = "-";
        [UIValue("displayPPInfo_SessionCountryRankDiff")]
        internal string SessionCountryRankDiff { get; set; } = "-";
        [UIValue("displayPPInfo_SessionCountryRankDiffColor")]
        internal string SessionCountryRankDiffColor { get; set; } = DisplayHelper.ColorWhite;
        [UIValue("displayPPInfo_SessionPP")]
        internal string SessionPP { get; set; } = "-";
        [UIValue("displayPPInfo_SessionPPDiff")]
        internal string SessionPPDiff { get; set; } = "-";
        [UIValue("displayPPInfo_SessionPPDiffColor")]
        internal string SessionPPDiffColor { get; set; } = DisplayHelper.ColorWhite;
    }
}
