using System.Collections.Generic;
using static PPPredictor.Core.DataType.LeaderBoard.BeatLeaderDataTypes;
using static PPPredictor.Core.DataType.LeaderBoard.HitBloqDataTypes;
using static PPPredictor.Core.DataType.LeaderBoard.ScoreSaberDataTypes;
using static PPPredictor.Core.DataType.LeaderBoard.AccSaberDataTypes;

namespace PPPredictor.Core.DataType.Score
{
    class PPPScoreCollection
    {
        private readonly List<PPPScore> lsPPPScore = new List<PPPScore>();
        private readonly double page;
        private readonly double itemsPerPage;
        private readonly double total;

        public List<PPPScore> LsPPPScore { get => lsPPPScore; }
        public double Page { get => page; }
        public double ItemsPerPage { get => itemsPerPage; }
        public double Total { get => total; }

        public PPPScoreCollection()
        {
            lsPPPScore = new List<PPPScore>();
            page = -1;
            itemsPerPage = -1;
            total = -1;
        }

        public PPPScoreCollection(ScoreSaberPlayerScoreList scoreSaberPlayerScoreList)
        {
            page = scoreSaberPlayerScoreList.metadata.page;
            itemsPerPage = scoreSaberPlayerScoreList.metadata.itemsPerPage;
            total = scoreSaberPlayerScoreList.metadata.total;
            foreach (var playerScore in scoreSaberPlayerScoreList.playerScores)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        public PPPScoreCollection(BeatLeaderPlayerScoreList beatLeaderPlayerScoreList)
        {
            page = beatLeaderPlayerScoreList.metadata.page;
            itemsPerPage = beatLeaderPlayerScoreList.metadata.itemsPerPage;
            total = beatLeaderPlayerScoreList.metadata.total;
            foreach (var playerScore in beatLeaderPlayerScoreList.data)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        public PPPScoreCollection(List<HitBloqScores> lsHitBloqScores, int page)
        {
            this.page = page;
            itemsPerPage = 10;
            total = lsHitBloqScores.Count > 0 ? page * itemsPerPage + 1 : 0;
            foreach (var playerScore in lsHitBloqScores)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        public PPPScoreCollection(List<AccSaberScores> lsHitBloqScores, int page)
        {
            this.page = page;
            itemsPerPage = 10;
            total = lsHitBloqScores.Count > 0 ? page * itemsPerPage + 1 : 0;
            foreach (var playerScore in lsHitBloqScores)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }


    }
}
