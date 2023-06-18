namespace PPPredictor.Data
{
    class PPPBeatMapInfo
    {
        PPPStarRating _baseStarRating = new PPPStarRating();
        PPPStarRating _modifiedStarRating = new PPPStarRating();
        CustomBeatmapLevel _selectedCustomBeatmapLevel;
        IDifficultyBeatmap _beatmap;
        double _maxPP = -1;
        string _selectedMapSearchString;

        public PPPStarRating BaseStarRating { get => _baseStarRating; set => _baseStarRating = value; }
        internal PPPStarRating ModifiedStarRating { get => _modifiedStarRating; set => _modifiedStarRating = value; }
        public CustomBeatmapLevel SelectedCustomBeatmapLevel { get => _selectedCustomBeatmapLevel; set => _selectedCustomBeatmapLevel = value; }
        public IDifficultyBeatmap Beatmap { get => _beatmap; set => _beatmap = value; }
        public double MaxPP { get => _maxPP; set => _maxPP = value; }
        public string SelectedMapSearchString { get => _selectedMapSearchString; set => _selectedMapSearchString = value; }

        public PPPBeatMapInfo()
        {
            _baseStarRating = _modifiedStarRating = new PPPStarRating();
        }

        public PPPBeatMapInfo(PPPBeatMapInfo beatMapInfo, PPPStarRating baseStars) : this(beatMapInfo.SelectedCustomBeatmapLevel, beatMapInfo.Beatmap)
        {
            _baseStarRating = _modifiedStarRating = baseStars;
        }

        public PPPBeatMapInfo(CustomBeatmapLevel selectedCustomBeatmapLevel, IDifficultyBeatmap beatmap)
        {
            _selectedCustomBeatmapLevel = selectedCustomBeatmapLevel;
            this._beatmap = beatmap;
        }
    }
}
