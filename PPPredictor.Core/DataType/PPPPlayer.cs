using static PPPredictor.Core.DataType.LeaderBoard.AccSaberDataTypes;
using static PPPredictor.Core.DataType.LeaderBoard.BeatLeaderDataTypes;
using static PPPredictor.Core.DataType.LeaderBoard.HitBloqDataTypes;
using static PPPredictor.Core.DataType.LeaderBoard.ScoreSaberDataTypes;

namespace PPPredictor.Core.DataType
{
    public class PPPPlayer
    {
        private double rank;
        private double countryRank;
        private double pp;
        private string country;
        private readonly bool isErrorUser = false;

        public double Rank { get => rank; set => rank = value; }
        public double CountryRank { get => countryRank; set => countryRank = value; }
        public double Pp { get => pp; set => pp = value; }
        public string Country { get => country; set => country = value; }
        public bool IsErrorUser { get => isErrorUser; }

        public PPPPlayer()
        {
            rank = countryRank = pp = 0;
        }
        public PPPPlayer(bool isErrorUser)
        {
            this.isErrorUser = isErrorUser;
            rank = countryRank = pp = 0;
        }
        internal PPPPlayer(ScoreSaberPlayer scoreSaberPlayer)
        {
            rank = scoreSaberPlayer.rank;
            countryRank = scoreSaberPlayer.countryRank;
            pp = scoreSaberPlayer.pp;
            country = scoreSaberPlayer.country;
        }

        internal PPPPlayer(BeatLeaderPlayer beatLeaderPlayerEvent)
        {
            rank = beatLeaderPlayerEvent.rank;
            countryRank = beatLeaderPlayerEvent.countryRank;
            pp = beatLeaderPlayerEvent.pp;
            country = beatLeaderPlayerEvent.country;
        }

        internal PPPPlayer(BeatLeaderPlayerEvents beatLeaderPlayerEvent)
        {
            rank = beatLeaderPlayerEvent.rank;
            countryRank = beatLeaderPlayerEvent.countryRank;
            pp = beatLeaderPlayerEvent.pp;
            country = beatLeaderPlayerEvent.country;
        }

        internal PPPPlayer(HitBloqUser hitBloqUser)
        {
            rank = hitBloqUser.rank;
            countryRank = 0;
            pp = hitBloqUser.cr;
            country = string.Empty;
        }

        internal PPPPlayer(AccSaberPlayer accSaberPlayer)
        {
            rank = accSaberPlayer.rank;
            countryRank = 0;
            pp = accSaberPlayer.ap;
            country = string.Empty;
        }

        public override string ToString()
        {
            return $"PPPPlayer: (Rank): {rank} (CountryRank): {countryRank} (PP): {pp} (Country): {country}";
        }
    }
}
