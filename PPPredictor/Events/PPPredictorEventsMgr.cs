using LeaderboardCore.Interfaces;
using System.Threading.Tasks;

namespace PPPredictor.Events
{
    class PPPredictorEventsMgr : INotifyScoreUpload
    {
        public async void OnScoreUploaded()
        {
            await Task.Delay(5000); //Wait after upload confirmation with reload, to give other scoreboards time to upload
            Plugin.pppViewController.RefreshCurrentData(1);
        }
    }
}
