using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Zenject;

namespace PPPredictor.UI.ViewController
{
    public partial class PPPredictorViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private Dictionary<string, DisplayInfoTypeSelector> dctDisplayPPInfo = new Dictionary<string, DisplayInfoTypeSelector>();
        private Dictionary<string, DisplayInfoTypeSelector> dctDisplaySessionInfo = new Dictionary<string, DisplayInfoTypeSelector>();

        #region UI Components
        [UIComponent("listDisplayPPInfo")]
        public CustomCellListTableData listDisplayPPInfo;
        [UIComponent("listDisplaySessionInfo")]
        public CustomCellListTableData listDisplaySessionInfo;
        
        #endregion

        #region UIValue

        [UIValue("MultiViewPPInfoData")]
        public List<object> MultiViewPPInfoData
        {
            get => dctDisplayPPInfo.Values.OrderBy(x => x.SortIndex).ToList<object>();
        }
        [UIValue("MultiViewSessionInfoData")]
        public List<object> MultiViewSessionInfoData
        {
            get => dctDisplaySessionInfo.Values.OrderBy(x => x.SortIndex).ToList<object>();
        }
        #endregion

        #region logic
        private void UpdateMultiViews()
        {
            UpdateMultiViewType(listDisplayPPInfo, dctDisplayPPInfo);
            UpdateMultiViewType(listDisplaySessionInfo, dctDisplaySessionInfo);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMultiViewEnabled)));
        }
        private void UpdateMultiViewType(CustomCellListTableData customCellListTable, Dictionary<string, DisplayInfoTypeSelector> dct)
        {
            dct.Values.ToList().ForEach((x) =>
            {
                x.multiViewType = Plugin.ProfileInfo.MultiViewType;
            });
            if(Plugin.ProfileInfo.IsMultiViewEnabled) floatingScreen.StartCoroutine(RefreshMultiView(customCellListTable, dct));
        }

        private IEnumerator RefreshMultiView(CustomCellListTableData customCellListTable, Dictionary<string, DisplayInfoTypeSelector> dctObjects)
        {
            yield return null; // Wait until the next frame
            if (customCellListTable != null && customCellListTable.isActiveAndEnabled && dctObjects.Values.ToList().Count > 0)
            {
                try
                {
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MultiViewPPInfoData)));
                    customCellListTable.data = dctObjects.Values.OrderBy(x => x.SortIndex).ToList<object>();
                    customCellListTable?.tableView?.ReloadData();
                }
                catch (Exception ex)
                {
                    Plugin.Log.Error($"Error while refreshing multivew: {ex.Message}");
                }
            }
        }
        #endregion
    }
}
