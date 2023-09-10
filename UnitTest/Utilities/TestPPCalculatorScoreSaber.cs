using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPPredictor.Utilities;

namespace UnitTest.Utilities
{
    [TestClass]
    public class TestPPCalculatorScoreSaber
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            PPCalculator calculator = new PPCalculatorScoreSaber<MockServices.MockScoreSaberApi>();
            var profile = calculator.GetProfile("123");
        }
    }
}
