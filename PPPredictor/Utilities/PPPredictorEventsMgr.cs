//using LeaderboardCore.Interfaces;

namespace PPPredictor.Utilities
{
    public class PPPredictorEventsMgr// : INotifyScoreUpload
    {
        public void OnScoreUploaded()
        {
            Plugin.Log?.Error($"OnScoreUploaded");
            Plugin.pppViewController.refreshCurrentData(1);
        }
    }
}
