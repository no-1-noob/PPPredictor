using System;

namespace PPPredictor.Shared.Data
{
    //http://answers.unity.com/answers/1580674/view.html
    [Serializable]
    struct SVector3
    {
        public float x;
        public float y;
        public float z;

        public SVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
            => $"[x, y, z]";


        public static SVector3 operator +(SVector3 a, SVector3 b)
            => new SVector3(a.x + b.x, a.y + b.y, a.z + b.z);

        public static SVector3 operator -(SVector3 a, SVector3 b)
            => new SVector3(a.x - b.x, a.y - b.y, a.z - b.z);

        public static SVector3 operator -(SVector3 a)
            => new SVector3(-a.x, -a.y, -a.z);

        public static SVector3 operator *(SVector3 a, float m)
            => new SVector3(a.x * m, a.y * m, a.z * m);

        public static SVector3 operator *(float m, SVector3 a)
            => new SVector3(a.x * m, a.y * m, a.z * m);

        public static SVector3 operator /(SVector3 a, float d)
            => new SVector3(a.x / d, a.y / d, a.z / d);
    }
}
