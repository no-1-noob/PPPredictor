using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using PPPredictor.Utilities;
using System.ComponentModel;

namespace PPPredictor.UI.ViewController
{
    [HotReload(RelativePathToLayout = @"..\Views\PPPredictorSettingsView.bsml")]
    [ViewDefinition("PPPredictor.UI.Views.PPPredictorSettingsView.bsml")]
    internal class PPPredictorSettingsViewController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region settings
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        [UIParams]
        private readonly BSMLParserParams bsmlParserParams;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null

#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("reset-data")]
        private void ResetData()
        {
            bsmlParserParams.EmitEvent("open-reset-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members

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

#pragma warning disable IDE0051 // Remove unused private members
        #region modal actions
        [UIAction("confirm-reset-modal")]
        private void ConfirmResetModal()
        {
            ProfileInfoMgr.ResetProfile();
            RefreshSettingsDisplay();
            bsmlParserParams.EmitEvent("close-reset-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members
        #endregion

        private void RefreshSettingsDisplay()
        {
            WindowHandleEnabled = Plugin.ProfileInfo.WindowHandleEnabled;
            DisplaySessionValues = Plugin.ProfileInfo.DisplaySessionValues;
            ResetSessionHours = Plugin.ProfileInfo.ResetSessionHours;
            Plugin.pppViewController?.ResetDisplay(true); //Needed for canceling of settings
        }
    }
}
