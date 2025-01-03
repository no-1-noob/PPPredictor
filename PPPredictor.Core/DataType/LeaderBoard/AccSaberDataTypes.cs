using System;

namespace PPPredictor.Core.DataType.LeaderBoard
{
    internal class AccSaberDataTypes
    {
        internal class AccSaberScores
        {
            public double ap { get; set; }
            public double weightedAp { get; set; }
            public string songHash { get; set; }
            public string difficulty { get; set; }
            public DateTime timeSet { get; set; }
            public string categoryDisplayName { get; set; }
        }

        internal class AccSaberPlayer
        {
            public int rank { get; set; }
            public double ap { get; set; }
        }

        internal class AccSaberMapPool
        {
            public string categoryName { get; set; }
            public string categoryDisplayName { get; set; }
        }

        internal class AccSaberRankedMap
        {
            public string difficulty { get; set; }
            public string songHash { get; set; }
            public double complexity { get; set; }
            public string categoryDisplayName { get; set; }
        }
    }
}
