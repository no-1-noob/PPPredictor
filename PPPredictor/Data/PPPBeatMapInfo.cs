namespace PPPredictor.Data
{
    public class PPPBeatMapInfo
    {
        PPPStarRating _baseStarRating = new PPPStarRating();
        int _modifierValueId = 0;

        public PPPStarRating BaseStarRating { get => _baseStarRating; set => _baseStarRating = value; }
        public int ModifierValueId { get => _modifierValueId; set => _modifierValueId = value; }

        public PPPBeatMapInfo()
        {
        }

        public PPPBeatMapInfo(PPPStarRating baseStars, int modifierValueId)
        {
            _baseStarRating = baseStars;
            _modifierValueId = modifierValueId;
        }
    }
}
