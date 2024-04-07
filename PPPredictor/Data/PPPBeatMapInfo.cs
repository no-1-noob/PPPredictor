namespace PPPredictor.Data
{
    class PPPBeatMapInfo
    {
        PPPStarRating _baseStarRating = new PPPStarRating();
        PPPStarRating _modifiedStarRating = new PPPStarRating();
        BeatmapLevel _selectedCustomBeatmapLevel;
        BeatmapKey _beatmapkey;
        double _maxPP = -1;
        string _selectedMapSearchString;
        bool _oldDotsEnabled;

        public PPPStarRating BaseStarRating { get => _baseStarRating; set => _baseStarRating = value; }
        internal PPPStarRating ModifiedStarRating { get => _modifiedStarRating; set => _modifiedStarRating = value; }
        public BeatmapLevel SelectedCustomBeatmapLevel { get => _selectedCustomBeatmapLevel; set => _selectedCustomBeatmapLevel = value; }
        public BeatmapKey BeatmapKey { get => _beatmapkey; set => _beatmapkey = value; }
        public double MaxPP { get => _maxPP; set => _maxPP = value; }
        public string SelectedMapSearchString { get => _selectedMapSearchString; set => _selectedMapSearchString = value; }
        public bool OldDotsEnabled { get => _oldDotsEnabled; set => _oldDotsEnabled = value; }

        public PPPBeatMapInfo()
        {
            _baseStarRating = _modifiedStarRating = new PPPStarRating();
        }

        public PPPBeatMapInfo(PPPBeatMapInfo beatMapInfo, PPPStarRating baseStars) : this(beatMapInfo.SelectedCustomBeatmapLevel, beatMapInfo.BeatmapKey)
        {
            _baseStarRating = _modifiedStarRating = baseStars;
        }

        public PPPBeatMapInfo(BeatmapLevel selectedCustomBeatmapLevel, BeatmapKey beatmapKey)
        {
            _selectedCustomBeatmapLevel = selectedCustomBeatmapLevel;
            this._beatmapkey = beatmapKey;
        }
    }
}
