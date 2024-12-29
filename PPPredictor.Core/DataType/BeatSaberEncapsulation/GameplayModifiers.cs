namespace PPPredictor.Core.DataType.BeatSaberEncapsulation
{
    public class GameplayModifiers
    {
        public bool disappearingArrows;
        public SongSpeed songSpeed;
        public bool ghostNotes;
        public bool noArrows;
        public bool noBombs;
        public bool noFailOn0Energy;
        public EnabledObstacleType enabledObstacleType;
        public bool proMode;
        public bool smallCubes;
        public bool instaFail;
        public EnergyType energyType;
        public bool strictAngles;
        public bool zenMode;

        public enum SongSpeed
        {
            Normal,
            Faster,
            Slower,
            SuperFast
        }

        public enum EnergyType
        {
            Bar,
            Battery
        }

        public enum EnabledObstacleType
        {
            All,
            FullHeightOnly,
            NoObstacles
        }
    }
}
