using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.TestUtils
{
    public class TestUtils
    {
        public static BeatmapLevel CreateBeatmapLevel()
        {
            return new BeatmapLevel(1,false, string.Empty, string.Empty, string.Empty, string.Empty, new string[0], new string[0], 1, 1, 1, 1, 1, 1, PlayerSensitivityFlag.Explicit, null, null);
        }

        public static BeatmapKey CreateBeatmapKey()
        {
            return new BeatmapKey();
        }
    }
}
