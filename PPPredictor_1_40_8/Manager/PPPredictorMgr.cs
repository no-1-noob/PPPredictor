using IPA.Loader;
using PPPredictor.Core.DataType;
using PPPredictor.Shared.Interfaces;
using SongDetailsCache;
using SongDetailsCache.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;

namespace PPPredictor.Manager
{
    internal class PPPredictorMgr : PPPredictor.Shared.Manager.PPPredictorMgrBase, IInitializable, IDisposable

    {
        protected SongDetails songDetails { get; set; }

        public PPPredictorMgr() : base(new WebSocketMgr())
        {
        }

        public override List<string> GetEnabledPlugins()
        {
            return PluginManager.EnabledPlugins.ToList().Select(x => x.Name).ToList();
        }
        
        
        
        public override PPPBeatMapInfo ScoreSaberSongCoreLookUp(PPPBeatMapInfo beatMapInfo)
        {
            if (songDetails.songs.FindByHash(beatMapInfo.CustomLevelHash, out Song song))
            {
                if(Enum.TryParse(beatMapInfo.BeatmapKey.difficulty.ToString(), out MapDifficulty mapDifficulty))
                {
                    if (song.GetDifficulty(out SongDifficulty songDiff, mapDifficulty))
                    {
                        return new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(songDiff.stars));
                    }
                }
            }
            return new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(0));
        }

        public override async Task SetupSongDetails()
        {
            songDetails = await SongDetails.Init();
        }

        public override void Initialize()
        {
            //throw new NotImplementedException();
        }

        public override void Dispose()
        {
            foreach (IPPPredictor pPPredictor in _lsPPPredictor)
            {
                pPPredictor.OnDataLoading -= PPPredictor_OnDataLoading;
                pPPredictor.OnDisplayPPInfo -= PPPredictor_OnDisplayPPInfo;
                pPPredictor.OnDisplaySessionInfo -= PPPredictor_OnDisplaySessionInfo;
                pPPredictor.OnMapPoolRefreshed -= PPPredictor_OnMapPoolRefreshed;
            }
            _websocketMgr.OnScoreSet -= WebsocketMgrOnOnScoreSet;
            _websocketMgr.Dispose();
        }
    }
}
