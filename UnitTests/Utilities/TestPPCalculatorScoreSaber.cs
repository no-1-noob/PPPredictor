using PPPredictor.OpenAPIs;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
