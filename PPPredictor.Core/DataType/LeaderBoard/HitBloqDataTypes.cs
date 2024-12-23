using System.Collections.Generic;
using System.ComponentModel;

namespace PPPredictor.Core.DataType.LeaderBoard
{
    internal class HitBloqDataTypes
    {
#pragma warning disable IDE1006 // Naming Styles; api dictates them...
        public class HitBloqUserId
        {
            public long id { get; set; }
        }

        public class HitBloqMapPool
        {
            public string id { get; set; }
            public string title { get; set; }
            public string short_description { get; set; }
            public string image { get; set; }
            public int popularity { get; set; }
            public string download_url { get; set; }
        }

        public class HitBloqMapPoolDetails
        {
            public float accumulation_constant { get; set; }
            public HitBloqCrCurve cr_curve { get; set; }
            public List<string> leaderboard_id_list { get; set; }
            public string long_description { get; set; }
        }

        public class HitBloqCrCurve
        {
            public List<double[]> points { get; set; }
            public string type { get; set; }
            [DefaultValue(null)]
            public double? baseline { get; set; }
            [DefaultValue(null)]
            public double? exponential { get; set; }
            [DefaultValue(null)]
            public double? cutoff { get; set; }
        }

        public class HitBloqUser
        {
            public float cr { get; set; }
            public int rank { get; set; }
            public string username { get; set; }
        }

        public class HitBloqScores
        {
            public float cr_received { get; set; }
            public string song_id { get; set; }
            public long time { get; set; }
        }

        public class HitBloqLadder
        {
            public List<HitBloqUser> ladder { get; set; }
        }

        public class HitBloqRankFromCr
        {
            public int rank { get; set; }
        }

        public class HitBloqLeaderboardInfo
        {
            public Dictionary<string, double> star_rating { get; set; }
        }
#pragma warning restore IDE1006 // Naming Styles
    }
}
