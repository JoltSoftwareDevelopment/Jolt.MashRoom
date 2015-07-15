using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Ignostic.Studio256.RenderApi
{
    public static class FMath
    {
        public const float PI = (float)Math.PI;

        public static float LCos(float lerpAngle)
        {
            return (float)Math.Cos(2 * Math.PI * lerpAngle);
        }

        public static float LSin(float lerpAngle)
        {
            return (float)Math.Sin(2 * Math.PI * lerpAngle);
        }

        public static float Saturate(float value)
        {
            if (value < 0)
                return 0;
            if (value > 1)
                return 1;
            return value;
        }

        public static Vector3 LPolarXY(float lerpAngle)
        {
            return new Vector3(LCos(lerpAngle), LSin(lerpAngle), 0);
        }

        public static float LerpToRad(float lerp)
        {
            return 2 * PI * lerp;
        }

        public static float Lerp(float min, float max, float lerp)
        {
            return min + lerp * (max - min);
        }

        public static float Clamp(this float value, float min, float max)
        {
            return Math.Min(max, Math.Max(min, value));
        }
    }
}
