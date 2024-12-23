namespace PPPredictor.Core.DataType.BeatSaberEncapsulation
{
    internal class GameplayModifiers
    {
        internal bool disappearingArrows;
        internal SongSpeed songSpeed;
        internal bool ghostNotes;
        internal bool noArrows;
        internal bool noBombs;
        internal bool noFailOn0Energy;
        internal EnabledObstacleType enabledObstacleType;
        internal bool proMode;
        internal bool smallCubes;
        internal bool instaFail;
        internal EnergyType energyType;
        internal bool strictAngles;
        internal bool zenMode;

        internal enum SongSpeed
        {
            Faster,
            Slower,
            SuperFast
        }

        internal enum EnergyType
        {
            Battery
        }

        internal enum EnabledObstacleType
        {
            NoObstacles
        }
    }
}
