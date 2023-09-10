using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPPredictor.Data;

namespace UnitTest.Data
{
    [TestClass]
    public class TestRankGainResult
    {
        [TestMethod]
        public void DefaultConstuctor()
        {
            RankGainResult ppGainResult = new RankGainResult();
            Assert.IsTrue(ppGainResult.RankGlobal == -1, "RankGlobal should be -1");
            Assert.IsTrue(ppGainResult.RankCountry == -1, "RankCountry should be -1");
            Assert.IsTrue(ppGainResult.RankGainGlobal == -1, "RankGainGlobal should be -1");
            Assert.IsTrue(ppGainResult.RankGainCountry == -1, "RankGainCountry should be -1");
        }
        [TestMethod]
        public void TripleConstructor()
        {
            RankGainResult ppGainResult = new RankGainResult(123, 234, new PPPPlayer());
            Assert.IsTrue(ppGainResult.RankGlobal == 123, "RankGlobal should be 123");
            Assert.IsTrue(ppGainResult.RankCountry == 234, "RankCountry should be 234");
            Assert.IsTrue(ppGainResult.RankGainGlobal == -123, "RankGainGlobal should be -123");
            Assert.IsTrue(ppGainResult.RankGainCountry == -234, "RankGainCountry should be -234");
        }
        [TestMethod]
        public void QuadConstructor()
        {
            RankGainResult ppGainResult = new RankGainResult(123, 234, 345, 456);
            Assert.IsTrue(ppGainResult.RankGlobal == 123, "RankGlobal should be 123");
            Assert.IsTrue(ppGainResult.RankCountry == 234, "RankCountry should be 234");
            Assert.IsTrue(ppGainResult.RankGainGlobal == 345, "RankGainGlobal should be 345");
            Assert.IsTrue(ppGainResult.RankGainCountry == 456, "RankGainCountry should be 456");
        }
    }
}
