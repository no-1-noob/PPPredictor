using HarmonyLib;
using IPA;
using PPPredictor.Converter;
using PPPredictor.Core.DataType;
using PPPredictor.Installers;
using PPPredictor.Manager;
using PPPredictor.Shared.Data;
using PPPredictor.UI.ViewController;
using PPPredictor.Utilities;
using SiraUtil.Zenject;
using System.Reflection;
using System;
using System.Threading.Tasks;
using IPALogger = IPA.Logging.Logger;

namespace PPPredictor.Shared
{
    public abstract class PluginBase
    {
        public static PluginBase Instance { get; internal set; }
        internal static ConverterBase Converter { get; set; }

        internal static ProfileInfo ProfileInfo = new ProfileInfo();
        internal static IPALogger Log;
        
        internal static PPPredictorViewController pppViewController;

        private const string kHarmonyID = "com.github.no-1-noob.PPPredictor";
        private static readonly Harmony harmony = new Harmony(kHarmonyID);

        internal abstract void CreateConverter();

        //Only Used for UnitTests
        internal PluginBase()
        {
            Instance = this;
            ProfileInfo = new ProfileInfo();
        }
        
        public PluginBase(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            ProfileInfo = ProfileInfoMgr.LoadProfileInfo();
            zenjector.UseSiraSync();
            zenjector.Install<PPPPredictorDisplayInstaller>(Location.Menu);
            zenjector.Install<MainMenuInstaller>(Location.Menu);
            zenjector.Install<CoreInstaller>(Location.App);
            zenjector.Install<GamePlayInstaller>(Location.StandardPlayer | Location.CampaignPlayer);
        }
        
        public void OnApplicationStart()
        {
            Log?.Debug("OnApplicationStart.");
            CreateConverter();
            ApplyHarmonyPatches();
        }
        
        public void OnApplicationQuit()
        {
            ProfileInfoMgr.SaveProfile(ProfileInfo, PPPredictorMgr.CalculatorInstance.GetSaveData());
        }
        
        public void OnDisable()
        {
            RemoveHarmonyPatches();
        }

        private static void ApplyHarmonyPatches()
        {
            try
            {
                Log?.Debug("Applying Harmony patches.");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Log?.Error("Error applying Harmony patches: " + ex.Message);
                Log?.Debug(ex);
            }
        }

        

        private static void RemoveHarmonyPatches()
        {
            try
            {
                harmony.UnpatchSelf();
            }
            catch (Exception ex)
            {
                Log?.Error("Error removing Harmony patches: " + ex.Message);
                Log?.Debug(ex);
            }
        }

        public async Task<string> GetPlatformUserId()
        {
            UserInfo user = await BS_Utils.Gameplay.GetUserInfo.GetUserAsync();
            return user.platformUserId;
        }

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
