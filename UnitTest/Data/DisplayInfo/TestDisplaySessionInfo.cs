using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Utilities;

namespace UnitTest.Data.DisplayInfo
{
    [TestClass]
    public class TestDisplaySessionInfo
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            string dash = "-";
            DisplaySessionInfo info = new DisplaySessionInfo();
            Assert.AreEqual(info.SessionRank, dash);
            Assert.AreEqual(info.SessionRankDiff, dash);
            Assert.AreEqual(info.SessionRankDiffColor, DisplayHelper.ColorWhite);
            Assert.AreEqual(info.SessionCountryRank, dash);
            Assert.AreEqual(info.SessionCountryRankDiff, dash);
            Assert.AreEqual(info.SessionCountryRankDiffColor, DisplayHelper.ColorWhite);
            Assert.AreEqual(info.CountryRankFontColor, DisplayHelper.ColorWhite);
            Assert.AreEqual(info.SessionPP, dash);
            Assert.AreEqual(info.SessionPPDiff, dash);
            Assert.AreEqual(info.SessionPPDiffColor, DisplayHelper.ColorWhite);
        }

        [TestMethod]
        public void TestSetter()
        {
            string dash = "-";
            string newValue = "Test";
            DisplaySessionInfo info = new DisplaySessionInfo();
            info.SessionRank = newValue;
            info.SessionRankDiff = newValue;
            info.SessionRankDiffColor = DisplayHelper.ColorRed;
            info.SessionCountryRank = newValue;
            info.SessionCountryRankDiff = newValue;
            info.SessionCountryRankDiffColor = DisplayHelper.ColorRed;
            info.CountryRankFontColor = DisplayHelper.ColorRed;
            info.SessionPP = newValue;
            info.SessionPPDiff = newValue;
            info.SessionPPDiffColor = DisplayHelper.ColorRed;

            Assert.AreNotEqual(info.SessionRank, dash);
            Assert.AreNotEqual(info.SessionRankDiff, dash);
            Assert.AreNotEqual(info.SessionRankDiffColor, DisplayHelper.ColorWhite);
            Assert.AreNotEqual(info.SessionCountryRank, dash);
            Assert.AreNotEqual(info.SessionCountryRankDiff, dash);
            Assert.AreNotEqual(info.SessionCountryRankDiffColor, DisplayHelper.ColorWhite);
            Assert.AreNotEqual(info.CountryRankFontColor, DisplayHelper.ColorWhite);
            Assert.AreNotEqual(info.SessionPP, dash);
            Assert.AreNotEqual(info.SessionPPDiff, dash);
            Assert.AreNotEqual(info.SessionPPDiffColor, DisplayHelper.ColorWhite);
        }
    }
}
