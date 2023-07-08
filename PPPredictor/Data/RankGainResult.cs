namespace PPPredictor.Data
{
    class RankGainResult
    {
        private double _rankGlobal;
        private double _rankCountry;
        private double _rankGainGlobal;
        private double _rankGainCountry;

        public double RankGlobal { get => _rankGlobal; }
        public double RankCountry { get => _rankCountry; }
        public double RankGainGlobal { get => _rankGainGlobal; }
        public double RankGainCountry { get => _rankGainCountry; }

        public RankGainResult()
        {
            _rankGlobal = -1;
            _rankCountry = -1;
            _rankGainGlobal = -1;
            _rankGainCountry = -1;
        }

        public RankGainResult(double rankGlobal, double rankCountry, PPPPlayer currentPlayer)
        {
            _rankCountry = rankCountry;
            _rankGlobal = rankGlobal;
            _rankGainGlobal = currentPlayer.Rank - rankGlobal;
            _rankGainCountry = currentPlayer.CountryRank - rankCountry;
        }

        public RankGainResult(double rankGlobal, double rankCountry, double rankGainGlobal, double rankGainCountry)
        {
            _rankGlobal = rankGlobal;
            _rankCountry = rankCountry;
            _rankGainGlobal = rankGainGlobal;
            _rankGainCountry = rankGainCountry;
        }
    }
}
