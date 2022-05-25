namespace PPPredictor.Data
{
    public class PPGainResult
    {
        private readonly double _ppTotal;
        private readonly double _ppGain;

        public double PpTotal { get => _ppTotal; }
        public double PpGain { get => _ppGain; }

        public PPGainResult(double ppTotal, double ppGain)
        {
            _ppTotal = ppTotal;
            _ppGain = ppGain;
        }
    }
}
