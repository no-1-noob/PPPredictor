using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.TestUtils
{
    public class TestUtils
    {
        public static CustomBeatmapLevel CreateCustomBeatmapLevel()
        {
            return new CustomBeatmapLevel(new CustomPreviewBeatmapLevel(null, null, null, null, null, null, null, null, null, 0, 0, 0, 0, 0, 0, null, null, null));
        }

        public static CustomDifficultyBeatmap CreateCustomDifficultyBeatmap()
        {
            return new CustomDifficultyBeatmap(null, null, 0, 0, 0, 0, 0, null, null);
        }
    }
}
