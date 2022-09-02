namespace PPPredictor.Data
{
    public class PPPPlayer
    {
        private double rank;
        private double countryRank;
        private double pp;
        private string country;

        public double Rank { get => rank; set => rank = value; }
        public double CountryRank { get => countryRank; set => countryRank = value; }
        public double Pp { get => pp; set => pp = value; }
        public string Country { get => country; set => country = value; }

        public PPPPlayer()
        {
            rank = countryRank = pp = 0;
        }
        public PPPPlayer(scoresaberapi.Player scoreSaberPlayer)
        {
            this.rank = scoreSaberPlayer.Rank;
            this.countryRank = scoreSaberPlayer.CountryRank;
            this.pp = scoreSaberPlayer.Pp;
            this.country = scoreSaberPlayer.Country;
        }

        public PPPPlayer(beatleaderapi.Player scoreSaberPlayer)
        {
            this.rank = scoreSaberPlayer.Rank;
            this.countryRank = scoreSaberPlayer.CountryRank;
            this.pp = scoreSaberPlayer.Pp;
            this.country = scoreSaberPlayer.Country;
        }

        public PPPPlayer(beatleaderapi.PlayerResponseWithStats scoreSaberPlayer)
        {
            this.rank = scoreSaberPlayer.Rank;
            this.countryRank = scoreSaberPlayer.CountryRank;
            this.pp = scoreSaberPlayer.Pp;
            this.country = scoreSaberPlayer.Country;
        }
    }
}
