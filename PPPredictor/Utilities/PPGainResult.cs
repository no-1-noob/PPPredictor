namespace PPPredictor.Utilities
{
    class PPGainResult
    {
        private double _ppTotal;
        private double _ppGain;

        public double PpTotal { get => _ppTotal; }
        public double PpGain { get => _ppGain; }

        public PPGainResult(double ppTotal, double ppGain)
        {
            _ppTotal = ppTotal;
            _ppGain = ppGain;
        }
    }
}
