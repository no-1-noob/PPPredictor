using LeaderboardCore.Interfaces;

namespace PPPredictor.Events
{
    public class PPPredictorEventsMgr : INotifyScoreUpload
    {
        public void OnScoreUploaded()
        {
            Plugin.pppViewController.RefreshCurrentData(1);
        }
    }
}
