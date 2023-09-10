using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using PPPredictor.Data;
using PPPredictor.Utilities;

namespace UnitTest.Data
{
    [TestClass]
    public class TestProfileInfo
    {
        [TestMethod]
        public void DefaultConstuctor()
        {
            ProfileInfo profileInfo = new ProfileInfo();
            Assert.IsNotNull(profileInfo.LsLeaderboardInfo);
            Assert.IsTrue(profileInfo.LastPercentageSelected == 90, "Percentage should be 90");
            Assert.IsTrue(profileInfo.Position.Equals(MenuPositionHelper.UnderScoreboardPosition), "Position should be under Scoreboard");
            Assert.IsTrue(profileInfo.EulerAngles.Equals(MenuPositionHelper.UnderScoreboardEulerAngles), "Position should be under Scoreboard");
            Assert.IsFalse(profileInfo.DisplaySessionValues, "DisplaySessionValues should be false");
            Assert.IsTrue(profileInfo.ResetSessionHours == 12, "ResetSessionHours should be 12");
            Assert.IsNotNull(profileInfo.LastSessionReset, "LastSessionReset should not be null");
            Assert.IsTrue(profileInfo.LastLeaderBoardSelected == Leaderboard.ScoreSaber.ToString(), "LastLeaderBoardSelected should be Scoresaber");
            Assert.IsTrue(profileInfo.CounterDisplayType == CounterDisplayType.PP, "CounterDisplayType should be PP");
            Assert.IsTrue(profileInfo.CounterScoringType == CounterScoringType.Local, "CounterScoringType should be Local");
            Assert.IsTrue(profileInfo.CounterHighlightTargetPercentage, "CounterHighlightTargetPercentage should be true");
            Assert.IsTrue(profileInfo.CounterHideWhenUnranked, "CounterHideWhenUnranked should be true");
            Assert.IsTrue(profileInfo.AcknowledgedVersion == string.Empty, "AcknowledgedVersion should be empty");
            Assert.IsTrue(profileInfo.CounterUseIcons, "CounterUseIcons should be true");
            Assert.IsTrue(profileInfo.CounterUseCustomMapPoolIcons, "CounterUseCustomMapPoolIcons should be true");
            Assert.IsTrue(profileInfo.DtLastVersionCheck == new DateTime(2000, 1, 1), "DtLastVersionCheck should be 2000");
            Assert.IsTrue(profileInfo.IsVersionCheckEnabled, "IsVersionCheckEnabled should be true");
            Assert.IsTrue(profileInfo.IsScoreSaberEnabled, "IsScoreSaberEnabled should be true");
            Assert.IsTrue(profileInfo.IsBeatLeaderEnabled, "IsBeatLeaderEnabled should be true");
            Assert.IsTrue(profileInfo.IsHitBloqEnabled, "IsHitBloqEnabled should be true");
            Assert.IsTrue(profileInfo.PpGainCalculationType == PPGainCalculationType.Weighted, "PpGainCalculationType should be Weighted");
            Assert.IsTrue(profileInfo.RawPPLossHighlightThreshold == -10, "RawPPLossHighlightThreshold should be -10");
            Assert.IsTrue(profileInfo.ProfileInfoVersion == 0, "ProfileInfoVersion should be 0");
            Assert.IsTrue(profileInfo.SelectedTab == 0, "SelectedTab should be 0");
            Assert.IsTrue(profileInfo.HitbloqMapPoolSorting == MapPoolSorting.Popularity, "HitbloqMapPoolSorting should be Popularity");
            Assert.IsTrue(profileInfo.IsPredictorSwitchBySyncUrlEnabled, "IsPredictorSwitchBySyncUrlEnabled should be true");
        }

        [TestMethod]
        public void ClearOldMapInfos()
        {
            ProfileInfo profileInfo = new ProfileInfo();
            PPPLeaderboardInfo leaderboardInfo = new PPPLeaderboardInfo(Leaderboard.ScoreSaber);
            PPPMapPool mapPool = new PPPMapPool();
            mapPool.LsLeaderboadInfo.Add(new ShortScore("TESTNOW", new PPPStarRating(), DateTime.Now));
            mapPool.LsLeaderboadInfo.Add(new ShortScore("TESTOLDER", new PPPStarRating(), DateTime.Now.AddDays(ProfileInfo.RefetchMapInfoAfterDays * 2)));
            leaderboardInfo.LsMapPools.Add(mapPool);
            profileInfo.LsLeaderboardInfo.Add(leaderboardInfo);
            int count = profileInfo.LsLeaderboardInfo.Sum(x => x.LsMapPools.Sum(y => y.LsLeaderboadInfo.Count()));
            Assert.IsTrue(count == 2, "Two scores should exist");
            profileInfo.ClearOldMapInfos();
            count = profileInfo.LsLeaderboardInfo.Sum(x => x.LsMapPools.Sum(y => y.LsLeaderboadInfo.Count()));
            Assert.IsTrue(count == 1, "One scores should exist after deleting the old one");
            profileInfo.LsLeaderboardInfo.ForEach(l => l.LsMapPools.ForEach(m => m.LsLeaderboadInfo.ForEach(li => Assert.IsTrue(li.Searchstring == "TESTNOW", "Only 'TestNow' should still exist"))));
        }

        [TestMethod]
        public void ResetCachedData()
        {
            ProfileInfo profileInfo = new ProfileInfo();
            PPPLeaderboardInfo leaderboardInfo = new PPPLeaderboardInfo(Leaderboard.ScoreSaber);
            PPPMapPool mapPool = new PPPMapPool();
            mapPool.LsLeaderboadInfo.Add(new ShortScore("TESTNOW1", new PPPStarRating(), DateTime.Now));
            mapPool.LsLeaderboadInfo.Add(new ShortScore("TESTNOW2", new PPPStarRating(), DateTime.Now));
            mapPool.LsScores.Add(new ShortScore("TESTNOW3", new PPPStarRating(), DateTime.Now));
            mapPool.LsScores.Add(new ShortScore("TESTNOW4", new PPPStarRating(), DateTime.Now));
            mapPool.DtLastScoreSet = new DateTime(2023, 1, 1);
            leaderboardInfo.LsMapPools.Clear();
            leaderboardInfo.LsMapPools.Add(mapPool);
            profileInfo.LsLeaderboardInfo.Add(leaderboardInfo);

            int countLsLeaderboadInfo = profileInfo.LsLeaderboardInfo.Sum(x => x.LsMapPools.Sum(y => y.LsLeaderboadInfo.Count()));
            int countLsScoresInfo = profileInfo.LsLeaderboardInfo.Sum(x => x.LsMapPools.Sum(y => y.LsScores.Count()));
            Assert.IsTrue(countLsLeaderboadInfo == 2, "Two scores on LsLeaderboadInfo should exist");
            Assert.IsTrue(countLsScoresInfo == 2, "Two scores on LsScores should exist");
            profileInfo.LsLeaderboardInfo.ForEach(l => l.LsMapPools.ForEach(m => Assert.IsTrue(m.DtLastScoreSet == new DateTime(2023, 1, 1), "DtLastScoreSet should be 2023")));
            
            profileInfo.ResetCachedData();
            countLsLeaderboadInfo = profileInfo.LsLeaderboardInfo.Sum(x => x.LsMapPools.Sum(y => y.LsLeaderboadInfo.Count()));
            countLsScoresInfo = profileInfo.LsLeaderboardInfo.Sum(x => x.LsMapPools.Sum(y => y.LsScores.Count()));
            Assert.IsTrue(countLsLeaderboadInfo == 0, "Zero scores on LsLeaderboadInfo should exist");
            Assert.IsTrue(countLsScoresInfo == 0, "Zero scores on LsScores should exist");
            profileInfo.LsLeaderboardInfo.ForEach(l => l.LsMapPools.ForEach(m => Assert.IsTrue(m.DtLastScoreSet == new DateTime(2000, 1, 1), "DtLastScoreSet should be 2023")));
        }
    }
}
