using PPPredictor.Data;
using PPPredictor.Data.Curve;
using PPPredictor.OpenAPIs;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PPPredictor.Data.LeaderBoardDataTypes.HitBloqDataTypes;

namespace UnitTests.Data.Curve
{
    [TestClass]
    public class TestCustomPPPCurve
    {
        private List<(double, double)> _testArrPPCurve = new System.Collections.Generic.List<(double, double)>()
        {
            (1.0, 1.0),
            (0.0, 0.0)
        };
        private double _testBasePPMulti = 1;
        private double _testBaseline = .95;
        private double _testExponential = 15;
        private double _testCutoff = 25;
        private PPPBeatMapInfo beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1));

        [TestMethod]
        public void TestConstructorOneLinear()
        {

            CustomPPPCurve curve = new CustomPPPCurve(_testArrPPCurve, CurveType.Linear, _testBasePPMulti, true);
            Assert.IsNotNull(curve);
            Assert.IsTrue(curve.IsDummy);
            curve = new CustomPPPCurve(_testArrPPCurve, CurveType.Linear, _testBasePPMulti);
            Assert.IsFalse(curve.IsDummy);
            Assert.AreEqual(curve.CalculateMaxPP(beatMapInfo), curve.CalculatePPatPercentage(beatMapInfo, 100, false, false));
            Assert.AreEqual(curve.CalculateMaxPP(beatMapInfo) * .5, curve.CalculatePPatPercentage(beatMapInfo, 50, false, false));
            Assert.AreEqual(0, curve.CalculatePPatPercentage(beatMapInfo, 0f, false, false));
            Assert.AreEqual(curve.CalculateMaxPP(beatMapInfo), curve.CalculatePPatPercentage(beatMapInfo, 100, true, false), "Isfailed should not affect linear");
        }

        [TestMethod]
        public void TestConstructorOneScoreSaber()
        {

            CustomPPPCurve curve = new CustomPPPCurve(_testArrPPCurve, CurveType.ScoreSaber, _testBasePPMulti, true);
            Assert.IsNotNull(curve);
            Assert.IsTrue(curve.IsDummy);
            curve = new CustomPPPCurve(_testArrPPCurve, CurveType.ScoreSaber, _testBasePPMulti);
            Assert.IsFalse(curve.IsDummy);
            Assert.AreEqual(curve.CalculateMaxPP(beatMapInfo), curve.CalculatePPatPercentage(beatMapInfo, 100, false, false));
            Assert.AreEqual(curve.CalculateMaxPP(beatMapInfo) * .5, curve.CalculatePPatPercentage(beatMapInfo, 50, false, false));
            Assert.AreEqual(0, curve.CalculatePPatPercentage(beatMapInfo, 0f, false, false));
            Assert.AreEqual(curve.CalculateMaxPP(beatMapInfo) * .5, curve.CalculatePPatPercentage(beatMapInfo, 100, true, false), "Isfailed halves score");
        }

        [TestMethod]
        public void TestEmptyCurveArray()
        {

            CustomPPPCurve curve = new CustomPPPCurve(new List<(double, double)>(), CurveType.Linear, _testBasePPMulti, true);
            Assert.AreEqual(0, curve.CalculatePPatPercentage(beatMapInfo, 100, false, false));
            Assert.AreEqual(0, curve.CalculatePPatPercentage(beatMapInfo, 50, false, false));
            Assert.AreEqual(0, curve.CalculatePPatPercentage(beatMapInfo, 0f, false, false));
            Assert.AreEqual(0, curve.CalculatePPatPercentage(beatMapInfo, 100, true, false));
        }

        [TestMethod]
        public void TestCreateBasicPPPCurve()
        {
            CustomPPPCurve curve = CustomPPPCurve.CreateBasicPPPCurve(_testBasePPMulti, _testBaseline, _testExponential, _testCutoff);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 99, false, false), 23.905575069868032);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 75, false, false), 18.75);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 50, false, false), 12.5);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 25, false, false), 6.25);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 0, false, false), 0);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 99, true, false), 23.905575069868032);

            curve = CustomPPPCurve.CreateBasicPPPCurve(_testBasePPMulti, null, null, null);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 99, false, false), 0.9401040430010454);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 75, false, false), 0.375);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 50, false, false), 0.25);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 25, false, false), 0.125);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 0, false, false), 0);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 99, true, false), 0.9401040430010454);
        }

        [TestMethod]
        public void TestConstructorHitBloqCrCurve()
        {
            HitBloqCrCurve crCurve = new HitBloqCrCurve();
            crCurve.type = "linear";
            var lsTestArray = _testArrPPCurve.Select(x => new double[2] { x.Item1, x.Item2 }).ToList();
            lsTestArray.Reverse();
            crCurve.points = lsTestArray;
            CustomPPPCurve curve = new CustomPPPCurve(crCurve);
            Assert.AreEqual(50, curve.CalculatePPatPercentage(beatMapInfo, 100, false, false));
            Assert.AreEqual(25, curve.CalculatePPatPercentage(beatMapInfo, 50, false, false));
            Assert.AreEqual(0, curve.CalculatePPatPercentage(beatMapInfo, 0f, false, false));
            Assert.AreEqual(50, curve.CalculatePPatPercentage(beatMapInfo, 100, true, false));


            crCurve = new HitBloqCrCurve();
            crCurve.type = "basic";
            crCurve.baseline = _testBaseline;
            crCurve.cutoff = _testCutoff;
            crCurve.exponential = _testExponential;
            curve = new CustomPPPCurve(crCurve);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 99, false, false), 1195.2787534934016);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 75, false, false), 937.5);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 50, false, false), 625);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 25, false, false), 312.5);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 0, false, false), 0);
            Assert.AreEqual(curve.CalculatePPatPercentage(beatMapInfo, 99, true, false), 1195.2787534934016);
        }

        [TestMethod]
        public void TestToCurveInfo()
        {
            CustomPPPCurve curve = new CustomPPPCurve(_testArrPPCurve, CurveType.ScoreSaber, _testBasePPMulti);
            CurveInfo info = curve.ToCurveInfo();
            Assert.IsNotNull(info);
            Assert.AreEqual(info.CurveType, CurveType.ScoreSaber);
            curve = new CustomPPPCurve(_testArrPPCurve, CurveType.Basic, _testBasePPMulti);
            info = curve.ToCurveInfo();
            Assert.IsNotNull(info);
            Assert.AreEqual(info.CurveType, CurveType.Basic);
            curve = new CustomPPPCurve(_testArrPPCurve, CurveType.Linear, _testBasePPMulti);
            info = curve.ToCurveInfo();
            Assert.IsNotNull(info);
            Assert.AreEqual(info.CurveType, CurveType.Linear);
        }

        [TestMethod]
        public void TestDummyPPPCurve()
        {
            CustomPPPCurve curve = CustomPPPCurve.CreateDummyPPPCurve();
            Assert.IsNotNull(curve);
            Assert.IsTrue(curve.IsDummy);
            Assert.AreEqual(0, curve.CalculatePPatPercentage(beatMapInfo, 100, false, false));
        }

        [TestMethod]
        public void TestToString()
        {
            CustomPPPCurve curve = new CustomPPPCurve(new List<(double, double)>(), PPPredictor.Utilities.CurveType.BeatLeader, _testBasePPMulti);
            Assert.AreEqual(curve.ToString(), $"CustomPPPCurve curveType:{CurveType.BeatLeader} - basePPMultiplier: {_testBasePPMulti} - dummy? {false}");

            curve = new CustomPPPCurve(new List<(double, double)>(), PPPredictor.Utilities.CurveType.BeatLeader, _testBasePPMulti, true);
            Assert.AreEqual(curve.ToString(), $"CustomPPPCurve curveType:{CurveType.BeatLeader} - basePPMultiplier: {_testBasePPMulti} - dummy? {true}");
        }
    }
}
