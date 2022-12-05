using PPPredictor.Utilities;

namespace PPPredictor.Data.Curve
{
    public class CurveInfo
    {
        private CurveType _curveType;
        private double[,] _arrPPCurve;
        private double? basePPMultiplier;

        public double[,] ArrPPCurve { get => _arrPPCurve; set => _arrPPCurve = value; }
        public double? BasePPMultiplier { get => basePPMultiplier; set => basePPMultiplier = value; }
        public CurveType CurveType { get => _curveType; set => _curveType = value; }

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

        public override string ToString()
        {
            return $"CurveInfo: curveType {_curveType} - basePPMultiplier {basePPMultiplier} _arrPPCurve {_arrPPCurve}";
        }
    }
}
