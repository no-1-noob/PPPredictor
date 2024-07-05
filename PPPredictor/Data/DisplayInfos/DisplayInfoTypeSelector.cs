using BeatSaberMarkupLanguage.Attributes;
using PPPredictor.Utilities;

namespace PPPredictor.Data.DisplayInfos
{
    internal class DisplayInfoTypeSelector
    {
        internal MultiViewType multiViewType = MultiViewType.PP;

        [UIValue("displayInfoTypeSelector_showGlobal")]
        internal bool ShowGlobal
        {
            get
            {
                return multiViewType == MultiViewType.Global;
            }
        }
        [UIValue("displayInfoTypeSelector_showLocal")]
        internal bool ShowLocal
        {
            get
            {
                return multiViewType == MultiViewType.Local;
            }
        }
        [UIValue("displayInfoTypeSelector_showPP")]
        internal bool ShowPP
        {
            get
            {
                return multiViewType == MultiViewType.PP;
            }
        }
        [UIValue("countryRankFontColor")]
        internal string CountryRankFontColor { get; set; } = DisplayHelper.ColorWhite;

        [UIValue("displayInfoTypeSelector_LeaderboardIcon")]
        internal string LeaderboardIcon { get; set; } = string.Empty;
        [UIValue("displayInfoTypeSelector_LeaderboardName")]
        internal string LeaderboardName { get; set; } = string.Empty;
    }
}
