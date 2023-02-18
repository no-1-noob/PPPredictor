﻿using PPPredictor.Interfaces;
using PPPredictor.OpenAPIs;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;

namespace PPPredictor.Data.Curve
{
    class CustomPPPCurve : IPPPCurve
    {
        //TODO: Make arrPPCurve a list?
        private readonly double[,] arrPPCurve;
        private readonly CurveType curveType;
        private readonly double basePPMultiplier;
        private readonly bool _isDummy;
        public bool isDummy { get => _isDummy; }

        public CustomPPPCurve(double[,] arrPPCurve, CurveType curveType, double basePPMultiplier, bool isDummy = false)
        {
            this.arrPPCurve = arrPPCurve;
            this.curveType = curveType;
            this.basePPMultiplier = basePPMultiplier;
            _isDummy = isDummy;
        }

        public CustomPPPCurve(CrCurve crCurve)
        {
            //TODO:  Hitbloq base_curve
            arrPPCurve = new double[crCurve.points.Count, 2];
            switch (crCurve.type)
            {
                case "linear":
                    this.curveType = CurveType.Linear;
                    break;
                default:
                    break;
            }
            for (int i = 0; i < crCurve.points.Count; i++)
            {
                //Reversed insert. Should be 100% first
                arrPPCurve[crCurve.points.Count - 1 - i, 0] = crCurve.points[i][0];
                arrPPCurve[crCurve.points.Count - 1 - i, 1] = crCurve.points[i][1];
            }
            basePPMultiplier = 50;
        }

        public double CalculatePPatPercentage(double star, double percentage, bool failed)
        {
            switch (curveType)
            {
                case CurveType.Linear:
                    return LinearCalculatePPatPercentage(star, percentage);
                case CurveType.ScoreSaber:
                    return LinearCalculatePPatPercentage(star, failed ? percentage / 2.0f : percentage);
                default:
                    return 0;
            }
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
                for (int i = 0; i < arrPPCurve.GetLength(0); i++)
                {
                    if (arrPPCurve[i, 0] == percentage)
                    {
                        return arrPPCurve[i, 1];
                    }
                    else
                    {
                        if (arrPPCurve[i + 1, 0] < percentage)
                        {
                            return CalculateMultiplierAtPercentageWithLine((arrPPCurve[i + 1, 0], arrPPCurve[i + 1, 1]), (arrPPCurve[i, 0], arrPPCurve[i, 1]), percentage);
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

        public CurveInfo ToCurveInfo()
        {
            if(curveType == CurveType.ScoreSaber)
            {
                return new CurveInfo(CurveType.ScoreSaber);
            }
            return new CurveInfo(CurveType.Linear, this.arrPPCurve, this.basePPMultiplier);
        }

        public static CustomPPPCurve DummyPPPCurve()
        {
            return new CustomPPPCurve(new double[0, 2] { }, CurveType.Linear, 0, true);
        }

        public override string ToString()
        {
            return $"CustomPPPCurve curveType:{curveType.ToString()} - basePPMultiplier: {basePPMultiplier} - dummy? {isDummy}";
        }
    }
}