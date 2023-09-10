using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPPredictor.Data;
using System.Collections.Generic;
using static PPPredictor.Data.LeaderBoardDataTypes.BeatLeaderDataTypes;
using static PPPredictor.Data.LeaderBoardDataTypes.HitBloqDataTypes;
using static PPPredictor.Data.LeaderBoardDataTypes.ScoreSaberDataTypes;

namespace UnitTest.Data
{
    [TestClass]
    public class TestPPPScoreCollection
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            PPPScoreCollection pPPScoreCollection = new PPPScoreCollection();
            Assert.IsNotNull(pPPScoreCollection.LsPPPScore, "LsPPPScore should exist");
            Assert.IsTrue(pPPScoreCollection.LsPPPScore.Count == 0, "LsPPPScore should be empty");
            Assert.IsTrue(pPPScoreCollection.Page == -1, "Page should be -1");
            Assert.IsTrue(pPPScoreCollection.ItemsPerPage == -1, "ItemsPerPage should be -1");
            Assert.IsTrue(pPPScoreCollection.Total == -1, "Total should be -1");
        }

        [TestMethod]
        public void ScoreSaberPlayerScoreListConstructor()
        {
            ScoreSaberPlayerScoreList scoreList = new ScoreSaberPlayerScoreList();
            scoreList.metadata = new ScoreSaberScoreListMetadata();
            scoreList.metadata.page = 1;
            scoreList.metadata.itemsPerPage = 2;
            scoreList.metadata.total = 3;
            scoreList.playerScores = new List<ScoreSaberPlayerScore>
            {
                new ScoreSaberPlayerScore(),
                new ScoreSaberPlayerScore()
            };
            PPPScoreCollection pPPScoreCollection = new PPPScoreCollection(scoreList);
            Assert.IsNotNull(pPPScoreCollection.LsPPPScore, "LsPPPScore should exist");
            Assert.IsTrue(pPPScoreCollection.LsPPPScore.Count == scoreList.playerScores.Count, "LsPPPScore length should match");
            Assert.IsTrue(pPPScoreCollection.Page == 1, "Page should be 1");
            Assert.IsTrue(pPPScoreCollection.ItemsPerPage == 2, "ItemsPerPage should be 2");
            Assert.IsTrue(pPPScoreCollection.Total == 3, "Total should be 3");
        }

        [TestMethod]
        public void BeatLeaderPlayerScoreListConstructor()
        {
            BeatLeaderPlayerScoreList scoreList = new BeatLeaderPlayerScoreList();
            scoreList.metadata = new BeatLeaderPlayerScoreListMetaData();
            scoreList.metadata.page = 1;
            scoreList.metadata.itemsPerPage = 2;
            scoreList.metadata.total = 3;
            scoreList.data = new List<BeatLeaderPlayerScore>
            {
                new BeatLeaderPlayerScore(),
                new BeatLeaderPlayerScore()
            };
            PPPScoreCollection pPPScoreCollection = new PPPScoreCollection(scoreList);
            Assert.IsNotNull(pPPScoreCollection.LsPPPScore, "LsPPPScore should exist");
            Assert.IsTrue(pPPScoreCollection.LsPPPScore.Count == scoreList.data.Count, "LsPPPScore length should match");
            Assert.IsTrue(pPPScoreCollection.Page == 1, "Page should be 1");
            Assert.IsTrue(pPPScoreCollection.ItemsPerPage == 2, "ItemsPerPage should be 2");
            Assert.IsTrue(pPPScoreCollection.Total == 3, "Total should be 3");
        }

        [TestMethod]
        public void HitBloqScoresConstructor()
        {
            List<HitBloqScores> scoreList = new List<HitBloqScores>
            {
                new HitBloqScores(),
                new HitBloqScores()
            };
            PPPScoreCollection pPPScoreCollection = new PPPScoreCollection(scoreList, 1);
            Assert.IsNotNull(pPPScoreCollection.LsPPPScore, "LsPPPScore should exist");
            Assert.IsTrue(pPPScoreCollection.LsPPPScore.Count == scoreList.Count, "LsPPPScore length should match");
            Assert.IsTrue(pPPScoreCollection.Page == 1, "Page should be 1");
            Assert.IsTrue(pPPScoreCollection.ItemsPerPage == 10, "ItemsPerPage should be 2");
            Assert.IsTrue(pPPScoreCollection.Total == 11, "Total should be 11");
            pPPScoreCollection = new PPPScoreCollection(new List<HitBloqScores>(), 1);
            Assert.IsTrue(pPPScoreCollection.Total == 0, "Total should be 0");
        }
    }
}
