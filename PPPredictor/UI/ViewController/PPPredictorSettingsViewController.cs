using BeatSaberMarkupLanguage.Attributes;
using System.ComponentModel;

namespace PPPredictor.UI.ViewController
{
    [HotReload(RelativePathToLayout = @"..\Views\PPPredictorSettingsView.bsml")]
    [ViewDefinition("PPPredictor.UI.Views.PPPredictorSettingsView.bsml")]
    internal class PPPredictorSettingsViewController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region settings
        [UIAction("reset-data")]
        private void ResetData()
        {
            Plugin.ProfileInfo = new ProfileInfo();
            refreshSettingsDisplay();
        }

        [UIValue("window-handle-enabled")]
        public bool WindowHandleEnabled
        {
            get => Plugin.ProfileInfo.WindowHandleEnabled;
            set
            {
                Plugin.ProfileInfo.WindowHandleEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowHandleEnabled)));
            }
        }
        #endregion

        private void refreshSettingsDisplay()
        {
            WindowHandleEnabled = Plugin.ProfileInfo.WindowHandleEnabled;
            Plugin.pppViewController?.refreshDisplay(); //Needed for canceling of settings
        }
    }
}
