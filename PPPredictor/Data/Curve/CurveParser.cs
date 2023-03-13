using PPPredictor.Interfaces;
using System.Collections.Generic;

namespace PPPredictor.Data.Curve
{
    class CurveParser
    {
        private static readonly double basePPMultiplierScoreSaber = 42.117208413;
        private static readonly List<(double, double)> arrPPCurveScoreSaber = new List<(double, double)>(new (double, double)[32] {
            (1, 7),
            (0.999, 6.24),
            (0.9975, 5.31),
            (0.995, 4.14),
            (0.9925, 3.31),
            (0.99, 2.73),
            (0.9875, 2.31),
            (0.985, 2.0),
            (0.9825, 1.775),
            (0.98, 1.625),
            (0.9775, 1.515),
            (0.975, 1.43),
            (0.9725, 1.36),
            (0.97, 1.3),
            (0.965, 1.195),
            (0.96, 1.115),
            (0.955, 1.05),
            (0.95, 1),
            (0.94, 0.94),
            (0.93, 0.885),
            (0.92, 0.835),
            (0.91, 0.79),
            (0.9, 0.75),
            (0.875, 0.655),
            (0.85, 0.57),
            (0.825, 0.51),
            (0.8, 0.47),
            (0.75, 0.40),
            (0.7, 0.34),
            (0.65, 0.29),
            (0.6, 0.25),
            (0.0, 0.0) });
        public static IPPPCurve ParseToCurve(CurveInfo curveInfo)
        {
            switch (curveInfo.CurveType)
            {
                case Utilities.CurveType.ScoreSaber:
                    return new CustomPPPCurve(arrPPCurveScoreSaber, Utilities.CurveType.ScoreSaber, basePPMultiplierScoreSaber);
                case Utilities.CurveType.BeatLeader:
                    return new BeatLeaderPPPCurve();
                case Utilities.CurveType.Linear:
                    return new CustomPPPCurve(curveInfo.ArrPPCurve, Utilities.CurveType.Linear, curveInfo.BasePPMultiplier.Value);
                case Utilities.CurveType.Basic:
                    return new CustomPPPCurve(curveInfo.ArrPPCurve, Utilities.CurveType.Basic, curveInfo.BasePPMultiplier.Value, curveInfo.Baseline, curveInfo.Exponential, curveInfo.Cutoff);
                default:
                    return CustomPPPCurve.DummyPPPCurve();
            }
        }
    }
}
