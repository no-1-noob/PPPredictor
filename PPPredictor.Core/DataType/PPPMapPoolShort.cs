using System;
namespace PPPredictor.Core.DataType
{
    public class PPPMapPoolShort
    {
        private string iconUrl;
        private byte[] _iconData;
        private PPPPlayer _currentPlayer;
        private PPPPlayer _sessionPlayer;
        private DateTime _dtUtcLastRefresh;
        private string _id;
        private string _mapPoolName;
        private int _sortIndex;


        public string Id { get => _id; set => _id = value; }
        public string IconUrl { get => iconUrl; set => iconUrl = value; }
        public byte[] IconData { get => _iconData; set => _iconData = value; }
        public PPPPlayer SessionPlayer { get => _sessionPlayer; set => _sessionPlayer = value; }
        public PPPPlayer CurrentPlayer { get => _currentPlayer; set => _currentPlayer = value; }
        public DateTime DtUtcLastRefresh { get => _dtUtcLastRefresh; set => _dtUtcLastRefresh = value; }
        public string MapPoolName { get => _mapPoolName; set => _mapPoolName = value; }
        public int SortIndex { get => _sortIndex; set => _sortIndex = value; }

        public PPPMapPoolShort(string iconUrl, byte[] iconData, PPPPlayer currentPlayer, PPPPlayer sessionPlayer, DateTime dtUtcLastRefresh, string id, string mapPoolName, int sortIndex)
        {
            this.iconUrl = iconUrl;
            _iconData = iconData;
            _currentPlayer = currentPlayer;
            _sessionPlayer = sessionPlayer;
            _dtUtcLastRefresh = dtUtcLastRefresh;
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
