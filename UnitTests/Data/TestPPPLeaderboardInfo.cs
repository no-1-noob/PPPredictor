using PPPredictor.Data;
using PPPredictor.Data.Curve;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Data
{
    [TestClass]
    public class TestPPPLeaderboardInfo
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            PPPLeaderboardInfo leaderboardInfo = new PPPLeaderboardInfo();
            Assert.IsNull(leaderboardInfo.LsMapPools);
            Assert.IsNull(leaderboardInfo.LeaderboardName);
            Assert.IsNull(leaderboardInfo.LeaderboardIcon);
            Assert.IsNull(leaderboardInfo.CurrentMapPool);
            Assert.IsNull(leaderboardInfo.LastSelectedMapPoolId);
            Assert.IsNull(leaderboardInfo.CustomLeaderboardUserId);
            Assert.IsNull(leaderboardInfo.PpSuffix);
            Assert.AreEqual(leaderboardInfo.LeaderboardFirstPageIndex, 0);
            Assert.AreEqual(leaderboardInfo.IsCountryRankEnabled, false);
            Assert.ThrowsException<NullReferenceException>(() => leaderboardInfo.DefaultMapPool);

            leaderboardInfo.LsMapPools = new List<PPPMapPool>();
            leaderboardInfo.LeaderboardName = "Name";
            leaderboardInfo.LeaderboardIcon = "Icon";
            leaderboardInfo.CurrentMapPool = new PPPMapPool();
            leaderboardInfo.CustomLeaderboardUserId = "1";
            leaderboardInfo.PpSuffix = "pp";
            leaderboardInfo.LeaderboardFirstPageIndex = 1;
            leaderboardInfo.IsCountryRankEnabled = true;

            Assert.IsNotNull(leaderboardInfo.LsMapPools);
            Assert.IsNotNull(leaderboardInfo.LeaderboardName);
            Assert.IsNotNull(leaderboardInfo.LeaderboardIcon);
            Assert.IsNotNull(leaderboardInfo.CurrentMapPool);
            Assert.IsNotNull(leaderboardInfo.LastSelectedMapPoolId);
            Assert.IsNotNull(leaderboardInfo.CustomLeaderboardUserId);
            Assert.IsNotNull(leaderboardInfo.PpSuffix);
            Assert.AreNotEqual(leaderboardInfo.LeaderboardFirstPageIndex, 0);
            Assert.AreNotEqual(leaderboardInfo.IsCountryRankEnabled, false);
            Assert.IsNull(leaderboardInfo.DefaultMapPool);
            leaderboardInfo.LsMapPools.Add(new PPPMapPool(PPPredictor.Utilities.MapPoolType.Default, "", 0, 0, new BeatLeaderPPPCurve()));
            Assert.IsNotNull(leaderboardInfo.DefaultMapPool);

            leaderboardInfo.LastSelectedMapPoolId = "5";
            Assert.AreEqual(leaderboardInfo.LastSelectedMapPoolId, "5");
        }

        [TestMethod]
        public void ScoreSaberConstructor()
        {
            PPPLeaderboardInfo leaderboardInfo = new PPPLeaderboardInfo(Leaderboard.ScoreSaber);
            Assert.AreEqual(leaderboardInfo.LeaderboardName, Leaderboard.ScoreSaber.ToString());
            Assert.IsNotNull(leaderboardInfo.LsMapPools);
            Assert.AreEqual(leaderboardInfo.LsMapPools.Count, 1);
            Assert.AreEqual(leaderboardInfo.CustomLeaderboardUserId, string.Empty);
            Assert.AreEqual(leaderboardInfo.PpSuffix, "pp");
            Assert.AreEqual(leaderboardInfo.LeaderboardFirstPageIndex, 1);
            Assert.IsTrue(leaderboardInfo.IsCountryRankEnabled);
            Assert.IsNotNull(leaderboardInfo.LeaderboardIcon);
            Assert.IsNotNull(leaderboardInfo.CurrentMapPool);
        }

        [TestMethod]
        public void BeatLeaderConstructor()
        {
            PPPLeaderboardInfo leaderboardInfo = new PPPLeaderboardInfo(Leaderboard.BeatLeader);
            Assert.AreEqual(leaderboardInfo.LeaderboardName, Leaderboard.BeatLeader.ToString());
            Assert.IsNotNull(leaderboardInfo.LsMapPools);
            Assert.AreEqual(leaderboardInfo.LsMapPools.Count, 4);
            Assert.AreEqual(leaderboardInfo.CustomLeaderboardUserId, string.Empty);
            Assert.AreEqual(leaderboardInfo.PpSuffix, "pp");
            Assert.AreEqual(leaderboardInfo.LeaderboardFirstPageIndex, 1);
            Assert.IsTrue(leaderboardInfo.IsCountryRankEnabled);
            Assert.IsNotNull(leaderboardInfo.LeaderboardIcon);
            Assert.IsNotNull(leaderboardInfo.CurrentMapPool);
        }

        [TestMethod]
        public void NoLeaderboardConstructor()
        {
            PPPLeaderboardInfo leaderboardInfo = new PPPLeaderboardInfo(Leaderboard.NoLeaderboard);
            Assert.AreEqual(leaderboardInfo.LeaderboardName, Leaderboard.NoLeaderboard.ToString());
            Assert.IsNotNull(leaderboardInfo.LsMapPools);
            Assert.AreEqual(leaderboardInfo.LsMapPools.Count, 1);
            Assert.AreEqual(leaderboardInfo.CustomLeaderboardUserId, string.Empty);
            Assert.AreEqual(leaderboardInfo.PpSuffix, "pp");
            Assert.AreEqual(leaderboardInfo.LeaderboardFirstPageIndex, 1);
            Assert.IsTrue(leaderboardInfo.IsCountryRankEnabled);
            Assert.IsNotNull(leaderboardInfo.LeaderboardIcon);
            Assert.IsNotNull(leaderboardInfo.CurrentMapPool);
        }

        [TestMethod]
        public void HitBloqConstructor()
        {
            PPPLeaderboardInfo leaderboardInfo = new PPPLeaderboardInfo(Leaderboard.HitBloq);
            Assert.AreEqual(leaderboardInfo.LeaderboardName, Leaderboard.HitBloq.ToString());
            Assert.IsNotNull(leaderboardInfo.LsMapPools);
            Assert.AreEqual(leaderboardInfo.LsMapPools.Count, 1);
            Assert.AreEqual(leaderboardInfo.CustomLeaderboardUserId, string.Empty);
            Assert.AreEqual(leaderboardInfo.PpSuffix, "cr");
            Assert.AreEqual(leaderboardInfo.LeaderboardFirstPageIndex, 0);
            Assert.IsFalse(leaderboardInfo.IsCountryRankEnabled);
            Assert.IsNotNull(leaderboardInfo.LeaderboardIcon);
            Assert.IsNotNull(leaderboardInfo.CurrentMapPool);
        }

        [TestMethod]
        public void TestSetCurrentMapPool()
        {
            PPPLeaderboardInfo leaderboardInfo = new PPPLeaderboardInfo();
            leaderboardInfo.SetCurrentMapPool();
            Assert.IsNull(leaderboardInfo.CurrentMapPool);
        }
    }
}
