using beatleaderapi;
using scoresaberapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Data
{
    public class PPPScoreCollection
    {
        private List<PPPScore> lsPPPScore = new List<PPPScore>();
        private double page;
        private double itemsPerPage;
        private double total;

        public List<PPPScore> LsPPPScore { get => lsPPPScore; }
        public double Page { get => page; }
        public double ItemsPerPage { get => itemsPerPage; }
        public double Total { get => total; }

        public PPPScoreCollection(PlayerScoreCollection scoreSaberPlayerScoreCollection)
        {
            this.page = scoreSaberPlayerScoreCollection.Metadata.Page;
            this.itemsPerPage = scoreSaberPlayerScoreCollection.Metadata.ItemsPerPage;
            this.total = scoreSaberPlayerScoreCollection.Metadata.Total;
            foreach (var playerScore in scoreSaberPlayerScoreCollection.PlayerScores)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }

        public PPPScoreCollection(ScoreResponseWithMyScoreResponseWithMetadata scoreSaberPlayerScoreCollection)
        {
            this.page = scoreSaberPlayerScoreCollection.Metadata.Page;
            this.itemsPerPage = scoreSaberPlayerScoreCollection.Metadata.ItemsPerPage;
            this.total = scoreSaberPlayerScoreCollection.Metadata.Total;
            foreach (var playerScore in scoreSaberPlayerScoreCollection.Data)
            {
                lsPPPScore.Add(new PPPScore(playerScore));
            }
        }
    }
}
