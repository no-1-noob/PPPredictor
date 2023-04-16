using static PPPredictor.OpenAPIs.BeatleaderAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PPPredictor.Data
{
    internal class PPPStarRating
    {
        private double _stars = 0;
        private double _multiplier = 1;
        private double _predictedAcc = 0;
        private double _passRating = 0;
        private double _accRating = 0;
        private double _techRating = 0;
        private Dictionary<string, double> _modifiersRating = new Dictionary<string, double>();
        private Dictionary<string, double> _modifierValues = new Dictionary<string, double>();

        [DefaultValue(0d)]
        public double Stars { get => _stars; set => _stars = value; }
        [DefaultValue(0d)]
        public double PredictedAcc { get => _predictedAcc; set => _predictedAcc = value; }
        [DefaultValue(0d)]
        public double PassRating { get => _passRating; set => _passRating = value; }
        [DefaultValue(0d)] 
        public double AccRating { get => _accRating; set => _accRating = value; }
        [DefaultValue(0d)]
        public double TechRating { get => _techRating; set => _techRating = value; }
        [DefaultValue(null)]
        public Dictionary<string, double> ModifiersRating { get => _modifiersRating; set => _modifiersRating = value; }
        [DefaultValue(null)]
        public Dictionary<string, double> ModifierValues { get => _modifierValues; set => _modifierValues = value; }
        [JsonIgnoreAttribute]
        [DefaultValue(1)]
        public double Multiplier { get => _multiplier; set => _multiplier = value; }

        internal PPPStarRating()
        {
        }

        internal PPPStarRating(double starRating)
        {
            _stars = _predictedAcc = _passRating = _accRating = _techRating = starRating;
        }

        internal PPPStarRating(BeatLeaderDifficulty beatLeaderDifficulty)
        {
            _predictedAcc = beatLeaderDifficulty.predictedAcc.GetValueOrDefault();
            _passRating = beatLeaderDifficulty.passRating.GetValueOrDefault();
            _accRating = beatLeaderDifficulty.accRating.GetValueOrDefault();
            _techRating = beatLeaderDifficulty.techRating.GetValueOrDefault();
            _modifiersRating = beatLeaderDifficulty.modifiersRating ?? null;
            _modifierValues = beatLeaderDifficulty.modifierValues ?? null;
        }

        internal PPPStarRating(double mulitplier, double accRating, double passRating, double techRating)
        {
            _multiplier = mulitplier;
            _accRating = accRating;
            _passRating = passRating;
            _techRating = techRating;
        }

        internal bool IsRanked()
        {
            return _predictedAcc > 0 || _passRating > 0 || _accRating > 0 || _techRating > 0 || _stars > 0;
        }
    }
}
