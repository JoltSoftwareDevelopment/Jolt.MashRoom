using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic.Studio256.RenderApi
{
    public class Noise
    {
        private Random _random;
        private float[] _data;

        
        public Noise(int size)
        {
            _random = new Random(1415926535);
            _data = new float[size];
        }


        public float[] Data
        {
            get { return _data; }
        }


        //private float GetX(float t)
        //{
        //    var y = t * (_data.Length - 1);
        //    var i0 = (int)Math.Floor(y);
        //    var i1 = Math.Min(i0 + 1, _data.Length-1);
        //    var x0 = _data[i0];
        //    var x1 = _data[i1];
        //    var x = FMath.Lerp(x0, x1, y - i0);
        //    return x;
        //}

        //public float[] CalculateSpline(int size)
        //{
        //    var spline = new float[size];
        //    for (int i = 0; i < size; i++)
        //    {
        //        var t = 1F * i / size;
        //        spline[i] = GetX(t);
        //    }
        //    return spline;
        //}


        public void Recalculate()
        {
            Recurse(0, _data.Length - 1);
        }


        public void Recurse(int i0, int i2)
        {
            var i1 = (i0 + i2) / 2;
            var x0 = _data[i0];
            var x2 = _data[i2];
            var x1 = 0.5F * /*data[i1] + 0.25F * */ (x0 + x2);
            //var dx = Math.Abs(x0 - x2);
            var dy = 1F * (i2 - i1) / _data.Length;
            var dx = dy;

            var r = 0F;
            r += (float)_random.NextDouble();
            r -= 0.5F;
            r *= 2;

            //var r = (float)(1 - 2 * random.NextDouble() * random.NextDouble());
            _data[i1] = 0.5F * _data[i1] + 0.5F * (x1 + r * dx);
            if (i1 - i0 > 1)
                Recurse(i0, i1);
            if (i2 - i1 > 1)
                Recurse(i1, i2);
        }
    }
}
