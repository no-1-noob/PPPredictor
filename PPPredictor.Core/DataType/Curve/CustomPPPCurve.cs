using PPPredictor.Core.Interface;
using PPPredictor.Core.DataType;
using System;
using System.Linq;
using System.Collections.Generic;
using static PPPredictor.Core.DataType.LeaderBoard.HitBloqDataTypes;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.DataType.Curve
{
    class CustomPPPCurve : IPPPCurve
    {
        private readonly List<(double, double)> arrPPCurve;
        private readonly CurveType curveType;
        private readonly double basePPMultiplier;
        private readonly double starOffest = 0;
        private readonly double? baseline;
        private readonly double? exponential;
        private readonly double? cutoff;
        private readonly bool _isDummy;
        public bool IsDummy { get => _isDummy; }

        public CustomPPPCurve(List<(double, double)> arrPPCurve, CurveType curveType, double basePPMultiplier, bool isDummy = false, double starOffest = 0)
        {
            this.arrPPCurve = arrPPCurve;
            this.curveType = curveType;
            this.basePPMultiplier = basePPMultiplier;
            _isDummy = isDummy;
            this.starOffest = starOffest;
        }

        private CustomPPPCurve(List<(double, double)> arrPPCurve, double basePPMultiplier, double? baseline, double? exponential, double? cutoff, bool isDummy = false) : this(arrPPCurve, CurveType.Basic, basePPMultiplier, isDummy)
        {
            this.exponential = exponential;
            this.baseline = baseline;
            this.cutoff = cutoff;
        }

        public CustomPPPCurve(HitBloqCrCurve crCurve)
        {
            switch (crCurve.type?.ToLower())
            {
                case "linear":
                    arrPPCurve = new List<(double, double)>();
                    for (int i = 0; i < crCurve.points.Count; i++)
                    {
                        arrPPCurve.Add((crCurve.points[i][0], crCurve.points[i][1]));

                    }
                    arrPPCurve.Reverse();
                    this.curveType = CurveType.Linear;
                    break;
                case "basic":
                    this.curveType = CurveType.Basic;
                    this.baseline = crCurve.baseline;
                    this.cutoff = crCurve.cutoff;
                    this.exponential = crCurve.exponential;
                    break;
            }
            basePPMultiplier = 50;
        }

        public double CalculatePPatPercentage(PPPBeatMapInfo beatMapInfo, double percentage, bool failed, bool paused, LeaderboardContext leaderboardContext = LeaderboardContext.None)
        {
            switch (curveType)
            {
                case CurveType.Linear:
                    return LinearCalculatePPatPercentage(beatMapInfo, percentage);
                case CurveType.Basic:
                    return BasicCurveCalculatePPatPercentage(beatMapInfo, percentage);
                case CurveType.ScoreSaber:
                    return LinearCalculatePPatPercentage(beatMapInfo, failed ? percentage / 2.0f : percentage);
                case CurveType.AccSaber:
                    return LinearCalculatePPatPercentage(beatMapInfo, failed ? percentage / 2.0f : percentage);
                default:
                    return 0;
            }
        }

        public double CalculateMaxPP(PPPBeatMapInfo beatMapInfo, LeaderboardContext leaderboardContext = LeaderboardContext.None)
        {
            double percent = 100;
            if(curveType == CurveType.Linear && arrPPCurve.Count > 1)
            {
                (double, double) peakMultiplier = arrPPCurve.Aggregate((i1, i2) => i1.Item2 > i2.Item2 ? i1 : i2);
                percent = peakMultiplier.Item1 * 100;
            }
            return CalculatePPatPercentage(beatMapInfo, percent, false, false);
        }

        private double LinearCalculatePPatPercentage(PPPBeatMapInfo beatMapInfo, double percentage)
        {
            try
            {
                if (!beatMapInfo.ModifiedStarRating.IsRanked()) return 0; //Needed because of starOffset in AccSaber
                percentage /= 100.0;
                double multiplier = CalculateMultiplierAtPercentage(percentage);
                return multiplier * (beatMapInfo.ModifiedStarRating.Stars + starOffest) * basePPMultiplier;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"CustomPPPCurve linearCalculatePPatPercentage Error: {ex.Message}");
                return -1;
            }
        }

        private double CalculateMultiplierAtPercentage(double percentage)
        {
            try
            {
                for (int i = 0; i < arrPPCurve.Count; i++)
                {
                    if (arrPPCurve[i].Item1 == percentage)
                    {
                        return arrPPCurve[i].Item2;
                    }
                    else
                    {
                        if (arrPPCurve[i + 1].Item1 < percentage)
                        {
                            return CalculateMultiplierAtPercentageWithLine((arrPPCurve[i + 1].Item1, arrPPCurve[i + 1].Item2), (arrPPCurve[i].Item1, arrPPCurve[i].Item2), percentage);
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"CustomPPPCurve CalculateMultiplierAtPercentage Error: {ex.Message}");
                return -1;
            }
        }

        private double CalculateMultiplierAtPercentageWithLine((double x, double y) p1, (double x, double y) p2, double percentage)
        {
            try
            {
                double m = (p2.y - p1.y) / (p2.x - p1.x);
                double b = p1.y - (m * p1.x);
                return m * percentage + b;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"CustomPPPCurve CalculateMultiplierAtPercentageWithLine Error: {ex.Message}");
                return -1;
            }
        }

        private double BasicCurveCalculatePPatPercentage(PPPBeatMapInfo beatMapInfo, double percentage)
        {
            double baseline = (this.baseline ?? 0.78) * 100f;
            double cutoff = this.cutoff ?? 0.5f;
            double exponential = this.exponential ?? 2.5f;

            double multiplier;
            if(percentage < baseline)
            {
                multiplier = (percentage / 100f) * cutoff;
            }
            else
            {
                multiplier = (percentage / 100f) * cutoff + (1 - cutoff) * Math.Pow((percentage - baseline) / (100 - baseline), exponential);
            }
            return beatMapInfo.ModifiedStarRating.Stars * basePPMultiplier * multiplier;
        }

        public CurveInfo ToCurveInfo()
        {
            switch (curveType)
            {
                case CurveType.ScoreSaber:
                    return new CurveInfo(CurveType.ScoreSaber);
                case CurveType.Basic:
                    return new CurveInfo(CurveType.Basic, this.arrPPCurve, this.basePPMultiplier, this.baseline, this.exponential, this.cutoff);
                case CurveType.AccSaber:
                    return new CurveInfo(CurveType.AccSaber);
                default:
                    return new CurveInfo(CurveType.Linear, this.arrPPCurve, this.basePPMultiplier);
            }            
        }

        public static CustomPPPCurve CreateDummyPPPCurve()
        {
            return new CustomPPPCurve(new List<(double, double)>(), CurveType.Dummy, 0, true);
        }

        public static CustomPPPCurve CreateBasicPPPCurve(double basePPMultiplier, double? baseline, double? exponential, double? cutoff, bool isDummy = false)
        {
            return new CustomPPPCurve(new List<(double, double)>(), basePPMultiplier, baseline, exponential, cutoff, isDummy);
        }

        public override string ToString()
        {
            return $"CustomPPPCurve curveType:{curveType} - basePPMultiplier: {basePPMultiplier} - dummy? {IsDummy}";
        }
    }
}
