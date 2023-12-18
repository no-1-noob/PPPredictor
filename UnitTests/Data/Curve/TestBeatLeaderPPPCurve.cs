using PPPredictor.Data;
using PPPredictor.Data.Curve;
using PPPredictor.Utilities;
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
        PPPBeatMapInfo beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1, 11.82055, 7.9189496, 5.424088, true));
        BeatLeaderPPPCurve curve = new BeatLeaderPPPCurve();

        [TestMethod]
        public void TestCalculatePPatPercentage()
        {
            double result = curve.CalculatePPatPercentage(beatMapInfo, 96.39, false, false);
            bool isInRange = result < 595.08 && result > 593.08;
            Assert.IsTrue(isInRange);

            result = curve.CalculatePPatPercentage(beatMapInfo, 96.39, true, false, LeaderboardContext.BeatLeaderDefault);
            Assert.AreEqual(result, 0, "Failed Beatleader level should return 0pp");

            result = curve.CalculatePPatPercentage(beatMapInfo, 96.39, true, false, LeaderboardContext.BeatLeaderNoPauses);
            Assert.AreEqual(result, 0, "Failed Beatleader level should return 0pp");

            result = curve.CalculatePPatPercentage(beatMapInfo, 96.39, true, false, LeaderboardContext.BeatLeaderNoModifiers);
            Assert.AreEqual(result, 0, "Failed Beatleader level should return 0pp");

            result = curve.CalculatePPatPercentage(beatMapInfo, 96.39, true, false, LeaderboardContext.BeatLeaderGolf);
            Assert.AreEqual(result, 0, "Failed Beatleader level should return 0pp");
        }

        [TestMethod]
        public void TestCalculatePPatPercentageWhenPaused()
        {
            double result = curve.CalculatePPatPercentage(beatMapInfo, 30, false, true, LeaderboardContext.BeatLeaderDefault);
            bool isInRange = result < 114 && result > 113;
            Assert.IsTrue(isInRange, "Paused level should only apply to NoPauseMode");

            result = curve.CalculatePPatPercentage(beatMapInfo, 30, false, true, LeaderboardContext.BeatLeaderNoPauses);
            Assert.AreEqual(result, 0, "Paused level should result in 0pp in NoPauseMode");

            result = curve.CalculatePPatPercentage(beatMapInfo, 30, false, true, LeaderboardContext.BeatLeaderNoModifiers);
            isInRange = result < 114 && result > 113;
            Assert.IsTrue(isInRange, "Paused level should only apply to NoPauseMode");

            result = curve.CalculatePPatPercentage(beatMapInfo, 30, false, true, LeaderboardContext.BeatLeaderGolf);
            isInRange = result < 436 && result > 435;
            Assert.IsTrue(isInRange, "Paused level should only apply to NoPauseMode");
        }

        [TestMethod]
        public void TestCalculatePPatPercentageModifier()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void TestCalculatePPatPercentageGolf()
        {
            beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1, 6.9128127, 3.937846, 3.5577383, true));

            double result = curve.CalculatePPatPercentage(beatMapInfo, 49.77, false, false, LeaderboardContext.BeatLeaderGolf);
            bool isInRange = result < 148 && result > 147;
            Assert.IsTrue(isInRange, "PP should be in given range");

            result = curve.CalculatePPatPercentage(beatMapInfo, 49.77, true, false, LeaderboardContext.BeatLeaderGolf);
            isInRange = result < 436 && result > 435;
            Assert.IsFalse(isInRange, "Only finished levels count");

            result = curve.CalculatePPatPercentage(beatMapInfo, 51, true, false, LeaderboardContext.BeatLeaderGolf);
            Assert.AreEqual(result, 0, "Percentage has to be 50 or below");
        }

        [TestMethod]
        public void TestToCurveInfo()
        {
            CurveInfo result = curve.ToCurveInfo();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.CurveType, CurveType.BeatLeader);
        }

        [TestMethod]
        public void TestInfintePassPP()
        {
            PPPBeatMapInfo beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1, 11.82055, -7.9189496, 5.424088, true));
            double result = curve.CalculatePPatPercentage(beatMapInfo, 96.39, false, false);
            Assert.IsNotNull(result);
            bool isInRange = result < 462 && result > 461;
            Assert.IsTrue(isInRange);
        }

        [TestMethod]
        public void TestExceptionHandling()
        {
            PPPBeatMapInfo beatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(1, 1, 1, 1, true));
            double result = curve.CalculatePPatPercentage(beatMapInfo, double.NaN, false, false);
            Assert.IsNotNull(result);
            Assert.AreEqual(result, -1, "Returns -1 if anything goes wrong");
        }

        [TestMethod]
        public void TestCalculateMaxPP()
        {
            double result = curve.CalculateMaxPP(beatMapInfo);
            bool isInRange = result < 5017 && result > 5014;
            Assert.IsTrue(isInRange);
        }
    }
}
