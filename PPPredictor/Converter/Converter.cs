using static PlayerSaveData;

namespace PPPredictor.Converter
{
    internal class Converter
    {
        public static Core.DataType.BeatSaberEncapsulation.BeatmapKey ConvertBeatmapKey(BeatmapKey beatmapKey)
        {
            return new Core.DataType.BeatSaberEncapsulation.BeatmapKey
            {
                serializedName = beatmapKey.beatmapCharacteristic.serializedName,
                difficulty = GetBeatMapKeyDifficulty(beatmapKey.difficulty)
            };
        }

        private static Core.DataType.Enums.BeatMapDifficulty GetBeatMapKeyDifficulty(BeatmapDifficulty diff)
        {
            switch (diff)
            {
                case BeatmapDifficulty.Easy:
                    return Core.DataType.Enums.BeatMapDifficulty.Easy;
                case BeatmapDifficulty.Normal:
                    return Core.DataType.Enums.BeatMapDifficulty.Normal;
                case BeatmapDifficulty.Hard:
                    return Core.DataType.Enums.BeatMapDifficulty.Hard;
                case BeatmapDifficulty.Expert:
                    return Core.DataType.Enums.BeatMapDifficulty.Expert;
                case BeatmapDifficulty.ExpertPlus:
                    return Core.DataType.Enums.BeatMapDifficulty.ExpertPlus;
                default:
                    return Core.DataType.Enums.BeatMapDifficulty.Easy;
            }
        }

        //public static Core.DataType.BeatSaberEncapsulation.BeatmapLevel ConvertBeatMapLevel(BeatmapLevel beatmapLevel)
        //{
        //    return new Core.DataType.BeatSaberEncapsulation.BeatmapLevel
        //    {
        //        serializedName = beatmapKey.SerializedName(),
        //        difficulty = GetBeatMapKeyDifficulty(beatmapKey.difficulty)
        //    };
        //}

        public static Core.DataType.BeatSaberEncapsulation.GameplayModifiers ConvertGameplayModifiers(GameplayModifiers gameplayModifiers)
        {
            if(gameplayModifiers == null)
            {
                return new Core.DataType.BeatSaberEncapsulation.GameplayModifiers();
            }
            return new Core.DataType.BeatSaberEncapsulation.GameplayModifiers
            {
                disappearingArrows = gameplayModifiers.disappearingArrows,
                songSpeed = GetSongSpeed(gameplayModifiers.songSpeed),
                ghostNotes = gameplayModifiers.ghostNotes,
                noArrows = gameplayModifiers.noArrows,
                noBombs = gameplayModifiers.noBombs,
                noFailOn0Energy = gameplayModifiers.noFailOn0Energy,
                enabledObstacleType = GetObstacleType(gameplayModifiers.enabledObstacleType),
                proMode = gameplayModifiers.proMode,
                smallCubes = gameplayModifiers.smallCubes,
                instaFail = gameplayModifiers.instaFail,
                energyType = GetEnergyType(gameplayModifiers.energyType),
                strictAngles = gameplayModifiers.strictAngles,
                zenMode = gameplayModifiers.zenMode,
            };
        }

        private static Core.DataType.BeatSaberEncapsulation.GameplayModifiers.SongSpeed GetSongSpeed(GameplayModifiers.SongSpeed speed)
        {
            switch (speed)
            {
                case GameplayModifiers.SongSpeed.Normal:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.SongSpeed.Normal;
                case GameplayModifiers.SongSpeed.Faster:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.SongSpeed.Faster;
                case GameplayModifiers.SongSpeed.Slower:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.SongSpeed.Slower;
                case GameplayModifiers.SongSpeed.SuperFast:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.SongSpeed.SuperFast;
                default:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.SongSpeed.Normal;
            }
        }

        private static Core.DataType.BeatSaberEncapsulation.GameplayModifiers.EnabledObstacleType GetObstacleType(GameplayModifiers.EnabledObstacleType obstacleType)
        {
            switch (obstacleType)
            {
                case GameplayModifiers.EnabledObstacleType.All:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.EnabledObstacleType.All;
                case GameplayModifiers.EnabledObstacleType.FullHeightOnly:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.EnabledObstacleType.FullHeightOnly;
                case GameplayModifiers.EnabledObstacleType.NoObstacles:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.EnabledObstacleType.NoObstacles;
                default:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.EnabledObstacleType.All;
            }
        }

        private static Core.DataType.BeatSaberEncapsulation.GameplayModifiers.EnergyType GetEnergyType(GameplayModifiers.EnergyType energyType)
        {
            switch (energyType)
            {
                case GameplayModifiers.EnergyType.Bar:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.EnergyType.Bar;
                case GameplayModifiers.EnergyType.Battery:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.EnergyType.Battery;
                default:
                    return Core.DataType.BeatSaberEncapsulation.GameplayModifiers.EnergyType.Bar;
            }
        }
    }
}
