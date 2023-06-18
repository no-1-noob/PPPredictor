using PPPredictor.Utilities;

namespace PPPredictor.Data.DisplayInfos
{
    class DisplaySessionInfo
    {
        internal string SessionRank { get; set; } = "-";
        internal string SessionRankDiff { get; set; } = "-";
        internal string SessionRankDiffColor { get; set; } = DisplayHelper.ColorWhite;
        internal string SessionCountryRank { get; set; } = "-";
        internal string SessionCountryRankDiff { get; set; } = "-";
        internal string SessionCountryRankDiffColor { get; set; } = DisplayHelper.ColorWhite;
        internal string CountryRankFontColor { get; set; } = DisplayHelper.ColorWhite;
        internal string SessionPP { get; set; } = "-";
        internal string SessionPPDiff { get; set; } = "-";
        internal string SessionPPDiffColor { get; set; } = DisplayHelper.ColorWhite;
    }
}
