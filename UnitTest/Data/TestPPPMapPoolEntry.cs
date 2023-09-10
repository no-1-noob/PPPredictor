using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPPredictor.Data;
using PPPredictor.Utilities;
using static PPPredictor.Data.LeaderBoardDataTypes.BeatLeaderDataTypes;

namespace UnitTest.Data
{
    [TestClass]
    public class TestPPPMapPoolEntry
    {
        private readonly string testHash = "#Test";

        [TestMethod]
        public void DefaultConstructor()
        {
            PPPMapPoolEntry mapPoolEntry = new PPPMapPoolEntry();
            Assert.IsNotNull(mapPoolEntry.Searchstring);
            Assert.IsTrue(mapPoolEntry.Searchstring == string.Empty, "Searchstring is string.Empty");
            mapPoolEntry.Searchstring = testHash;
            Assert.IsTrue(mapPoolEntry.Searchstring == testHash, "Searchstring is testHash");
        }

        [TestMethod]
        public void SingleConstructor()
        {
            PPPMapPoolEntry mapPoolEntry = new PPPMapPoolEntry(testHash);
            Assert.IsNotNull(mapPoolEntry.Searchstring);
            Assert.IsTrue(mapPoolEntry.Searchstring == testHash, "Searchstring is testHash");
        }

        [TestMethod]
        public void TupelConstructor()
        {
            BeatLeaderPlayListSong song = new BeatLeaderPlayListSong() { hash = testHash };
            BeatLeaderPlayListDifficulties diff = new BeatLeaderPlayListDifficulties() { name = PPPBeatMapDifficulty.ExpertPlus};
            PPPMapPoolEntry mapPoolEntry = new PPPMapPoolEntry(song, diff);
            Assert.IsNotNull(mapPoolEntry.Searchstring);
            Assert.IsTrue(mapPoolEntry.Searchstring == $"{testHash}_{(int)PPPBeatMapDifficulty.ExpertPlus}", "Searchstring is correct");
        }
    }
}
