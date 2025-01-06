using Newtonsoft.Json;
using PPPredictor.Core.DataType.Curve;
using PPPredictor.Core.DataType.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.DataType.MapPool
{
    public class PPPMapPool
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
        private List<ShortScore> _lsLeaderboardInfo;
        private List<PPPMapPoolEntry> _lsMapPoolEntries;
        private List<PPPPlayer> _lsPlayerRankings;
        private DateTime _dtUtcLastRefresh;
        private DateTimeOffset _dtLastScoreSet;
        private double _popularity;
        private string _iconUrl;
        private byte[] _iconData;
        private string _syncUrl;
        private LeaderboardContext _leaderboardContext;
        private bool isPlayerFound;
        private Dictionary<int, double> dctWeightLookup;
        private string _customLeaderboardUserId;

        public string MapPoolName { get => _mapPoolName; set => _mapPoolName = value; }
        public float AccumulationConstant { get => _accumulationConstant; set => _accumulationConstant = value; }
        public int SortIndex { get => _sortIndex; set => _sortIndex = value; }
        public List<ShortScore> LsScores
        {
            get => _lsScores;
            set
            {
                if (value != null)
                    _lsScores = value.OrderByDescending(x => x.Pp).ToList();
                else
                    _lsScores = value;
            }
        }
        public List<ShortScore> LsLeaderboadInfo { get => _lsLeaderboardInfo; set => _lsLeaderboardInfo = value; }
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
        public DateTimeOffset DtLastScoreSet { get => _dtLastScoreSet; set => _dtLastScoreSet = value; }
        public string IconUrl { get => _iconUrl; set => _iconUrl = value; }
        [JsonIgnore]
        public byte[] IconData { get => _iconData; set => _iconData = value; }
        public double Popularity { get => _popularity; set => _popularity = value; }
        public string SyncUrl { get => _syncUrl; set => _syncUrl = value; }
        public LeaderboardContext LeaderboardContext { get => _leaderboardContext; set => _leaderboardContext = value; }
        public bool IsPlayerFound { get => isPlayerFound; set => isPlayerFound = value; }
        [JsonIgnore]
        public Dictionary<int, double> DctWeightLookup { get => dctWeightLookup; set => dctWeightLookup = value; }
        public string CustomLeaderboardUserId { get => _customLeaderboardUserId; set => _customLeaderboardUserId = value; }

        [JsonConstructor]

        public PPPMapPool()
        {
            _currentPlayer = new PPPPlayer();
            _lsScores = new List<ShortScore>();
            LsLeaderboadInfo = new List<ShortScore>();
            _lsMapPoolEntries = new List<PPPMapPoolEntry>();
            _lsPlayerRankings = new List<PPPPlayer>();
            _dtUtcLastRefresh = new DateTime(2000, 1, 1);
            _curve = CustomPPPCurve.CreateDummyPPPCurve();
            _id = "-1";
            _playListId = "-1";
            _mapPoolType = MapPoolType.Custom;
            _mapPoolName = string.Empty;
            _accumulationConstant = 0;
            _sortIndex = -1;
            _dtLastScoreSet = new DateTime(2000, 1, 1);
            _popularity = 0;
            _syncUrl = string.Empty;
            _leaderboardContext = LeaderboardContext.None;
            isPlayerFound = true;
            dctWeightLookup = new Dictionary<int, double>();
            _customLeaderboardUserId = string.Empty;
        }

        public PPPMapPool(string id, string playListId, MapPoolType mapPoolType, string mapPoolName, float accumulationConstant, int sortIndex, IPPPCurve curve, string iconUrl, double popularity = 0, string syncUrl = "", LeaderboardContext leaderboardContext = LeaderboardContext.None) : this()
        {
            _id = id;
            _playListId = playListId;
            _mapPoolType = mapPoolType;
            _mapPoolName = mapPoolName;
            _accumulationConstant = accumulationConstant;
            _sortIndex = sortIndex;
            _curve = curve;
            _iconUrl = iconUrl;
            _popularity = popularity;
            _syncUrl = syncUrl;
            _leaderboardContext = leaderboardContext;
        }

        public PPPMapPool(MapPoolType mapPoolType, string mapPoolName, float accumulationConstant, int sortIndex, IPPPCurve curve, LeaderboardContext leaderboardContext = LeaderboardContext.None) : this("-1", "-1", mapPoolType, mapPoolName, accumulationConstant, sortIndex, curve, string.Empty, 0, "", leaderboardContext)
        {
        }

        public PPPMapPool(string id, MapPoolType mapPoolType, string mapPoolName, float accumulationConstant, int sortIndex, IPPPCurve curve, LeaderboardContext leaderboardContext = LeaderboardContext.None) : this(id, "-1", mapPoolType, mapPoolName, accumulationConstant, sortIndex, curve, string.Empty, 0, "", leaderboardContext)
        {
        }

        public override string ToString()
        {
            return $"{_mapPoolName}";
        }

        public static implicit operator PPPMapPoolShort(PPPMapPool mapPool)
        {
            if(mapPool == null) return null;
            return new PPPMapPoolShort(
                mapPool.IconUrl,
                mapPool.IconData,
                mapPool.Id,
                mapPool.MapPoolName,
                mapPool.SortIndex
            );
        }
    }
}
