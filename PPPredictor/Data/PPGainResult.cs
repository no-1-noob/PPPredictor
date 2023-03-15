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
        public double PpGainRaw => _ppGainRaw;

        public PPGainResult(double ppTotal, double ppGain, double ppGainRaw)
        {
            _ppTotal = ppTotal;
            _ppGainWeighted = ppGain;
            _ppGainRaw = ppGainRaw;
        }

        internal double GetDisplayPPValue()
        {
            switch (Plugin.ProfileInfo.PpGainCalculationType)
            {
                case PPGainCalculationType.Weighted:
                    return PpGainWeighted;
                case PPGainCalculationType.Raw:
                    return PpGainRaw;
                default:
                    return 0;
            }
        }

        public override string ToString()
        {
            return $"PPGainResult: PpTotal {PpTotal} PpGainWeighted {PpGainWeighted} PpGainRaw {PpGainRaw} GetDisplayPPValue {GetDisplayPPValue()}";
        }
    }
}
