using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    class ParsingUtil
    {
        static readonly Dictionary<string, int> dctDifficultyNameToInt = new Dictionary<string, int>{
            { "ExpertPlus", 9 },
            { "Expert", 7 },
            { "Hard", 5 },
            { "Normal", 3 },
            { "Easy", 1 }
        };

        public static int ParseDifficultyNameToInt(string difficulty)
        {
            try
            {
                return dctDifficultyNameToInt[difficulty];
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"Error in ParseDifficultyNameToInt could not parse {difficulty}, {ex.Message}");
            }
            return -1;
        }
    }
}
