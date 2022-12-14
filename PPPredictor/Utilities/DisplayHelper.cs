namespace PPPredictor.Utilities
{
    class DisplayHelper
    {
        public static string GetDisplayColor(double value, bool invert, bool isPPGainWithVergeOption = false)
        {
            if (invert) value *= -1;
            if (value > 0)
            {
                return $"green";
            }
            else if (isPPGainWithVergeOption && Plugin.ProfileInfo.PpGainCalculationType == PPGainCalculationType.Raw && value < 0 && value >= Plugin.ProfileInfo.RawPPLossHighlightThreshold)
            {
                return $"yellow";
            }
            else if (value < 0)
            {
                return $"red";
            }
            return "white";
        }
    }
}
