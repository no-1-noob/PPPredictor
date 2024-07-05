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
        private Dictionary<string, object> dctDisplayPPInfo = new Dictionary<string, object>();
        private Dictionary<string, object> dctDisplaySessionInfo = new Dictionary<string, object>();

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
            get => dctDisplayPPInfo.Values.ToList();
        }
        [UIValue("MultiViewSessionInfoData")]
        public List<object> MultiViewSessionInfoData
        {
            get => dctDisplaySessionInfo.Values.ToList();
        }
        #endregion

        #region logic
        private void UpdateMultiViewType<T>(CustomCellListTableData customCellListTable, Dictionary<string, object> dct, MultiViewType multiViewType) where T : DisplayInfoTypeSelector
        {
            dct.Values.ToList().ForEach((x) =>
            {
                var xy = x as T;
                xy.multiViewType = multiViewType;
            });
            if(Plugin.ProfileInfo.IsMultiViewEnabled) floatingScreen.StartCoroutine(RefreshMultiView(customCellListTable, dct));
        }

        private IEnumerator RefreshMultiView(CustomCellListTableData customCellListTable, Dictionary<string, object> dctObjects)
        {
            yield return null; // Wait until the next frame
            if (customCellListTable != null && customCellListTable.isActiveAndEnabled && dctObjects.Values.ToList().Count > 0)
            {
                try
                {
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MultiViewPPInfoData)));
                    customCellListTable.data = dctObjects.Values.ToList();
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
