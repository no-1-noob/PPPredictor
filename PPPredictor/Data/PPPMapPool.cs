using BeatSaberMarkupLanguage.Attributes;
using Newtonsoft.Json;
using PPPredictor.Data.Curve;
using PPPredictor.Interfaces;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;

namespace PPPredictor.Data
{
    class PPPMapPool
    {
        private string _id;
        private string _playListId;
        private MapPoolType _mapPoolType;
        private string _mapPoolName;
        private float _accumulationConstant;
        private int _sortIndex;
        private IPPPCurve _curve;
        private PPPPlayer _sessionPlayer;
        private PPPPlayer _currentPlayer;
        private List<ShortScore> _lsScores;
        private List<ShortScore> _lsLeaderboardScores;
        private List<PPPMapPoolEntry> _lsMapPoolEntries;
        private List<PPPPlayer> _lsPlayerRankings;
        private DateTime _dtUtcLastRefresh;
        private DateTime _dtUtcLastSessionReset;
        private DateTimeOffset _dtLastScoreSet;
        private double _popularity;
        private string _iconUrl;
        private byte[] _iconData;
        private string _syncUrl;

        [UIValue("mapPoolName")]
        public string MapPoolName { get => _mapPoolName; set => _mapPoolName = value; }
        public float AccumulationConstant { get => _accumulationConstant; set => _accumulationConstant = value; }
        public int SortIndex { get => _sortIndex; set => _sortIndex = value; }
        public List<ShortScore> LsScores { get => _lsScores; set => _lsScores = value; }
        public List<ShortScore> LsLeaderboardScores { get => _lsLeaderboardScores; set => _lsLeaderboardScores = value; }
        public List<PPPMapPoolEntry> LsMapPoolEntries { get => _lsMapPoolEntries; set => _lsMapPoolEntries = value; }
        public MapPoolType MapPoolType { get => _mapPoolType; set => _mapPoolType = value; }
        internal IPPPCurve Curve { get => _curve; set => _curve = value; }
        public CurveInfo CurveInfo { get => _curve.IsDummy ? null : _curve.ToCurveInfo(); set => _curve = CurveParser.ParseToCurve(value); }
        public PPPPlayer SessionPlayer { get => _sessionPlayer; set => _sessionPlayer = value; }
        public PPPPlayer CurrentPlayer { get => _currentPlayer; set => _currentPlayer = value; }
        public string Id { get => _id; set => _id = value; }
        public string PlayListId { get => _playListId; set => _playListId = value; }
        [JsonIgnore]
        public List<PPPPlayer> LsPlayerRankings { get => _lsPlayerRankings; set => _lsPlayerRankings = value; }
        public DateTime DtUtcLastRefresh { get => _dtUtcLastRefresh; set => _dtUtcLastRefresh = value; }
        public DateTime DtUtcLastSessionReset { get => _dtUtcLastSessionReset; set => _dtUtcLastSessionReset = value; }
        public DateTimeOffset DtLastScoreSet { get => _dtLastScoreSet; set => _dtLastScoreSet = value; }
        public string IconUrl { get => _iconUrl; set => _iconUrl = value; }
        [JsonIgnore]
        public byte[] IconData { get => _iconData; set => _iconData = value; }
        public double Popularity { get => _popularity; set => _popularity = value; }
        public string SyncUrl { get => _syncUrl; set => _syncUrl = value; }

        [JsonConstructor]

        public PPPMapPool()
        {
            _currentPlayer = new PPPPlayer();
            _lsScores = new List<ShortScore>();
            LsLeaderboardScores = new List<ShortScore>();
            _lsMapPoolEntries = new List<PPPMapPoolEntry>();
            _lsPlayerRankings = new List<PPPPlayer>();
            _dtUtcLastRefresh = new DateTime(2000, 1, 1);
            DtUtcLastSessionReset = new DateTime(2000, 1, 1);
            _curve = CustomPPPCurve.DummyPPPCurve();
            _id = "-1";
            _playListId = "-1";
            _mapPoolType = MapPoolType.Custom;
            _mapPoolName = string.Empty;
            _accumulationConstant = 0;
            _sortIndex = -1;
            _dtLastScoreSet = new DateTime(2000, 1, 1);
            _popularity = 0;
            _syncUrl = string.Empty;
        }

        public PPPMapPool(string id, string playListId, MapPoolType mapPoolType, string mapPoolName, float accumulationConstant, int sortIndex, IPPPCurve curve, string iconUrl, double popularity = 0, string syncUrl = "") : this()
        {
            _id = id;
            _playListId = playListId;
            _mapPoolType = mapPoolType;
            _mapPoolName = mapPoolName;
            _accumulationConstant = accumulationConstant;
            _sortIndex = sortIndex;
            _curve = curve;
            _iconUrl= iconUrl;
            _popularity = popularity;
            _syncUrl= syncUrl;
        }

        public PPPMapPool(MapPoolType mapPoolType, string mapPoolName, float accumulationConstant, int sortIndex, IPPPCurve curve) : this("-1", "-1", mapPoolType, mapPoolName, accumulationConstant, sortIndex, curve, string.Empty)
        {
        }

        public override string ToString()
        {
            return $"{_mapPoolName}";
        }
    }
}
