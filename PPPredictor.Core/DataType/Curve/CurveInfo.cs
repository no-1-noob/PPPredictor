using System.Collections.Generic;
using System.ComponentModel;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.DataType.Curve
{
    public class CurveInfo
    {
        private CurveType _curveType;
        private List<(double, double)> _arrPPCurve;
        private double? basePPMultiplier;
        private double? baseline;
        private double? exponential;
        private double? cutoff;

        public List<(double, double)> ArrPPCurve { get => _arrPPCurve; set => _arrPPCurve = value; }
        [DefaultValue(null)]
        public double? BasePPMultiplier { get => basePPMultiplier; set => basePPMultiplier = value; }
        public CurveType CurveType { get => _curveType; set => _curveType = value; }
        [DefaultValue(null)]
        public double? Baseline { get => baseline; set => baseline = value; }
        [DefaultValue(null)]
        public double? Exponential { get => exponential; set => exponential = value; }
        [DefaultValue(null)]
        public double? Cutoff { get => cutoff; set => cutoff = value; }

        public CurveInfo()
        {
        }

        public CurveInfo(CurveType curveType)
        {
            _curveType = curveType;
        }

        public CurveInfo(CurveType curveType, List<(double, double)> arrPPCurve, double basePPMultiplier)
        {
            _curveType = curveType;
            _arrPPCurve = arrPPCurve;
            this.basePPMultiplier = basePPMultiplier;
        }

        public CurveInfo(CurveType curveType, List<(double, double)> arrPPCurve, double basePPMultiplier, double? baseline, double? exponential, double? cutoff) : this(curveType, arrPPCurve, basePPMultiplier)
        {
            this.baseline = baseline;
            this.exponential = exponential;
            this.cutoff = cutoff;
        }

        public override string ToString()
        {
            return $"CurveInfo: curveType {_curveType} - basePPMultiplier {basePPMultiplier.GetValueOrDefault()} _arrPPCurve {_arrPPCurve} baseline {baseline.GetValueOrDefault()} exponential {exponential.GetValueOrDefault()} cutoff {cutoff.GetValueOrDefault()}";
        }
    }
}
