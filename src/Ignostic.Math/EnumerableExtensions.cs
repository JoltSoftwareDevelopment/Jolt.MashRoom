using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Math
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Vector3> TransformPosition(this IEnumerable<Vector3> positions, Matrix matrix)
        {
            return positions.Select(p => Vector3.TransformCoordinate(p, matrix));
        }

    }
}
