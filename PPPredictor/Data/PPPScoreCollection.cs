using PPPredictor.OpenAPIs;
using System.Collections.Generic;
using static PPPredictor.Data.LeaderBoardDataTypes.BeatLeaderDataTypes;
using static PPPredictor.Data.LeaderBoardDataTypes.HitBloqDataTypes;
using static PPPredictor.Data.LeaderBoardDataTypes.ScoreSaberDataTypes;
using static PPPredictor.OpenAPIs.BeatleaderAPI;

namespace PPPredictor.Data
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
            this.page = scoreSaberPlayerScoreList.metadata.page;
            this.itemsPerPage = scoreSaberPlayerScoreList.metadata.itemsPerPage;
            this.total = scoreSaberPlayerScoreList.metadata.total;
            foreach (var playerScore in scoreSaberPlayerScoreList.playerScores)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        public PPPScoreCollection(BeatLeaderPlayerScoreList beatLeaderPlayerScoreList)
        {
            this.page = beatLeaderPlayerScoreList.metadata.page;
            this.itemsPerPage = beatLeaderPlayerScoreList.metadata.itemsPerPage;
            this.total = beatLeaderPlayerScoreList.metadata.total;
            foreach (var playerScore in beatLeaderPlayerScoreList.data)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        public PPPScoreCollection(List<HitBloqScores> lsHitBloqScores, int page)
        {
            this.page = page;
            this.itemsPerPage = 10;
            this.total = (lsHitBloqScores.Count > 0) ? page * itemsPerPage + 1 : 0;
            foreach (var playerScore in lsHitBloqScores)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        
    }
}
