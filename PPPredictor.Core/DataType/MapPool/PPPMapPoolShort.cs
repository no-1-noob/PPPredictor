using System;
namespace PPPredictor.Core.DataType.MapPool
{
    public class PPPMapPoolShort
    {
        private string iconUrl;
        private byte[] _iconData;
        private string _id;
        private string _mapPoolName;
        private int _sortIndex;
        private bool _selectedByLoading = false;


        public string Id { get => _id; set => _id = value; }
        public string IconUrl { get => iconUrl; set => iconUrl = value; }
        public byte[] IconData { get => _iconData; set => _iconData = value; }
        public string MapPoolName { get => _mapPoolName; set => _mapPoolName = value; }
        public int SortIndex { get => _sortIndex; set => _sortIndex = value; }
        public bool SelectedByLoading { get => _selectedByLoading; set => _selectedByLoading = value; }

        public PPPMapPoolShort()
        {
            this.iconUrl = string.Empty;
            _iconData = new byte[0];
            _id = string.Empty;
            _mapPoolName = string.Empty;
            _sortIndex = 0;
        }

        public PPPMapPoolShort(string iconUrl, byte[] iconData, string id, string mapPoolName, int sortIndex)
        {
            this.iconUrl = iconUrl;
            _iconData = iconData;
            _id = id;
            _mapPoolName = mapPoolName;
            _sortIndex = sortIndex;
        }

        public override string ToString()
        {
            return $"{_mapPoolName}";
        }
    }
}
