using PPPredictor.Core.DataType.BeatSaberEncapsulation;

namespace PPPredictor.Core.DataType
{
    public class PPPBeatMapInfo
    {
        PPPStarRating _baseStarRating = new PPPStarRating();
        PPPStarRating _modifiedStarRating = new PPPStarRating();
        BeatmapKey _beatmapkey;
        string _customLevelHash;
        double _maxPP = -1;
        string _selectedMapSearchString;
        bool _oldDotsEnabled;

        public PPPStarRating BaseStarRating { get => _baseStarRating; set => _baseStarRating = value; }
        internal PPPStarRating ModifiedStarRating { get => _modifiedStarRating; set => _modifiedStarRating = value; }
        public BeatmapKey BeatmapKey { get => _beatmapkey; set => _beatmapkey = value; }
        public double MaxPP { get => _maxPP; set => _maxPP = value; }
        public string SelectedMapSearchString { get => _selectedMapSearchString; set => _selectedMapSearchString = value; }
        public bool OldDotsEnabled { get => _oldDotsEnabled; set => _oldDotsEnabled = value; }
        public string CustomLevelHash { get => _customLevelHash; set => _customLevelHash = value; }

        public PPPBeatMapInfo()
        {
            _baseStarRating = _modifiedStarRating = new PPPStarRating();
        }
        public PPPBeatMapInfo(PPPBeatMapInfo beatMapInfo, PPPStarRating baseStars) : this(beatMapInfo.CustomLevelHash, beatMapInfo.BeatmapKey)
        {
            _baseStarRating = _modifiedStarRating = baseStars;
        }
        public PPPBeatMapInfo(string customLevelHash, BeatmapKey beatmapKey)
        {
            _customLevelHash = customLevelHash;
            this._beatmapkey = beatmapKey;
        }
    }
}
