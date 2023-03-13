using PPPredictor.Interfaces;
using PPPredictor.OpenAPIs;
using PPPredictor.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PPPredictor.Data.Curve
{
    class CustomPPPCurve : IPPPCurve
    {
        private readonly List<(double, double)> arrPPCurve;
        private readonly CurveType curveType;
        private readonly double basePPMultiplier;
        private readonly double? baseline;
        private readonly double? exponential;
        private readonly double? cutoff;
        private readonly bool _isDummy;
        public bool isDummy { get => _isDummy; }

        public CustomPPPCurve(List<(double, double)> arrPPCurve, CurveType curveType, double basePPMultiplier, bool isDummy = false)
        {
            this.arrPPCurve = arrPPCurve;
            this.curveType = curveType;
            this.basePPMultiplier = basePPMultiplier;
            _isDummy = isDummy;
        }

        public CustomPPPCurve(List<(double, double)> arrPPCurve, CurveType curveType, double basePPMultiplier, double? baseline, double? exponential, double? cutoff, bool isDummy = false) : this(arrPPCurve, curveType, basePPMultiplier, isDummy)
        {
            this.exponential = exponential;
            this.baseline = baseline;
            this.cutoff = cutoff;
        }

        public CustomPPPCurve(CrCurve crCurve)
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
                default:
                    break;
            }
            basePPMultiplier = 50;
        }

        public double CalculatePPatPercentage(double star, double percentage, bool failed)
        {
            switch (curveType)
            {
                case CurveType.Linear:
                    return LinearCalculatePPatPercentage(star, percentage);
                case CurveType.Basic:
                    return BasicCurveCalculatePPatPercentage(star, percentage);
                case CurveType.ScoreSaber:
                    return LinearCalculatePPatPercentage(star, failed ? percentage / 2.0f : percentage);
                default:
                    return 0;
            }
        }

        public double CalculateMaxPP(double star)
        {
            double percent = 100;
            if(curveType == CurveType.Linear && arrPPCurve.Count > 1)
            {
                (double, double) peakMultiplier = arrPPCurve.Aggregate((i1, i2) => i1.Item2 > i2.Item2 ? i1 : i2);
                percent = peakMultiplier.Item1 * 100;
            }
            return CalculatePPatPercentage(star, percent, false);
        }

        private double LinearCalculatePPatPercentage(double star, double percentage)
        {
            try
            {
                percentage /= 100.0;
                double multiplier = CalculateMultiplierAtPercentage(percentage);
                return multiplier * star * basePPMultiplier;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"CustomPPPCurve linearCalculatePPatPercentage Error: {ex.Message}");
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
                Plugin.Log?.Error($"CustomPPPCurve CalculateMultiplierAtPercentage Error: {ex.Message}");
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
                Plugin.Log?.Error($"CustomPPPCurve CalculateMultiplierAtPercentageWithLine Error: {ex.Message}");
                return -1;
            }
        }

        private double BasicCurveCalculatePPatPercentage(double star, double percentage)
        {
            double baseline = (this.baseline.HasValue ? this.baseline.Value : 0.78) * 100f;
            double cutoff = this.cutoff.HasValue ? this.cutoff.Value : 0.5f;
            double exponential = this.exponential.HasValue ? this.exponential.Value : 2.5;

            double multiplier;
            if(percentage < baseline)
            {
                multiplier = (percentage / 100f) * cutoff;
            }
            else
            {
                multiplier = (percentage / 100f) * cutoff + (1 - cutoff) * Math.Pow((percentage - baseline) / (100 - baseline), exponential);
            }
            return star * basePPMultiplier * multiplier;
        }

        public CurveInfo ToCurveInfo()
        {
            switch (curveType)
            {
                case CurveType.ScoreSaber:
                    return new CurveInfo(CurveType.ScoreSaber);
                case CurveType.Basic:
                    return new CurveInfo(CurveType.Basic, this.arrPPCurve, this.basePPMultiplier, this.baseline, this.exponential, this.cutoff);
                default:
                    return new CurveInfo(CurveType.Linear, this.arrPPCurve, this.basePPMultiplier);
            }            
        }

        public static CustomPPPCurve DummyPPPCurve()
        {
            return new CustomPPPCurve(new List<(double, double)>(), CurveType.Linear, 0, true);
        }

        public override string ToString()
        {
            return $"CustomPPPCurve curveType:{curveType.ToString()} - basePPMultiplier: {basePPMultiplier} - dummy? {isDummy}";
        }
    }
}
