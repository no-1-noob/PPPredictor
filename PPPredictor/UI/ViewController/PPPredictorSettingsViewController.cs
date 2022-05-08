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

        [UIValue("display-session-values")]
        public bool DisplaySessionValues
        {
            get => Plugin.ProfileInfo.DisplaySessionValues;
            set
            {
                Plugin.ProfileInfo.DisplaySessionValues = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplaySessionValues)));
            }
        }
        #endregion

        private void refreshSettingsDisplay()
        {
            WindowHandleEnabled = Plugin.ProfileInfo.WindowHandleEnabled;
            DisplaySessionValues = Plugin.ProfileInfo.DisplaySessionValues;
            Plugin.pppViewController?.resetDisplay(); //Needed for canceling of settings
        }
    }
}
