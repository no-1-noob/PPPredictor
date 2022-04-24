using LeaderboardCore.Interfaces;

namespace PPPredictor.Utilities
{
    public class PPPredictorEventsMgr : INotifyScoreUpload
    {
        public void OnScoreUploaded()
        {
            Plugin.pppViewController.refreshCurrentData(1);
        }
    }
}
