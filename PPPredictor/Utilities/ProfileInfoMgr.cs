﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;
using IPA.Utilities;
using Newtonsoft.Json;
using PPPredictor.Data;
using PPPredictor.UI;
using System;
using System.IO;

namespace PPPredictor.Utilities
{
    class ProfileInfoMgr
    {
        internal static FlowCoordinator _parentFlow { get; private set; }
        internal static PPPredictorFlowCoordinator _flow { get; private set; }
        private static readonly string profilePath = Path.Combine(UnityGame.UserDataPath, "PPPredictorProfileInfo.json");
        private static int _profileInfoVersion = 1;
        internal static ProfileInfo LoadProfileInfo()
        {
            MenuButtons.instance.RegisterButton(new MenuButton("PPPredictor", "Predict PP gains", ShowSettingsFlow, true));
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
                File.WriteAllText(profilePath, JsonConvert.SerializeObject(profile, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error(ex);
                Plugin.Log?.Error("Error saving Config");
                saved = false;
            }
            return saved;
        }

        internal static void ResetProfile()
        {
            Plugin.ProfileInfo = new ProfileInfo();
        }

        public static void ShowSettingsFlow()
        {
            if (_flow == null)
                _flow = BeatSaberUI.CreateFlowCoordinator<PPPredictorFlowCoordinator>();

            _parentFlow = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();

            BeatSaberUI.PresentFlowCoordinator(_parentFlow, _flow, immediately: true);
        }
    }
}
