using PPPredictor.Interfaces;
using System;

namespace PPPredictor.Data.Curve
{
    class BeatLeaderPPPCurve : IPPPCurve
    {
        private readonly double ppCalcWeight = 42;
        public bool IsDummy { get => false; }
        public double CalculatePPatPercentage(double star, double percentage, bool failed)
        {
            try
            {
                if (star <= 0) return 0;
                var l = 1.0 - (0.03 * ((star - 0.5) - 3) / 11);
                var n = percentage / 100.0;
                n = Math.Min(n, l - 0.001);
                var a = 0.96 * l;
                var f = 1.2 - (0.6 * ((star - 0.5) / 14));
                return (star + 0.5) * ppCalcWeight * Math.Pow((Math.Log(l / (l - n)) / (Math.Log(l / (l - a)))), f);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"BeatLeaderPPPCurve CalculatePPatPercentage Error: {ex.Message}");
                return -1;
            }
        }

        public double CalculateMaxPP(double star)
        {
            return CalculatePPatPercentage(star, 100, false);
        }

        public CurveInfo ToCurveInfo()
        {
            CurveInfo retCurve = new CurveInfo
            {
                CurveType = Utilities.CurveType.BeatLeader
            };
            return retCurve;
        }
    }
}
