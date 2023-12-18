using PPPredictor.Data;
using PPPredictor.Utilities;
using static PPPredictor.Data.LeaderBoardDataTypes.BeatLeaderDataTypes;

namespace UnitTests.Data
{
    [TestClass]
    public class TestPPPStarRating
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            PPPStarRating starRating = new PPPStarRating();
            Assert.AreEqual(starRating.Stars, 0);
            Assert.AreEqual(starRating.PredictedAcc, 0);
            Assert.AreEqual(starRating.PassRating, 0);
            Assert.AreEqual(starRating.AccRating, 0);
            Assert.AreEqual(starRating.TechRating, 0);
            Assert.IsNotNull(starRating.ModifiersRating);
            Assert.IsNotNull(starRating.ModifierValues);
            Assert.AreEqual(starRating.Multiplier, 1);
            Assert.IsFalse(starRating.IsRanked());

            starRating.Stars = 1;
            starRating.PredictedAcc = 1;
            starRating.PassRating = 1;
            starRating.AccRating = 1;
            starRating.TechRating = 1;
            starRating.ModifiersRating = null;
            starRating.ModifierValues = null;
            starRating.Multiplier = 2;

            Assert.AreNotEqual(starRating.Stars, 0);
            Assert.AreNotEqual(starRating.PredictedAcc, 0);
            Assert.AreNotEqual(starRating.PassRating, 0);
            Assert.AreNotEqual(starRating.AccRating, 0);
            Assert.AreNotEqual(starRating.TechRating, 0);
            Assert.IsNull(starRating.ModifiersRating);
            Assert.IsNull(starRating.ModifierValues);
            Assert.AreNotEqual(starRating.Multiplier, 1);
            Assert.IsTrue(starRating.IsRanked());
        }

        [TestMethod]
        public void StarRatingConstructor()
        {
            int star = 5;
            PPPStarRating starRating = new PPPStarRating(star);
            Assert.AreEqual(starRating.Stars, star);
            Assert.AreEqual(starRating.PredictedAcc, star);
            Assert.AreEqual(starRating.PassRating, star);
            Assert.AreEqual(starRating.AccRating, star);
            Assert.AreEqual(starRating.TechRating, star);
        }

        [TestMethod]
        public void BeatLeaderDifficultyConstructor()
        {
            double passRating = 7;
            double accRating = 8;
            double techRating = 9;
            double predictedAcc = 10;
            BeatLeaderDifficulty beatLeaderDifficulty = new BeatLeaderDifficulty();
            PPPStarRating starRating = new PPPStarRating(beatLeaderDifficulty);
            Assert.AreEqual(starRating.Stars, 0);
            Assert.AreEqual(starRating.PredictedAcc, 0);
            Assert.AreEqual(starRating.PassRating, 0);
            Assert.AreEqual(starRating.AccRating, 0);
            Assert.AreEqual(starRating.TechRating, 0);
            Assert.IsNull(starRating.ModifiersRating);
            Assert.IsNull(starRating.ModifierValues);

            //Unranked
            beatLeaderDifficulty = new BeatLeaderDifficulty() {
                predictedAcc = predictedAcc,
                passRating = passRating,
                accRating = accRating,
                techRating = techRating,
                modifiersRating = new Dictionary<string, double>(),
                modifierValues = new Dictionary<string, double>(),
                status = (int)BeatLeaderDifficultyStatus.unranked
            };
            starRating = new PPPStarRating(beatLeaderDifficulty);

            Assert.AreEqual(starRating.Stars, 0);
            Assert.AreEqual(starRating.PredictedAcc, predictedAcc);
            Assert.AreEqual(starRating.PassRating, passRating);
            Assert.AreEqual(starRating.AccRating, accRating);
            Assert.AreEqual(starRating.TechRating, techRating);
            Assert.IsNotNull(starRating.ModifiersRating);
            Assert.IsNotNull(starRating.ModifierValues);
            Assert.IsFalse(starRating.IsRanked()); //Status is unranked, so should be unranked

            //Ranked
            beatLeaderDifficulty = new BeatLeaderDifficulty()
            {
                predictedAcc = predictedAcc,
                passRating = passRating,
                accRating = accRating,
                techRating = techRating,
                modifiersRating = new Dictionary<string, double>(),
                modifierValues = new Dictionary<string, double>(),
                status = (int)BeatLeaderDifficultyStatus.ranked
            };
            starRating = new PPPStarRating(beatLeaderDifficulty);

            Assert.AreEqual(starRating.Stars, 0);
            Assert.AreEqual(starRating.PredictedAcc, predictedAcc);
            Assert.AreEqual(starRating.PassRating, passRating);
            Assert.AreEqual(starRating.AccRating, accRating);
            Assert.AreEqual(starRating.TechRating, techRating);
            Assert.IsNotNull(starRating.ModifiersRating);
            Assert.IsNotNull(starRating.ModifierValues);
            Assert.IsTrue(starRating.IsRanked()); //Status is ranked, so should be ranked
        }

        [TestMethod]
        public void MultiStarRatingConstructor()
        {
            double star = 0;
            double multi = 6;
            double passRating = 7;
            double accRating = 8;
            double techRating = 9;
            PPPStarRating starRating = new PPPStarRating(multi, accRating, passRating, techRating, true);
            Assert.AreEqual(starRating.Stars, star);
            Assert.AreEqual(starRating.Multiplier, multi);
            Assert.AreEqual(starRating.PassRating, passRating);
            Assert.AreEqual(starRating.AccRating, accRating);
            Assert.AreEqual(starRating.TechRating, techRating);
        }
    }
}

