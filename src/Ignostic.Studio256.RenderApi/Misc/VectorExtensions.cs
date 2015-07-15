using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using System.Globalization;

namespace Ignostic.Studio256.RenderApi
{
    public static class VectorExtensions
    {
        public static Vector4[] AsVector4(this IEnumerable<Vector3> vectors, float w)
        {
            return vectors
                .Select(v3 => new Vector4(v3, w))
                .ToArray();
        }


        public static float[] AsFloat(this IEnumerable<string> args)
        {
            return args
                .Select(arg => float.Parse(arg, CultureInfo.InvariantCulture))
                .ToArray();
        }

        
        public static int[] AsInt(this IEnumerable<string> args)
        {
            return args
                .Select(arg => int.Parse(arg, CultureInfo.InvariantCulture))
                .ToArray();
        }


        public static Vector3 RotateAround(this Vector3 position, Vector3 axis, float angle)
        {
            return Vector3.TransformCoordinate(position, Matrix.RotationAxis(axis, angle));
        }
    }
}
