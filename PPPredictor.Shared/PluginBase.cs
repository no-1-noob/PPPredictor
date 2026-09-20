using PPPredictor.Core.DataType;
using PPPredictor.Data;
using PPPredictor.Shared.Data;
using PPPredictor.Shared.Interfaces;
using System;
using System.Threading.Tasks;
namespace PPPredictor.Shared
{
    abstract class PluginBase
    {
        internal static IPluginLog Log { get; set; }
        public static PluginBase Instance { get; internal set; }
        
        internal static ProfileInfo ProfileInfo;
        
        public abstract Task<string> GetPlatformUserId();

        public static void ErrorPrint(string text)
        {
            PluginBase.Log?.Error(text);
        }
        
        public static void WarnPrint(string text)
        {
            PluginBase.Log?.Warn(text);
        }
        
        public static void WarnPrint(Exception ex)
        {
            PluginBase.Log?.Warn(ex);
        }

        public static void DebugPrint(string text)
        {
            PluginBase.Log?.Error(text);
        }

        public static void DebugNetworkPrint(string text, Enums.Leaderboard leaderBoard)
        {

            switch (leaderBoard)
            {
#if SCORESABERNETWORK
                case Enums.Leaderboard.ScoreSaber:
                    break;
#endif
#if BEATLEADERNETWORK
                case Enums.Leaderboard.BeatLeader:
                    break;
#endif
#if ACCSABERNETWORK
                case Enums.Leaderboard.AccSaber:
                    break;
#endif
#if HITBLOQNETWORK
                case Enums.Leaderboard.HitBloq:
                    break;
#endif
#if ACCSABERRELOADEDNETWORK
                case Enums.Leaderboard.AccSaberReloaded:
                    break;
#endif
                default:
                    return;
            }
            PluginBase.Log?.Error(text);
        }
    }
}
