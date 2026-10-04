using SongCore;
namespace PPPredictor.Converter
{
    internal class Converter : ConverterBase
    {

        public override string GetCustomLevelHash(BeatmapLevel beatmapLevel)
        {
            return Collections.GetCustomLevelHash(beatmapLevel.levelID);
        }
    }
}
