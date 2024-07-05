using BeatSaberMarkupLanguage.Attributes;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Zenject;

namespace PPPredictor.UI.ViewController
{
    public partial class PPPredictorViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly List<object> multiViewType = new List<object>();

        private void QuickSettingsSetup()
        {
            foreach (MultiViewType enumValue in Enum.GetValues(typeof(MultiViewType)))
            {
                multiViewType.Add(enumValue.ToString());
            }
        }

        #region values
        [UIValue("isMultiViewEnabled")]
        private bool IsMultiViewEnabled
        {
            get => Plugin.ProfileInfo.IsMultiViewEnabled;
            set
            {
                Plugin.ProfileInfo.IsMultiViewEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMultiViewEnabled)));
            }
        }
        [UIValue("multi-view-type-options")]
        public List<object> MultiViewTypeOptions
        {
            get => this.multiViewType;
        }
        [UIValue("multi-view-type")]
        public string MultiViewType
        {
            get => Plugin.ProfileInfo.MultiViewType.ToString();
            set
            {
                Plugin.ProfileInfo.MultiViewType = (MultiViewType)Enum.Parse(typeof(MultiViewType), value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CounterScoringType)));
            }
        }

        #endregion
        #region actions
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("click-quick-settings-modal-show")]
        private void ClickQuickSettingsModalShow()
        {
            bsmlParserParams.EmitEvent("show-quick-settings-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("click-quick-settings-modal-close")]
        private void ClickQuickSettingsModalClose()
        {
            UpdateMultiViewType<DisplayPPInfo>(listDisplayPPInfo, dctDisplayPPInfo, Plugin.ProfileInfo.MultiViewType);
            UpdateMultiViewType<DisplaySessionInfo>(listDisplaySessionInfo, dctDisplaySessionInfo, Plugin.ProfileInfo.MultiViewType);
            bsmlParserParams.EmitEvent("close-quick-settings-modal");
        }
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("refresh-profile-clicked")]
        private void RefreshProfileClicked()
        {
            this.ppPredictorMgr.RefreshCurrentData(10);
        }
#pragma warning restore IDE0051 // Remove unused private members

#pragma warning disable IDE0051 // Remove unused private members
        [UIAction("reset-session-clicked")]
        private void ResetSessionClicked()
        {
            this.ppPredictorMgr.UpdateCurrentAndCheckResetSession(true);
        }
#pragma warning restore IDE0051 // Remove unused private members
#endregion
    }
}
