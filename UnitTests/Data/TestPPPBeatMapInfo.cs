using PPPredictor.Data;

namespace UnitTests.Data
{
    [TestClass]
    public class TestPPPBeatMapInfo
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            PPPBeatMapInfo pPPBeatMapInfo = new PPPBeatMapInfo();
            Assert.IsNotNull(pPPBeatMapInfo.BaseStarRating, "BaseStarRating should be set");
            Assert.IsNotNull(pPPBeatMapInfo.ModifiedStarRating, "ModifiedStarRating should be set");
            Assert.IsNull(pPPBeatMapInfo.SelectedCustomBeatmapLevel, "SelectedCustomBeatmapLevel should not be set");
            Assert.IsNull(pPPBeatMapInfo.Beatmap, "Beatmap should not be set");
            Assert.IsNull(pPPBeatMapInfo.SelectedMapSearchString, "SelectedMapSearchString should not be set");

            Assert.IsTrue(pPPBeatMapInfo.MaxPP == -1, "MaxPPS should be -1");
            Assert.IsTrue(pPPBeatMapInfo.BaseStarRating.Stars == 0, "MaxPPS should be 0");
            Assert.IsTrue(pPPBeatMapInfo.ModifiedStarRating.Stars == 0, "MaxPPS should be 0");

            pPPBeatMapInfo.BaseStarRating = new PPPStarRating(1);
            pPPBeatMapInfo.ModifiedStarRating = new PPPStarRating(2);
            pPPBeatMapInfo.SelectedCustomBeatmapLevel = TestUtils.TestUtils.CreateCustomBeatmapLevel();
            pPPBeatMapInfo.Beatmap = TestUtils.TestUtils.CreateCustomDifficultyBeatmap();
            pPPBeatMapInfo.MaxPP = 123;
            pPPBeatMapInfo.SelectedMapSearchString = "Test";

            Assert.IsTrue(pPPBeatMapInfo.BaseStarRating.Stars == 1, "MaxPPS should be 1");
            Assert.IsTrue(pPPBeatMapInfo.ModifiedStarRating.Stars == 2, "MaxPPS should be 2");
            Assert.IsNotNull(pPPBeatMapInfo.SelectedCustomBeatmapLevel, "SelectedCustomBeatmapLevel should not be null");
            Assert.IsNotNull(pPPBeatMapInfo.Beatmap, "Beatmap should not be null");
            Assert.IsTrue(pPPBeatMapInfo.MaxPP == 123, "MaxPPS should be 123");
            Assert.IsTrue(pPPBeatMapInfo.SelectedMapSearchString == "Test", "SelectedMapSearchString should be 'Test'");
        }

        [TestMethod]
        public void BaseStarsConstructor()
        {
            int startRating = 5;
            PPPBeatMapInfo pPPBeatMapInfo = new PPPBeatMapInfo(new PPPBeatMapInfo(), new PPPStarRating(startRating));
            Assert.IsNotNull(pPPBeatMapInfo.BaseStarRating, "BaseStarRating should be set");
            Assert.IsNotNull(pPPBeatMapInfo.ModifiedStarRating, "ModifiedStarRating should be set");
            Assert.IsNull(pPPBeatMapInfo.SelectedCustomBeatmapLevel, "SelectedCustomBeatmapLevel should not be set");
            Assert.IsNull(pPPBeatMapInfo.Beatmap, "Beatmap should not be set");
            Assert.IsNull(pPPBeatMapInfo.SelectedMapSearchString, "SelectedMapSearchString should not be set");

            Assert.IsTrue(pPPBeatMapInfo.MaxPP == -1, "MaxPPS should be -1");
            Assert.IsTrue(pPPBeatMapInfo.BaseStarRating.Stars == startRating, "MaxPPS should be startRating");
            Assert.IsTrue(pPPBeatMapInfo.ModifiedStarRating.Stars == startRating, "MaxPPS should be startRating");
        }

        [TestMethod]
        public void BeatmapConstructor()
        {
            PPPBeatMapInfo pPPBeatMapInfo = new PPPBeatMapInfo(TestUtils.TestUtils.CreateCustomBeatmapLevel(), TestUtils.TestUtils.CreateCustomDifficultyBeatmap());
            Assert.IsNotNull(pPPBeatMapInfo.BaseStarRating, "BaseStarRating should be set");
            Assert.IsNotNull(pPPBeatMapInfo.ModifiedStarRating, "ModifiedStarRating should be set");
            Assert.IsNotNull(pPPBeatMapInfo.SelectedCustomBeatmapLevel, "SelectedCustomBeatmapLevel should be set");
            Assert.IsNotNull(pPPBeatMapInfo.Beatmap, "Beatmap should be set");
            Assert.IsNull(pPPBeatMapInfo.SelectedMapSearchString, "SelectedMapSearchString should not be set");

            Assert.IsTrue(pPPBeatMapInfo.MaxPP == -1, "MaxPPS should be -1");
            Assert.IsTrue(pPPBeatMapInfo.BaseStarRating.Stars == 0, "MaxPPS should be startRating");
            Assert.IsTrue(pPPBeatMapInfo.ModifiedStarRating.Stars == 0, "MaxPPS should be startRating");
        }
    }
}
