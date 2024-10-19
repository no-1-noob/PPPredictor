using PPPredictor.Data.DisplayInfos;
using PPPredictor.Utilities;

namespace UnitTests.Data.DisplayInfo
{
    [TestClass]
    public class TestDisplayPPInfo
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            DisplayPPInfo info = new DisplayPPInfo();
            Assert.AreEqual(info.PPRaw, string.Empty);
            Assert.AreEqual(info.PPGain, string.Empty);
            Assert.AreEqual(info.PPGainDiffColor, DisplayHelper.ColorWhite);
            Assert.AreEqual(info.PredictedRank, string.Empty);
            Assert.AreEqual(info.PredictedRankDiff, string.Empty);
            Assert.AreEqual(info.PredictedRankDiffColor, DisplayHelper.ColorWhite);
            Assert.AreEqual(info.PredictedCountryRank, string.Empty);
            Assert.AreEqual(info.PredictedCountryRankDiff, string.Empty);
            Assert.AreEqual(info.PredictedCountryRankDiffColor, DisplayHelper.ColorWhite);
        }

        [TestMethod]
        public void TestSetter()
        {
            string testPPRaw = "PPRaw";
            string testPPGain = "PPGain";
            string testPPGainDiffColor = "PPGainDiffColor";
            string testPredictedRank = "PredictedRank";
            string testPredictedRankDiff = "PredictedRankDiff";
            string testPredictedRankDiffColor = "PredictedRankDiffColor";
            string testPredictedCountryRank = "PredictedCountryRank";
            string testPredictedCountryRankDiff = "PredictedCountryRankDiff";
            string testPredictedCountryRankDiffColor = "PredictedCountryRankDiffColor";
            DisplayPPInfo info = new DisplayPPInfo();

            info.PPRaw = testPPRaw;
            info.PPGain = testPPGain;
            info.PPGainDiffColor = testPPGainDiffColor;
            info.PredictedRank = testPredictedRank;
            info.PredictedRankDiff = testPredictedRankDiff;
            info.PredictedRankDiffColor = testPredictedRankDiffColor;
            info.PredictedCountryRank = testPredictedCountryRank;
            info.PredictedCountryRankDiff = testPredictedCountryRankDiff;
            info.PredictedCountryRankDiffColor = testPredictedCountryRankDiffColor;

            Assert.AreEqual(info.PPRaw, testPPRaw);
            Assert.AreEqual(info.PPGain, testPPGain);
            Assert.AreEqual(info.PPGainDiffColor, testPPGainDiffColor);
            Assert.AreEqual(info.PredictedRank, testPredictedRank);
            Assert.AreEqual(info.PredictedRankDiff, testPredictedRankDiff);
            Assert.AreEqual(info.PredictedRankDiffColor, testPredictedRankDiffColor);
            Assert.AreEqual(info.PredictedCountryRank, testPredictedCountryRank);
            Assert.AreEqual(info.PredictedCountryRankDiff, testPredictedCountryRankDiff);
            Assert.AreEqual(info.PredictedCountryRankDiffColor, testPredictedCountryRankDiffColor);
        }
    }
}
