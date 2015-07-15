using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Ignostic
{
    public static class VectorExtensions
    {
        public static Vector3 XYZ(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector3 Sum(this IEnumerable<Vector3> vectors)
        {
            return vectors.Aggregate(Vector3.Zero, (a, v) => a + v);
        }

        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            return vectors.Sum() / vectors.Count();
        }
    }
}
