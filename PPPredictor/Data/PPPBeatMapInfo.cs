namespace PPPredictor.Data
{
    class PPPBeatMapInfo
    {
        double _baseStars = 0;
        double _currentSelectionStars;
        int _modifierValueId = 0;
        CustomBeatmapLevel _selectedCustomBeatmapLevel;
        IDifficultyBeatmap _beatmap;
        double _maxPP = -1;
        string _selectedMapSearchString;

        public double BaseStars { get => _baseStars; set => _baseStars = value; }
        public int ModifierValueId { get => _modifierValueId; set => _modifierValueId = value; }
        public CustomBeatmapLevel SelectedCustomBeatmapLevel { get => _selectedCustomBeatmapLevel; set => _selectedCustomBeatmapLevel = value; }
        public IDifficultyBeatmap Beatmap { get => _beatmap; set => _beatmap = value; }
        public double CurrentSelectionStars { get => _currentSelectionStars; set => _currentSelectionStars = value; }
        public double MaxPP { get => _maxPP; set => _maxPP = value; }
        public string SelectedMapSearchString { get => _selectedMapSearchString; set => _selectedMapSearchString = value; }

        public PPPBeatMapInfo()
        {
        }

        public PPPBeatMapInfo(PPPBeatMapInfo beatMapInfo, double baseStars) : this(beatMapInfo.SelectedCustomBeatmapLevel, beatMapInfo.Beatmap)
        {
            _baseStars = baseStars;
        }

        public PPPBeatMapInfo(PPPBeatMapInfo beatMapInfo, double baseStars, int modifierValueId) : this(beatMapInfo.SelectedCustomBeatmapLevel, beatMapInfo.Beatmap)
        {
            _baseStars = baseStars;
            _modifierValueId = modifierValueId;
        }

        public PPPBeatMapInfo(CustomBeatmapLevel selectedCustomBeatmapLevel, IDifficultyBeatmap beatmap)
        {
            _selectedCustomBeatmapLevel = selectedCustomBeatmapLevel;
            this._beatmap = beatmap;
        }
    }
}
