using System;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core
{
    public class Settings
    {
        private PPGainCalculationType ppGainCalculationType;
        private MapPoolSorting hitbloqMapPoolSorting;
        private string platformUserId;
        private int refetchMapInfoAfterDays;
        private DateTime lastSessionReset;
        private int resetSessionHours;

        private bool isScoreSaberEnabled;
        private bool isBeatLeaderEnabled;
        private bool isHitbloqEnabled;
        private bool isAccSaberEnabled;
        private string userId;

        public Settings(bool isScoreSaberEnabled, bool isBeatLeaderEnabled, bool isHitbloqEnabled, bool isAccSaberEnabled, string userId, PPGainCalculationType ppGainCalculationType, MapPoolSorting hitbloqMapPoolSorting, string platformUserId, int refetchMapInfoAfterDays, DateTime lastSessionReset, int resetSessionHours)
        {
            this.isScoreSaberEnabled = isScoreSaberEnabled;
            this.isBeatLeaderEnabled = isBeatLeaderEnabled;
            this.isHitbloqEnabled = isHitbloqEnabled;
            this.isAccSaberEnabled = isAccSaberEnabled;
            this.userId = userId;
            this.ppGainCalculationType = ppGainCalculationType;
            this.hitbloqMapPoolSorting = hitbloqMapPoolSorting;
            this.platformUserId = platformUserId;
            this.refetchMapInfoAfterDays = refetchMapInfoAfterDays;
            this.lastSessionReset = lastSessionReset;
            this.resetSessionHours = resetSessionHours;
        }

        public bool IsScoreSaberEnabled { get => isScoreSaberEnabled; set => isScoreSaberEnabled = value; }
        public bool IsBeatLeaderEnabled { get => isBeatLeaderEnabled; set => isBeatLeaderEnabled = value; }
        public bool IsHitbloqEnabled { get => isHitbloqEnabled; set => isHitbloqEnabled = value; }
        public bool IsAccSaberEnabled { get => isAccSaberEnabled; set => isAccSaberEnabled = value; }
        public string UserId { get => userId; set => userId = value; }
        public DateTime LastSessionReset { get => lastSessionReset; set => lastSessionReset = value; }
        public int ResetSessionHours { get => resetSessionHours; set => resetSessionHours = value; }
        internal PPGainCalculationType PpGainCalculationType { get => ppGainCalculationType; set => ppGainCalculationType = value; }
        internal MapPoolSorting HitbloqMapPoolSorting { get => hitbloqMapPoolSorting; set => hitbloqMapPoolSorting = value; }
        internal string PlatformUserId { get => platformUserId; set => platformUserId = value; }
        internal int RefetchMapInfoAfterDays { get => refetchMapInfoAfterDays; set => refetchMapInfoAfterDays = value; }
    }
}

