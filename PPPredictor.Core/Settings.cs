using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core
{
    public class Settings
    {
        internal static PPGainCalculationType PpGainCalculationType;
        internal static MapPoolSorting HitbloqMapPoolSorting;
        internal static string platformUserId;
        internal static int RefetchMapInfoAfterDays;

        private bool isScoreSaberEnabled;
        private bool isBeatLeaderEnabled;
        private bool isHitbloqEnabled;
        private bool isAccSaberEnabled;
        private string userId;

        public Settings(bool isScoreSaberEnabled, bool isBeatLeaderEnabled, bool isHitbloqEnabled, bool isAccSaberEnabled, string userId)
        {
            this.isScoreSaberEnabled = isScoreSaberEnabled;
            this.isBeatLeaderEnabled = isBeatLeaderEnabled;
            this.isHitbloqEnabled = isHitbloqEnabled;
            this.isAccSaberEnabled = isAccSaberEnabled;
            this.userId = userId;
        }

        public bool IsScoreSaberEnabled { get => isScoreSaberEnabled; set => isScoreSaberEnabled = value; }
        public bool IsBeatLeaderEnabled { get => isBeatLeaderEnabled; set => isBeatLeaderEnabled = value; }
        public bool IsHitbloqEnabled { get => isHitbloqEnabled; set => isHitbloqEnabled = value; }
        public bool IsAccSaberEnabled { get => isAccSaberEnabled; set => isAccSaberEnabled = value; }
        public string UserId { get => userId; set => userId = value; }
    }
}

