using PPPredictor.Core.DataType.MapPool;
using System.Collections.Generic;

namespace PPPredictor.Core.DataType.LeaderBoard
{
    public class LeaderboardData
    {
        private Dictionary<string, PPPMapPool> dctMapPool = new Dictionary<string, PPPMapPool>();

        public Dictionary<string, PPPMapPool> DctMapPool { get => dctMapPool; set => dctMapPool = value; }
    }
}
