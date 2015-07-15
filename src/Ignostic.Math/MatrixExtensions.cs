using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Math
{
    public static class MatrixExtensions
    {
        public static Vector3 TransformPosition(this Matrix matrix, Vector3 position)
        {
            return Vector3.TransformCoordinate(position, matrix);
        }
    }
}
