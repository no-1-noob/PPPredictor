namespace PPPredictor.Core.DataType.BeatSaberEncapsulation
{
    public class GameplayModifiers
    {
        public bool disappearingArrows = false;
        public SongSpeed songSpeed = SongSpeed.Normal;
        public bool ghostNotes = false;
        public bool noArrows = false;
        public bool noBombs = false;
        public bool noFailOn0Energy = false;
        public EnabledObstacleType enabledObstacleType = EnabledObstacleType.All;
        public bool proMode = false;
        public bool smallCubes = false;
        public bool instaFail = false;
        public EnergyType energyType = EnergyType.Bar;
        public bool strictAngles = false;
        public bool zenMode = false;

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
