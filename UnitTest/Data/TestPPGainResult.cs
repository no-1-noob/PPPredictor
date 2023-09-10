using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPPredictor.Data;
using PPPredictor;

namespace UnitTest.Data
{
    [TestClass]
    public class TestPPGainResult
    {
        #region PPGainResult
        [TestMethod]
        public void DefaultConstuctor()
        {
            PPGainResult ppGainResult = new PPGainResult();
            Assert.IsTrue(ppGainResult.PpTotal == 0, "PpTotal should be 0");
            Assert.IsTrue(ppGainResult.PpGainWeighted == 0, "PpGainWeighted should be 0");
            Assert.IsTrue(ppGainResult.PpGainRaw == 0, "PpGainRaw should be 0");
        }
        [TestMethod]
        public void TripleConstuctor()
        {
            PPGainResult ppGainResult = new PPGainResult(1, 2, 3);
            Assert.IsTrue(ppGainResult.PpTotal == 1, "PpTotal should be 1");
            Assert.IsTrue(ppGainResult.PpGainWeighted == 2, "PpGainWeighted should be 2");
            Assert.IsTrue(ppGainResult.PpGainRaw == 3, "PpGainRaw should be 3");
        }
        [TestMethod]
        public void GetDisplayPPValue()
        {
            Plugin p = new Plugin();
            PPGainResult ppGainResult = new PPGainResult(1, 2, 3);
            Assert.IsTrue(ppGainResult.PpTotal == 1, "PpTotal should be 1");
            Assert.IsTrue(ppGainResult.PpGainWeighted == 2, "PpGainWeighted should be 2");
            Assert.IsTrue(ppGainResult.GetDisplayPPValue() == ppGainResult.PpGainWeighted, "GetDisplayPPValue should give the PpGainWeighted result");
            Plugin.ProfileInfo.PpGainCalculationType = PPPredictor.Utilities.PPGainCalculationType.Raw;
            Assert.IsTrue(ppGainResult.GetDisplayPPValue() == ppGainResult.PpGainRaw, "GetDisplayPPValue should give the PpGainRaw result");
        }
        [TestMethod]
        public void TestToString()
        {
            Plugin p = new Plugin();
            PPGainResult ppGainResult = new PPGainResult(1, 2, 3);
            Assert.IsTrue(ppGainResult.ToString() == "PPGainResult: PpTotal 1 PpGainWeighted 2 PpGainRaw 3 GetDisplayPPValue 2", "ToString should show all values and the PpGainWeighted");
            Plugin.ProfileInfo.PpGainCalculationType = PPPredictor.Utilities.PPGainCalculationType.Raw;
            Assert.IsTrue(ppGainResult.ToString() == "PPGainResult: PpTotal 1 PpGainWeighted 2 PpGainRaw 3 GetDisplayPPValue 3", "ToString should show all values and the PpGainRaw");
        }
        #endregion
    }
}