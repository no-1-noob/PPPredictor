namespace PPPredictor.Data
{
    public class PPPBeatMapInfo
    {
        double _baseStars = 0;
        int _modifierValueId = 0;

        public double BaseStars { get => _baseStars; set => _baseStars = value; }
        public int ModifierValueId { get => _modifierValueId; set => _modifierValueId = value; }

        public PPPBeatMapInfo()
        {
        }

        public PPPBeatMapInfo(double baseStars)
        {
            _baseStars = baseStars;
        }

        public PPPBeatMapInfo(double baseStars, int modifierValueId)
        {
            _baseStars = baseStars;
            _modifierValueId = modifierValueId;
        }
    }
}
