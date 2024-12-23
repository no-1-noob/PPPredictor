using PPPredictor.Core.Interface;
using System;
using System.Collections.Generic;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.DataType.Curve
{
    class BeatLeaderPPPCurve : IPPPCurve
    {
        private readonly List<(double, double)> accPointList = new List<(double, double)> {
                (1.0, 7.424),
                (0.999, 6.241),
                (0.9975, 5.158),
                (0.995, 4.010),
                (0.9925, 3.241),
                (0.99, 2.700),
                (0.9875, 2.303),
                (0.985, 2.007),
                (0.9825, 1.786),
                (0.98, 1.618),
                (0.9775, 1.490),
                (0.975, 1.392),
                (0.9725, 1.315),
                (0.97, 1.256),
                (0.965, 1.167),
                (0.96, 1.094),
                (0.955, 1.039),
                (0.95, 1.000),
                (0.94, 0.931),
                (0.93, 0.867),
                (0.92, 0.813),
                (0.91, 0.768),
                (0.9, 0.729),
                (0.875, 0.650),
                (0.85, 0.581),
                (0.825, 0.522),
                (0.8, 0.473),
                (0.75, 0.404),
                (0.7, 0.345),
                (0.65, 0.296),
                (0.6, 0.256),
                (0.0, 0.000), };
        public bool IsDummy { get => false; }
        public double CalculatePPatPercentage(PPPBeatMapInfo beatMapInfo, double percentage, bool failed, bool paused, LeaderboardContext leaderboardContext = LeaderboardContext.None)
        {
            try
            {
                if (beatMapInfo.ModifiedStarRating.IsRanked())
                {
                    percentage /= 100.0;
                    if (leaderboardContext == LeaderboardContext.BeatLeaderGolf)
                    {
                        if (percentage > 0.5f) return 0;
                        percentage = 1f - percentage;
                    }
                    if (!failed && !(leaderboardContext == LeaderboardContext.BeatLeaderNoPauses && paused))
                    {
                        var (passPP, accPP, techPP) = CalculatePP(percentage, beatMapInfo.ModifiedStarRating.AccRating * beatMapInfo.ModifiedStarRating.Multiplier, beatMapInfo.ModifiedStarRating.PassRating * beatMapInfo.ModifiedStarRating.Multiplier, beatMapInfo.ModifiedStarRating.TechRating * beatMapInfo.ModifiedStarRating.Multiplier, leaderboardContext);
                        var rawPP = Inflate(passPP + accPP + techPP);
                        return rawPP;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"BeatLeaderPPPCurve CalculatePPatPercentage Error: {ex.Message}");
                return -1;
            }
        }

        private (double, double, double) CalculatePP(double accuracy, double accRating, double passRating, double techRating, LeaderboardContext leaderboardContext = LeaderboardContext.None)
        {
            double passPP = 15.2f * Math.Exp(Math.Pow(passRating, 1 / 2.62f)) - 30f;
            if (double.IsInfinity(passPP) || double.IsNaN(passPP) || double.IsNegativeInfinity(passPP) || passPP < 0)
            {
                passPP = 0;
            }
            double accPP = leaderboardContext == LeaderboardContext.BeatLeaderGolf ? accuracy * accRating * 42f : AccCurve(accuracy) * accRating * 34f;
            double techPP = Math.Exp(1.9f * accuracy) * 1.08f * techRating;

            return (passPP, accPP, techPP);
        }

        private double AccCurve(double acc)
        {
            int i = 0;
            for (; i < accPointList.Count; i++)
            {
                if (accPointList[i].Item1 <= acc)
                {
                    break;
                }
            }

            if (i == 0)
            {
                i = 1;
            }

            double middle_dis = (acc - accPointList[i - 1].Item1) / (accPointList[i].Item1 - accPointList[i - 1].Item1);
            return (accPointList[i - 1].Item2 + middle_dis * (accPointList[i].Item2 - accPointList[i - 1].Item2));
        }

        private double Inflate(double pp)
        {
            return (650f * Math.Pow(pp, 1.3f)) / Math.Pow(650f, 1.3f);
        }

        public double CalculateMaxPP(PPPBeatMapInfo beatMapInfo, LeaderboardContext leaderboardContext = LeaderboardContext.None)
        {
            return CalculatePPatPercentage(beatMapInfo, 100, false, false, leaderboardContext);
        }

        public CurveInfo ToCurveInfo()
        {
            CurveInfo retCurve = new CurveInfo
            {
                CurveType = CurveType.BeatLeader
            };
            return retCurve;
        }
    }
}
