using PPPredictor.Data.Curve;
using PPPredictor.Utilities;

namespace UnitTests.Data.Curve
{
    [TestClass]
    public class TestCurveParser
    {
        [TestMethod]
        public void ParseToCurveScoreSaber()
        {
            var curve = CurveParser.ParseToCurve(new CurveInfo(CurveType.ScoreSaber));
            Assert.IsInstanceOfType(curve, typeof(CustomPPPCurve));
            Assert.IsFalse(curve.IsDummy);
        }

        [TestMethod]
        public void ParseToCurveBeatLeader()
        {
            var curve = CurveParser.ParseToCurve(new CurveInfo(CurveType.BeatLeader));
            Assert.IsInstanceOfType(curve, typeof(BeatLeaderPPPCurve));
            Assert.IsFalse(curve.IsDummy);
        }

        [TestMethod]
        public void ParseToCurveLinear()
        {
            var curve = CurveParser.ParseToCurve(new CurveInfo(CurveType.Linear));
            Assert.IsInstanceOfType(curve, typeof(CustomPPPCurve));
            Assert.IsFalse(curve.IsDummy);
        }

        [TestMethod]
        public void ParseToCurveBasic()
        {
            var curve = CurveParser.ParseToCurve(new CurveInfo(CurveType.Basic));
            Assert.IsInstanceOfType(curve, typeof(CustomPPPCurve));
            Assert.IsFalse(curve.IsDummy);
        }

        [TestMethod]
        public void ParseToCurveDummy()
        {
            var curve = CurveParser.ParseToCurve(new CurveInfo(CurveType.Dummy));
            Assert.IsInstanceOfType(curve, typeof(CustomPPPCurve));
            Assert.IsTrue(curve.IsDummy);
        }
    }
}
