using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPPredictor.Data;
using PPPredictor.Utilities;
using System;
using static PPPredictor.Data.LeaderBoardDataTypes.BeatLeaderDataTypes;
using static PPPredictor.Data.LeaderBoardDataTypes.HitBloqDataTypes;
using static PPPredictor.Data.LeaderBoardDataTypes.ScoreSaberDataTypes;

namespace UnitTest.Data
{
    [TestClass]
    public class TestPPPScore
    {
        private DateTimeOffset dtTest = new DateTimeOffset(new DateTime(2023, 1, 1));
        private float testPP = 123.45f;
        private string testHash = "#Hashtag;)";
        private int testDifficulty = 7;
        private string testDifficultyString = "ex";
        private string testGameModeShort = "s";
        private string testGameMode = "SoloStandard";
        private string testGameModeBeatLeader = "Standard";

        [TestMethod]
        public void ScoreSaberPlayerScoreConstructor()
        {
            ScoreSaberPlayerScore playerScore = new ScoreSaberPlayerScore();
            playerScore.score = new ScoreSaberScore();
            playerScore.score.timeSet = dtTest;
            playerScore.score.pp = testPP;
            playerScore.leaderboard = new ScoreSaberLeaderboardInfo();
            playerScore.leaderboard.songHash = testHash;
            playerScore.leaderboard.difficulty.difficulty = testDifficulty;
            playerScore.leaderboard.difficulty.gameMode = testGameMode;
            PPPScore score = new PPPScore(playerScore);

            Assert.IsTrue(score.TimeSet == dtTest, "Date should be set");
            Assert.IsTrue(score.Pp == testPP, "Pp should match");
            Assert.IsTrue(score.SongHash == testHash, "SongHash should match");
            Assert.IsTrue(score.Difficulty1 == testDifficulty, "Difficulty1 should match");
            Assert.IsTrue(score.GameMode == testGameMode, "GameMode should match");
        }

        [TestMethod]
        public void BeatLeaderPlayerScoreConstructor()
        {
            BeatLeaderPlayerScore playerScore = new BeatLeaderPlayerScore();
            playerScore.timeset = ((int)dtTest.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString();
            playerScore.pp = testPP;
            playerScore.leaderboard = new BeatLeaderLeaderboard();
            playerScore.leaderboard.difficulty = new BeatLeaderDifficulty();
            playerScore.leaderboard.difficulty.value = testDifficulty;
            playerScore.leaderboard.difficulty.modeName = testGameModeBeatLeader;
            playerScore.leaderboard.song = new BeatLeaderSong();
            playerScore.leaderboard.song.hash = testHash;
            PPPScore score = new PPPScore(playerScore);

            Assert.IsTrue(score.TimeSet == dtTest, "Date should be set");
            Assert.IsTrue(score.Pp == 0, "Pp should be 0 when unranked");
            Assert.IsTrue(score.SongHash == testHash, "SongHash should match");
            Assert.IsTrue(score.Difficulty1 == testDifficulty, "Difficulty1 should match");
            Assert.IsTrue(score.GameMode == testGameMode, "GameMode should match");

            playerScore.leaderboard.difficulty.status = (int)BeatLeaderDifficultyStatus.ranked;
            score = new PPPScore(playerScore);
            Assert.IsTrue(score.Pp == testPP, "Pp should match when ranked");
        }

        [TestMethod]
        public void HitBloqScoresConstructor()
        {
            HitBloqScores playerScore = new HitBloqScores();
            playerScore.time = ((long)dtTest.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            playerScore.cr_received = testPP;
            playerScore.song_id = $"{testHash}_{testDifficultyString}_{testGameModeShort}";
            PPPScore score = new PPPScore(playerScore);

            Assert.IsTrue(score.TimeSet == dtTest, "Date should be set");
            Assert.IsTrue(score.Pp == testPP, "Pp should match");
            Assert.IsTrue(score.SongHash == testHash, "SongHash should match");
            Assert.IsTrue(score.Difficulty1 == testDifficulty, "Difficulty1 should match");
            Assert.IsTrue(score.GameMode == testGameMode, "GameMode should match");
        }
    }
}
