using PPPredictor.Data;
using PPPredictor.Data.Curve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Data.Curve
{
    [TestClass]
    public class TestBeatLeaderPPPCurve
    {
        [TestMethod]
        public void TestCalculatePPatPercentage()
        {
            PPPBeatMapInfo beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1, 11.82055, 7.9189496, 5.424088));
            BeatLeaderPPPCurve curve = new BeatLeaderPPPCurve();
            double result = curve.CalculatePPatPercentage(beatMapInfo, 96.39, false);
            bool isInRange = result < 595.08 && result > 593.08;
            Assert.IsTrue(isInRange);
            
            result = curve.CalculatePPatPercentage(beatMapInfo, 96.39, true);
            Assert.AreEqual(result, 0, "Failed Beatleader level should return 0pp");
        }

        [TestMethod]
        public void TestToCurveInfo()
        {
            PPPBeatMapInfo beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1, 11.82055, 7.9189496, 5.424088));
            BeatLeaderPPPCurve curve = new BeatLeaderPPPCurve();
            CurveInfo result = curve.ToCurveInfo();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.CurveType, PPPredictor.Utilities.CurveType.BeatLeader);
        }

        [TestMethod]
        public void TestInfintePassPP()
        {
            PPPBeatMapInfo beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1, 11.82055, -7.9189496, 5.424088));
            BeatLeaderPPPCurve curve = new BeatLeaderPPPCurve();
            double result = curve.CalculatePPatPercentage(beatMapInfo, 96.39, false);
            Assert.IsNotNull(result);
            bool isInRange = result < 462 && result > 461;
            Assert.IsTrue(isInRange);
        }

        [TestMethod]
        public void TestExceptionHandling()
        {
            PPPBeatMapInfo beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1, 0, 0, 0));
            BeatLeaderPPPCurve curve = new BeatLeaderPPPCurve();
            double result = curve.CalculatePPatPercentage(beatMapInfo, double.NaN, false);
            Assert.IsNotNull(result);
            Assert.AreEqual(result, -1, "Returns -1 if anything goes wrong");
        }

        [TestMethod]
        public void TestCalculateMaxPP()
        {
            PPPBeatMapInfo beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1, 11.82055, 7.9189496, 5.424088));
            BeatLeaderPPPCurve curve = new BeatLeaderPPPCurve();
            double result = curve.CalculateMaxPP(beatMapInfo);
            bool isInRange = result < 5017 && result > 5014;
            Assert.IsTrue(isInRange);
        }
    }
}
