using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPPredictor.Data;
using static PPPredictor.Data.LeaderBoardDataTypes.BeatLeaderDataTypes;
using static PPPredictor.Data.LeaderBoardDataTypes.HitBloqDataTypes;
using static PPPredictor.Data.LeaderBoardDataTypes.ScoreSaberDataTypes;

namespace UnitTest.Data
{
    [TestClass]
    public class TestPPPlayer
    {
        [TestMethod]
        public void DefaultConstuctor()
        {
            PPPPlayer ppPlayer = new PPPPlayer();
            Assert.IsTrue(ppPlayer.Rank == 0, "RankGlobal should be 0");
            Assert.IsTrue(ppPlayer.CountryRank == 0, "CountryRank should be 0");
            Assert.IsTrue(ppPlayer.Pp == 0, "Pp should be 0");
            Assert.IsNull(ppPlayer.Country, "Coutry should be null");
            Assert.IsFalse(ppPlayer.IsErrorUser, "IsErrorUser should be false");
        }
        [TestMethod]
        public void IsErrorUserConstuctor()
        {
            PPPPlayer ppPlayer = new PPPPlayer(true);
            Assert.IsTrue(ppPlayer.Rank == 0, "RankGlobal should be 0");
            Assert.IsTrue(ppPlayer.CountryRank == 0, "CountryRank should be 0");
            Assert.IsTrue(ppPlayer.Pp == 0, "Pp should be 0");
            Assert.IsNull(ppPlayer.Country, "Coutry should be null");
            Assert.IsTrue(ppPlayer.IsErrorUser, "IsErrorUser should be given value");
        }
        [TestMethod]
        public void ScoreSaberPlayerConstuctor()
        {
            ScoreSaberPlayer scoreSaberPlayer = new ScoreSaberPlayer() { rank = 1, countryRank = 2, pp = 3, country = "TestCountry" };
            PPPPlayer ppPlayer = new PPPPlayer(scoreSaberPlayer);
            Assert.IsTrue(ppPlayer.Rank == 1, "RankGlobal should be 1");
            Assert.IsTrue(ppPlayer.CountryRank == 2, "CountryRank should be 2");
            Assert.IsTrue(ppPlayer.Pp == 3, "Pp should be 3");
            Assert.IsTrue(ppPlayer.Country == "TestCountry", "Coutry should be TestCountry");
            Assert.IsFalse(ppPlayer.IsErrorUser, "IsErrorUser should be false");
        }
        [TestMethod]
        public void BeatLeaderPlayerConstuctor()
        {
            BeatLeaderPlayer beatLeaderPlayer = new BeatLeaderPlayer(){ rank = 1, countryRank = 2, pp = 3, country = "TestCountry" };
            PPPPlayer ppPlayer = new PPPPlayer(beatLeaderPlayer);
            Assert.IsTrue(ppPlayer.Rank == 1, "RankGlobal should be 1");
            Assert.IsTrue(ppPlayer.CountryRank == 2, "CountryRank should be 2");
            Assert.IsTrue(ppPlayer.Pp == 3, "Pp should be 3");
            Assert.IsTrue(ppPlayer.Country == "TestCountry", "Coutry should be TestCountry");
            Assert.IsFalse(ppPlayer.IsErrorUser, "IsErrorUser should be false");
        }
        [TestMethod]
        public void BeatLeaderPlayerEventsConstuctor()
        {
            BeatLeaderPlayerEvents beatLeaderPlayerEvents = new BeatLeaderPlayerEvents() { rank = 1, countryRank = 2, pp = 3, country = "TestCountry" };
            PPPPlayer ppPlayer = new PPPPlayer(beatLeaderPlayerEvents);
            Assert.IsTrue(ppPlayer.Rank == 1, "RankGlobal should be 1");
            Assert.IsTrue(ppPlayer.CountryRank == 2, "CountryRank should be 2");
            Assert.IsTrue(ppPlayer.Pp == 3, "Pp should be 3");
            Assert.IsTrue(ppPlayer.Country == "TestCountry", "Coutry should be TestCountry");
            Assert.IsFalse(ppPlayer.IsErrorUser, "IsErrorUser should be false");
        }
        [TestMethod]
        public void HitBloqUserConstuctor()
        {
            HitBloqUser hitBloqUser = new HitBloqUser() { rank = 1, cr = 3 };
            PPPPlayer ppPlayer = new PPPPlayer(hitBloqUser);
            Assert.IsTrue(ppPlayer.Rank == 1, "RankGlobal should be 1");
            Assert.IsTrue(ppPlayer.CountryRank == 0, "CountryRank should be 0");
            Assert.IsTrue(ppPlayer.Pp == 3, "Pp should be 3");
            Assert.IsTrue(ppPlayer.Country == string.Empty, "Coutry should be empty");
            Assert.IsFalse(ppPlayer.IsErrorUser, "IsErrorUser should be false");
        }
        [TestMethod]
        public void PPPlayerToString()
        {
            ScoreSaberPlayer scoreSaberPlayer = new ScoreSaberPlayer() { rank = 1, countryRank = 2, pp = 3, country = "TestCountry" };
            PPPPlayer ppPlayer = new PPPPlayer(scoreSaberPlayer);
            Assert.IsTrue(ppPlayer.ToString() == "PPPPlayer: (Rank): 1 (CountryRank): 2 (PP): 3 (Country): TestCountry", "ToString should match");
        }

        [TestMethod]
        public void PPPlayerTestSetter()
        {
            PPPPlayer ppPlayer = new PPPPlayer();
            ppPlayer.Rank = 1;
            ppPlayer.CountryRank = 2;
            ppPlayer.Pp = 3;
            ppPlayer.Country = "TestCountry";
            Assert.IsTrue(ppPlayer.Rank == 1, "RankGlobal should be 0");
            Assert.IsTrue(ppPlayer.CountryRank == 2, "CountryRank should be 0");
            Assert.IsTrue(ppPlayer.Pp == 3, "Pp should be 0");
            Assert.IsTrue(ppPlayer.Country == "TestCountry", "Coutry should be null");
            Assert.IsFalse(ppPlayer.IsErrorUser, "IsErrorUser should be false");
        }
    }
}
