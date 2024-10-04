using System;
using System.Collections.Generic;

namespace PPPredictor.Utilities
{
    class ParsingUtil
    {
        static readonly Dictionary<string, int> dctDifficultyNameToInt = new Dictionary<string, int>{
            { "EXPERTPLUS", 9 },
            { "EXPERT", 7 },
            { "HARD", 5 },
            { "NORMAL", 3 },
            { "EASY", 1 }
        };

        public static int ParseDifficultyNameToInt(string difficulty)
        {
            try
            {
                return dctDifficultyNameToInt[difficulty.ToUpper()];
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"Error in ParseDifficultyNameToInt could not parse {difficulty}, {ex.Message}");
            }
            return -1;
        }
    }
}
