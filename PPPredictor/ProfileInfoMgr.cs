using IPA.Utilities;
using Newtonsoft.Json;
using System;
using System.IO;

namespace PPPredictor
{
    class ProfileInfoMgr
    {
        internal static readonly string profilePath = Path.Combine(UnityGame.UserDataPath, "PPPredictorProfileInfo.json");
        internal static ProfileInfo loadProfileInfo()
        {
            ProfileInfo info;
            if (File.Exists(profilePath))
            {
                try
                {
                    info = JsonConvert.DeserializeObject<ProfileInfo>(File.ReadAllText(profilePath));
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
                File.WriteAllText(profilePath, JsonConvert.SerializeObject(profile, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error(ex);
                Plugin.Log?.Error("Error saving Config");
                saved = false;
            }
            return saved;
        }
    }
}
