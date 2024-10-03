using PPPredictor.Utilities;

namespace UnitTests.Utilities
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
