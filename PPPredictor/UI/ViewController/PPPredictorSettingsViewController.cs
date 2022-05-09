using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using System.ComponentModel;

namespace PPPredictor.UI.ViewController
{
    [HotReload(RelativePathToLayout = @"..\Views\PPPredictorSettingsView.bsml")]
    [ViewDefinition("PPPredictor.UI.Views.PPPredictorSettingsView.bsml")]
    internal class PPPredictorSettingsViewController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region settings
        [UIParams]
        private readonly BSMLParserParams bsmlParserParams;

        [UIAction("reset-data")]
        private void ResetData()
        {
            bsmlParserParams.EmitEvent("open-reset-modal");
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
        [UIValue("reset-session-hours")]
        public int ResetSessionHours
        {
            get => Plugin.ProfileInfo.ResetSessionHours;
            set
            {
                Plugin.ProfileInfo.ResetSessionHours = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResetSessionHours)));
            }
        }
        #endregion

        #region modal actions
        [UIAction("confirm-reset-modal")]
        private void ConfirmResetModal()
        {
            Plugin.ProfileInfo = new ProfileInfo();
            refreshSettingsDisplay();
            bsmlParserParams.EmitEvent("close-reset-modal");
        }
        #endregion

        private void refreshSettingsDisplay()
        {
            WindowHandleEnabled = Plugin.ProfileInfo.WindowHandleEnabled;
            DisplaySessionValues = Plugin.ProfileInfo.DisplaySessionValues;
            ResetSessionHours = Plugin.ProfileInfo.ResetSessionHours;
            Plugin.pppViewController?.resetDisplay(true); //Needed for canceling of settings
        }
    }
}
