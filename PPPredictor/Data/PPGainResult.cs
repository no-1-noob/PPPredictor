using PPPredictor.Utilities;
namespace PPPredictor.Data
{
    class PPGainResult
    {
        private readonly double _ppTotal;
        private readonly double _ppGainWeighted;
        private readonly double _ppGainRaw;

        public double PpTotal { get => _ppTotal; }
        public double PpGainWeighted { get => _ppGainWeighted; }
        public double PpGainRaw { get => _ppGainRaw; }

        public PPGainResult() : this(0,0,0) { }

        public PPGainResult(double ppTotal, double ppGain, double ppGainRaw)
        {
            _ppTotal = ppTotal;
            _ppGainWeighted = ppGain;
            _ppGainRaw = ppGainRaw;
        }

        internal double GetDisplayPPValue()
        {
            double retValue = 0;
            switch (Plugin.ProfileInfo.PpGainCalculationType)
            {
                case PPGainCalculationType.Weighted:
                    retValue = PpGainWeighted;
                    break;
                case PPGainCalculationType.Raw:
                    retValue = PpGainRaw;
                    break;
            }
            return retValue;
        }

        public override string ToString()
        {
            return $"PPGainResult: PpTotal {PpTotal} PpGainWeighted {PpGainWeighted} PpGainRaw {PpGainRaw} GetDisplayPPValue {GetDisplayPPValue()}";
        }
    }
}
