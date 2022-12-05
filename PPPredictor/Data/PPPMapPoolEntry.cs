using static PPPredictor.OpenAPIs.beatleaderapi;

namespace PPPredictor.Data
{
    public class PPPMapPoolEntry
    {
        private string _searchstring;
        public string Searchstring { get => _searchstring; set => _searchstring = value; }

        public PPPMapPoolEntry()
        {
            Searchstring = string.Empty;
        }
        public PPPMapPoolEntry(string searchstring)
        {
            Searchstring = searchstring;
        }

        public PPPMapPoolEntry(BeatLeaderPlayListSong song, BeatLeaderPlayListDifficulties diff)
        {
            _searchstring = $"{song.hash}_{(int)diff.name}";
        }
    }
}
