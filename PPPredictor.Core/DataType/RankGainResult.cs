namespace PPPredictor.Core.DataType
{
    public class RankGainResult
    {
        private double _rankGlobal;
        private double _rankCountry;
        private double _rankGainGlobal;
        private double _rankGainCountry;
        private bool _isRankGainCanceledByLimit;

        public double RankGlobal { get => _rankGlobal; }
        public double RankCountry { get => _rankCountry; }
        public double RankGainGlobal { get => _rankGainGlobal; }
        public double RankGainCountry { get => _rankGainCountry; }
        public bool IsRankGainCanceledByLimit { get => _isRankGainCanceledByLimit; set => _isRankGainCanceledByLimit = value; }

        public RankGainResult()
        {
            _rankGlobal = -1;
            _rankCountry = -1;
            _rankGainGlobal = -1;
            _rankGainCountry = -1;
            _isRankGainCanceledByLimit = false;
        }

        public RankGainResult(double rankGlobal, double rankCountry, PPPPlayer currentPlayer, bool isRankGainCanceledByLimit = false)
        {
            _rankCountry = rankCountry;
            _rankGlobal = rankGlobal;
            _rankGainGlobal = currentPlayer.Rank - rankGlobal;
            _rankGainCountry = currentPlayer.CountryRank - rankCountry;
            _isRankGainCanceledByLimit = isRankGainCanceledByLimit;
        }

        public RankGainResult(double rankGlobal, double rankCountry, double rankGainGlobal, double rankGainCountry)
        {
            _rankGlobal = rankGlobal;
            _rankCountry = rankCountry;
            _rankGainGlobal = rankGainGlobal;
            _rankGainCountry = rankGainCountry;
            _isRankGainCanceledByLimit = false;
        }
    }
}
