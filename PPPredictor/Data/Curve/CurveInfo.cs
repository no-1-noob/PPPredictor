using PPPredictor.Utilities;
using System.ComponentModel;

namespace PPPredictor.Data.Curve
{
    public class CurveInfo
    {
        private CurveType _curveType;
        private double[,] _arrPPCurve;
        private double? basePPMultiplier;
        private double? baseline;
        private double? exponential;
        private double? cutoff;

        public double[,] ArrPPCurve { get => _arrPPCurve; set => _arrPPCurve = value; }
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

        public CurveInfo(CurveType curveType, double[,] arrPPCurve, double basePPMultiplier)
        {
            _curveType = curveType;
            _arrPPCurve = arrPPCurve;
            this.basePPMultiplier = basePPMultiplier;
        }

        public CurveInfo(CurveType curveType, double[,] arrPPCurve, double basePPMultiplier, double? baseline, double? exponential, double? cutoff) : this(curveType, arrPPCurve, basePPMultiplier)
        {
            this.baseline = baseline;
            this.exponential = exponential;
            this.cutoff = cutoff;
        }

        public override string ToString()
        {
            return $"CurveInfo: curveType {_curveType} - basePPMultiplier {basePPMultiplier} _arrPPCurve {_arrPPCurve} baseline {baseline.GetValueOrDefault()} exponential {exponential.GetValueOrDefault()} cutoff {cutoff.GetValueOrDefault()}";
        }
    }
}
