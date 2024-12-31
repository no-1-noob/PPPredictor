using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.DataType.Curve
{
    public interface IPPPCurve
    {
        bool IsDummy { get; }
        double CalculatePPatPercentage(PPPBeatMapInfo _currentBeatMapInfo, double percentage, bool failed, bool paused, LeaderboardContext leaderboardContext = LeaderboardContext.None);
        double CalculateMaxPP(PPPBeatMapInfo _currentBeatMapInfo, LeaderboardContext leaderboardContext = LeaderboardContext.None);
        CurveInfo ToCurveInfo();
    }
}
