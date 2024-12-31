using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.DataType
{
    public class PPGainResult
    {
        private readonly double _ppTotal;
        private readonly double _ppGainWeighted;
        private readonly double _ppGainRaw;
        private readonly double _ppDisplayValue;

        public double PpTotal { get => _ppTotal; }
        public double PpGainWeighted { get => _ppGainWeighted; }
        public double PpGainRaw { get => _ppGainRaw; }
        public double PpDisplayValue => _ppDisplayValue;

        public PPGainResult() : this(0,0,0, PPGainCalculationType.Raw) {
            _ppDisplayValue = 0;
        }

        public PPGainResult(double ppTotal, double ppGainWeighted, double ppGainRaw, PPGainCalculationType pPGainCalculationType)
        {
            _ppTotal = ppTotal;
            _ppGainWeighted = ppGainWeighted;
            _ppGainRaw = ppGainRaw;
            switch (pPGainCalculationType)
            {
                case PPGainCalculationType.Weighted:
                    _ppDisplayValue = PpGainWeighted;
                    break;
                case PPGainCalculationType.Raw:
                    _ppDisplayValue = PpGainRaw;
                    break;
            }
        }

        public override string ToString()
        {
            return $"PPGainResult: PpTotal {PpTotal} PpGainWeighted {PpGainWeighted} PpGainRaw {PpGainRaw} GetDisplayPPValue {PpDisplayValue}";
        }
    }
}
