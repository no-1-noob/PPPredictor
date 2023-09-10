using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPPredictor.Data;
using System;

namespace UnitTest.Data
{
    [TestClass]
    public class TestShortScore
    {
        private readonly string testSearchstring = "#SearchString";
        private readonly double testPp = 123;
        private readonly DateTime testFetchTime = new DateTime(2023, 1, 1);
        private readonly PPPStarRating testStarRating = new PPPStarRating(5);

        [TestMethod]
        public void TupelConstructor()
        {
            ShortScore score = new ShortScore(testSearchstring, testPp);
            Assert.IsNotNull(score.Searchstring);
            Assert.IsNotNull(score.Pp);
            Assert.IsNotNull(score.StarRating);
            Assert.IsNotNull(score.FetchTime);
            Assert.IsTrue(score.StarRating.Stars == 0, "StarRating should be 0");
            Assert.IsTrue(score.Pp == testPp, "StarRating should be testPp");
            Assert.IsTrue(score.Searchstring == testSearchstring.ToUpper(), "StarRating should be testSearchstring");
            Assert.IsFalse(score.FetchTime == testFetchTime, "FetchTime should not be be testFetchTime");
            Assert.IsFalse(score.ShouldSerializeStarRating(), "ShouldSerializeStarRating should be false");
            score.Pp = 234;
            score.StarRating = testStarRating;
            score.FetchTime = testFetchTime;
            Assert.IsFalse(score.StarRating.Stars == 0, "StarRating should not be 0");
            Assert.IsFalse(score.Pp == testPp, "StarRating should not be testPp");
            Assert.IsTrue(score.FetchTime == testFetchTime, "FetchTime should not be testFetchTime");
            Assert.IsTrue(score.ShouldSerializeStarRating(), "ShouldSerializeStarRating should be true");
        }

        [TestMethod]
        public void TripleConstructor()
        {
            ShortScore score = new ShortScore(testSearchstring, testStarRating, testFetchTime);
            Assert.IsTrue(score.StarRating.Stars == 5, "StarRating should be 5");
            Assert.IsTrue(score.Pp == 0, "StarRating should be 0");
            Assert.IsTrue(score.Searchstring == testSearchstring.ToUpper(), "StarRating should be testSearchstring");
            Assert.IsTrue(score.FetchTime == testFetchTime, "FetchTime should be testFetchTime");
        }

        [TestMethod]
        public void QuadConstructor()
        {
            ShortScore score = new ShortScore(testSearchstring, testPp, testStarRating, testFetchTime);
            Assert.IsTrue(score.StarRating.Stars == 5, "StarRating should be 5");
            Assert.IsTrue(score.Pp == testPp, "StarRating should be testPp");
            Assert.IsTrue(score.Searchstring == testSearchstring.ToUpper(), "StarRating should be testSearchstring");
            Assert.IsTrue(score.FetchTime == testFetchTime, "FetchTime should be testFetchTime");
            score = new ShortScore(testSearchstring, testPp, null, testFetchTime);
            Assert.IsTrue(score.StarRating.Stars == 0, "StarRating should be 0");
        }
    }
}
