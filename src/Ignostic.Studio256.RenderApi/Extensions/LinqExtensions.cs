using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Ignostic
{
    public static class LinqExtensions
    {
        public static Vector4 Average(this IEnumerable<Vector4> vectors)
        {
            Vector4 sum = Vector4.Zero;
            int count = 0;
            foreach (var vector in vectors)
            {
                sum += vector;
                count++;
            }

            return sum / count;
        }
    }
}
