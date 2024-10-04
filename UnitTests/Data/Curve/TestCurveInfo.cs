using PPPredictor.Data.Curve;

namespace UnitTests.Data.Curve
{
    [TestClass]
    public class TestCurveInfo
    {
        private List<(double, double)> _testArrPPCurve = new System.Collections.Generic.List<(double, double)>()
        {
            (1,1),
            (2,2),
            (3,3),
            (4,4),
            (5,5),
        };
        private double _testBasePPMulti = 1;
        private double _testBaseline = 2;
        private double _testExponential = 3;
        private double _testCutoff = 4;

        [TestMethod]
        public void DefaultConstructor()
        {
            CurveInfo info = new CurveInfo();
            Assert.IsNull(info.ArrPPCurve);
            Assert.IsNull(info.BasePPMultiplier);
            Assert.AreEqual(info.CurveType, PPPredictor.Utilities.CurveType.ScoreSaber);
            Assert.IsNull(info.Baseline);
            Assert.IsNull(info.Exponential);
            Assert.IsNull(info.Cutoff);
        }

        [TestMethod]
        public void TestSetter()
        {
            CurveInfo info = new CurveInfo();
            info.ArrPPCurve = _testArrPPCurve;
            info.BasePPMultiplier = _testBasePPMulti;
            info.Baseline = _testBaseline;
            info.Exponential = _testExponential;
            info.Cutoff = _testCutoff;
            info.CurveType = PPPredictor.Utilities.CurveType.Linear;
            Assert.IsNotNull(info.ArrPPCurve);
            Assert.AreEqual(info.ArrPPCurve.Count, _testArrPPCurve.Count);
            Assert.AreEqual(info.ArrPPCurve.Last(), _testArrPPCurve.Last());
            Assert.AreEqual(info.BasePPMultiplier, _testBasePPMulti);
            Assert.AreEqual(info.Baseline, _testBaseline);
            Assert.AreEqual(info.Exponential, _testExponential);
            Assert.AreEqual(info.Cutoff, _testCutoff);
            Assert.AreEqual(info.CurveType, PPPredictor.Utilities.CurveType.Linear);
        }

        [TestMethod]
        public void SinglleConstructor()
        {
            CurveInfo info = new CurveInfo(PPPredictor.Utilities.CurveType.BeatLeader);
            Assert.IsNull(info.ArrPPCurve);
            Assert.IsNull(info.BasePPMultiplier);
            Assert.AreEqual(info.CurveType, PPPredictor.Utilities.CurveType.BeatLeader);
            Assert.IsNull(info.Baseline);
            Assert.IsNull(info.Exponential);
            Assert.IsNull(info.Cutoff);
        }

        [TestMethod]
        public void TripleConstructor()
        {
            CurveInfo info = new CurveInfo(PPPredictor.Utilities.CurveType.Basic, _testArrPPCurve, _testBasePPMulti);
            Assert.IsNotNull(info.ArrPPCurve);
            Assert.AreEqual(info.ArrPPCurve.Count, _testArrPPCurve.Count);
            Assert.AreEqual(info.ArrPPCurve.Last(), _testArrPPCurve.Last());
            Assert.AreEqual(info.BasePPMultiplier, _testBasePPMulti);
            Assert.AreEqual(info.CurveType, PPPredictor.Utilities.CurveType.Basic);
            Assert.IsNull(info.Baseline);
            Assert.IsNull(info.Exponential);
            Assert.IsNull(info.Cutoff);
        }

        [TestMethod]
        public void SextupleConstructor()
        {
            CurveInfo info = new CurveInfo(PPPredictor.Utilities.CurveType.Basic, _testArrPPCurve, _testBasePPMulti, _testBaseline, _testExponential, _testCutoff);
            Assert.IsNotNull(info.ArrPPCurve);
            Assert.AreEqual(info.ArrPPCurve.Count, _testArrPPCurve.Count);
            Assert.AreEqual(info.ArrPPCurve.Last(), _testArrPPCurve.Last());
            Assert.AreEqual(info.BasePPMultiplier, _testBasePPMulti);
            Assert.AreEqual(info.CurveType, PPPredictor.Utilities.CurveType.Basic);
            Assert.AreEqual(info.Baseline, _testBaseline);
            Assert.AreEqual(info.Exponential, _testExponential);
            Assert.AreEqual(info.Cutoff, _testCutoff);
        }

        [TestMethod]
        public void TestToString()
        {
            CurveInfo info = new CurveInfo(PPPredictor.Utilities.CurveType.Basic, _testArrPPCurve, _testBasePPMulti, _testBaseline, _testExponential, _testCutoff);
            Assert.AreEqual(info.ToString(), $"CurveInfo: curveType {PPPredictor.Utilities.CurveType.Basic} - basePPMultiplier {_testBasePPMulti} _arrPPCurve {_testArrPPCurve} baseline {_testBaseline} exponential {_testExponential} cutoff {_testCutoff}");

            info = new CurveInfo();
            Assert.AreEqual(info.ToString(), $"CurveInfo: curveType {PPPredictor.Utilities.CurveType.ScoreSaber} - basePPMultiplier {0} _arrPPCurve {null} baseline {0} exponential {0} cutoff {0}");
        }
    }
}
