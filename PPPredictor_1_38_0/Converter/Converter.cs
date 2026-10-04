using SongCore.Utilities;
namespace PPPredictor.Converter
{
    internal partial class Converter : ConverterBase
    {
        public override string GetCustomLevelHash(BeatmapLevel beatmapLevel)
        {
            return Hashing.GetCustomLevelHash(beatmapLevel);
        }
    }
}
