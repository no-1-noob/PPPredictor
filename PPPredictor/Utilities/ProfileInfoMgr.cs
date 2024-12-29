using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
using Newtonsoft.Json;
using PPPredictor.Core.DataType.LeaderBoard;
using PPPredictor.Data;
using PPPredictor.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace PPPredictor.Utilities
{
    class ProfileInfoMgr
    {
        internal static FlowCoordinator ParentFlow { get; private set; }
        private static PPPredictorFlowCoordinator _flow;
        private static readonly string profilePath = Path.Combine(UnityGame.UserDataPath, "PPPredictorProfileInfo.json");
        private static readonly int _profileInfoVersion = 5;
        internal static ProfileInfo LoadProfileInfo()
        {
            ProfileInfo info;
            if (File.Exists(profilePath))
            {
                try
                {
                    info = JsonConvert.DeserializeObject<ProfileInfo>(File.ReadAllText(profilePath));
                    if (info.ProfileInfoVersion < _profileInfoVersion) info.ResetCachedData(); //If I need to refetch all data because of datastructure changes
                }
                catch (Exception ex)
                {
                    Plugin.Log?.Warn(ex);
                    Plugin.Log?.Warn("Unable to load Profile from file. Creating new Profile.");
                    info = new ProfileInfo();
                }
            }
            else
            {
                Plugin.Log?.Debug("Unable to load Profile from file. Creating new Profile.");
                info = new ProfileInfo();
            }
            return info;
        }

        internal static bool SaveProfile(ProfileInfo profile)
        {
            bool saved = true;
            try
            {
                profile.ProfileInfoVersion = _profileInfoVersion;
                profile.ClearOldMapInfos();
                File.WriteAllText(profilePath, JsonConvert.SerializeObject(profile, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                }));
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint(ex.Message);
                Plugin.ErrorPrint("Error saving Config");
                saved = false;
            }
            return saved;
        }

        internal static void ResetSettings()
        {
            List<PPPLeaderboardInfo> lsCachedData = Plugin.ProfileInfo.LsLeaderboardInfo;
            Plugin.ProfileInfo = new ProfileInfo();
            Plugin.ProfileInfo.LsLeaderboardInfo = lsCachedData;
        }

        internal static void ResetCache()
        {
            Plugin.ProfileInfo.LsLeaderboardInfo = new List<PPPLeaderboardInfo>();
        }

        public static void ShowSettingsFlow()
        {
            if (_flow == null)
                _flow = BeatSaberUI.CreateFlowCoordinator<PPPredictorFlowCoordinator>();

            ParentFlow = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();

            BeatSaberUI.PresentFlowCoordinator(ParentFlow, _flow, immediately: true);
        }
    }
}
