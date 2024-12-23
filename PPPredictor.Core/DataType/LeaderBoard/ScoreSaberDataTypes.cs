using System.Collections.Generic;

namespace PPPredictor.Core.DataType.LeaderBoard
{
    internal class ScoreSaberDataTypes
    {
#pragma warning disable IDE1006 // Naming Styles; api dictates them...
        public class ScoreSaberPlayerList
        {
            public List<ScoreSaberPlayer> players { get; set; }

            public ScoreSaberPlayerList()
            {
                players = new List<ScoreSaberPlayer>();
            }
        }

        public class ScoreSaberPlayer
        {
            public string country { get; set; }
            public double pp { get; set; }
            public double rank { get; set; }
            public double countryRank { get; set; }
        }

        public class ScoreSaberPlayerScoreList
        {
            public List<ScoreSaberPlayerScore> playerScores { get; set; }
            public ScoreSaberScoreListMetadata metadata { get; set; }
        }

        public class ScoreSaberScoreListMetadata
        {
            public double total { get; set; }
            public double page { get; set; }
            public double itemsPerPage { get; set; }
        }

        public class ScoreSaberPlayerScore
        {
            public ScoreSaberScore score { get; set; } = new ScoreSaberScore();
            public ScoreSaberLeaderboardInfo leaderboard { get; set; } = new ScoreSaberLeaderboardInfo();
        }

        public class ScoreSaberLeaderboardInfo
        {
            public string songHash { get; set; }
            public ScoreSaberDifficulty difficulty { get; set; } = new ScoreSaberDifficulty();
        }

        public class ScoreSaberDifficulty
        {
            public double difficulty { get; set; }
            public string gameMode { get; set; }
        }

        public class ScoreSaberScore
        {
            public double pp { get; set; }
            public System.DateTimeOffset timeSet { get; set; }
        }
#pragma warning restore IDE1006 // Naming Styles; api dictates them...
    }
}
