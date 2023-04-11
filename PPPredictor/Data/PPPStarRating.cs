using beatleaderapi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Data
{
    public class PPPStarRating
    {
        private double _stars = 0;
        private double _predictedAcc = 0;
        private double _passRating = 0;
        private double _accRating = 0;
        private double _techRating = 0;
        private Dictionary<string, double> _modifiersRating = new Dictionary<string, double>();

        [DefaultValue(0)]
        public double Stars { get => _stars; set => _stars = value; }
        [DefaultValue(0)]
        public double PredictedAcc { get => _predictedAcc; set => _predictedAcc = value; }
        [DefaultValue(0)]
        public double PassRating { get => _passRating; set => _passRating = value; }
        [DefaultValue(0)] 
        public double AccRating { get => _accRating; set => _accRating = value; }
        [DefaultValue(0)]
        public double TechRating { get => _techRating; set => _techRating = value; }
        [DefaultValue(null)]
        public Dictionary<string, double> ModifiersRating { get => _modifiersRating; set => _modifiersRating = value; }

        public PPPStarRating()
        {
        }

        public PPPStarRating(double starRating)
        {
            _stars = _predictedAcc = _passRating = _accRating = _techRating = starRating;
        }

        public PPPStarRating(DifficultyDescription beatLeaderDifficulty)
        {
            _predictedAcc = beatLeaderDifficulty.PredictedAcc.GetValueOrDefault();
            _passRating = beatLeaderDifficulty.PassRating.GetValueOrDefault();
            _accRating = beatLeaderDifficulty.AccRating.GetValueOrDefault();
            _techRating = beatLeaderDifficulty.TechRating.GetValueOrDefault();
            _modifiersRating = beatLeaderDifficulty.ModifiersRating ?? null;
        }
        
        public bool IsRanked()
        {
            return _predictedAcc > 0 || _passRating > 0 || _accRating > 0 || _techRating > 0 || _stars > 0;
        }
    }
}
