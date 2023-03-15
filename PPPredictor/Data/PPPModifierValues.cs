using Newtonsoft.Json;
using System.Collections.Generic;

namespace PPPredictor.Data
{
    class PPPModifierValues
    {
        private int id;
        private Dictionary<string, float> dctModifierValues = new Dictionary<string, float>();
        public int Id { get => id; set => id = value; }
        public Dictionary<string, float> DctModifierValues { get => dctModifierValues; set => dctModifierValues = value; }

        public PPPModifierValues(Dictionary<string, float> dctModifierValues)
        {
            this.dctModifierValues = new Dictionary<string, float>();
            //Make em all uppercase...
            foreach (var key in dctModifierValues.Keys)
            {
                this.dctModifierValues[key.ToUpperInvariant()] = dctModifierValues[key];
            }
        }
        [JsonConstructor]
        public PPPModifierValues(int id, Dictionary<string, float> dctModifierValues) : this(dctModifierValues)
        {
            this.id = id;
        }

        public bool Equals(PPPModifierValues other)
        {
            if (dctModifierValues.Count != other.DctModifierValues.Count)
            {
                return false;
            }
            var thisKeys = dctModifierValues.Keys;
            foreach (var key in thisKeys)
            {
                if (!(other.DctModifierValues.TryGetValue(key, out var value) &&
                      dctModifierValues[key] == value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
