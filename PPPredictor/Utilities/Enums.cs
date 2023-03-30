namespace PPPredictor.Utilities
{
    enum BeatLeaderDifficultyStatus
    {
        unranked = 0,
        nominated = 1,
        qualified = 2,
        ranked = 3,
        unrankable = 4,
        outdated = 5,
        inevent = 6
    }

    enum PPPBeatMapDifficulty
    {
        Easy = 1,
        Normal = 3,
        Hard = 5,
        Expert = 7,
        ExpertPlus = 9
    }

    enum Leaderboard
    {
        ScoreSaber,
        BeatLeader,
        AccSaber,
        NoLeaderboard,
        HitBloq
    }

    enum CounterScoringType
    {
        Global,
        Local
    }

    enum MapPoolType
    {
        Default,
        Custom
    }

    enum CurveType
    {
        ScoreSaber,
        BeatLeader,
        Linear,
        Basic
    }
    enum PPGainCalculationType
    {
        Weighted,
        Raw
    }

    enum MapPoolSorting
    {
        Alphabetical,
        Popularity
    }

    enum MenuPositionPreset
    {
        UnderScoreboard,
        RightOfScoreboard
    }

    enum CounterDisplayType
    {
        PP,
        PPNoSuffix,
        PPAndGain,
        PPAndGainNoSuffix,
        PPAndGainNoBrackets,
        PPAndGainNoBracketsNoSuffix,
        GainNoBrackets,
        GainNoBracketsNoSuffix
    }

    class EnumHelper
    {
        private const string CounterDisplayTypePP = "PP: 100pp";
        private const string CounterDisplayTypePPAndGain = "PP & Gain: 100pp [<color=green>10pp</color>]";
        private const string CounterDisplayTypePPAndGainNoBrackets = "PP & Gain: 100pp <color=green>10pp</color>";
        private const string CounterDisplayTypeGainNoBrackets = "Gain: <color=green>10pp</color>";
        private const string CounterDisplayTypePPNoSuffix = "PP: 100";
        private const string CounterDisplayTypePPAndGainNoSuffix = "PP & Gain: 100 [<color=green>10</color>]";
        private const string CounterDisplayTypePPAndGainNoBracketsNoSuffix = "PP & Gain: 100 <color=green>10</color>";
        private const string CounterDisplayTypeGainNoBracketsNoSuffix = "Gain: <color=green>10</color>";
        public static string CounterDisplayTypeGetDisplayValue(CounterDisplayType displayType)
        {
            switch (displayType)
            {
                case CounterDisplayType.PP:
                    return CounterDisplayTypePP;
                case CounterDisplayType.PPAndGain:
                    return CounterDisplayTypePPAndGain;
                case CounterDisplayType.PPAndGainNoBrackets:
                    return CounterDisplayTypePPAndGainNoBrackets;
                case CounterDisplayType.GainNoBrackets:
                    return CounterDisplayTypeGainNoBrackets;
                case CounterDisplayType.PPNoSuffix:
                    return CounterDisplayTypePPNoSuffix;
                case CounterDisplayType.PPAndGainNoSuffix:
                    return CounterDisplayTypePPAndGainNoSuffix;
                case CounterDisplayType.PPAndGainNoBracketsNoSuffix:
                    return CounterDisplayTypePPAndGainNoBracketsNoSuffix;
                case CounterDisplayType.GainNoBracketsNoSuffix:
                    return CounterDisplayTypeGainNoBracketsNoSuffix;
                default:
                    return "EnumNotFound";
            }
        }

        public static CounterDisplayType DisplayValueToCounterDisplayType(string displayValue)
        {
            switch (displayValue)
            {
                case CounterDisplayTypePP:
                    return CounterDisplayType.PP;
                case CounterDisplayTypePPAndGain:
                    return CounterDisplayType.PPAndGain;
                case CounterDisplayTypePPAndGainNoBrackets:
                    return CounterDisplayType.PPAndGainNoBrackets;
                case CounterDisplayTypeGainNoBrackets:
                    return CounterDisplayType.GainNoBrackets;
                case CounterDisplayTypePPNoSuffix:
                    return CounterDisplayType.PPNoSuffix;
                case CounterDisplayTypePPAndGainNoSuffix:
                    return CounterDisplayType.PPAndGainNoSuffix;
                case CounterDisplayTypePPAndGainNoBracketsNoSuffix:
                    return CounterDisplayType.PPAndGainNoBracketsNoSuffix;
                case CounterDisplayTypeGainNoBracketsNoSuffix:
                    return CounterDisplayType.GainNoBracketsNoSuffix;
                default:
                    return CounterDisplayType.PP;
            }
        }
    }
}
