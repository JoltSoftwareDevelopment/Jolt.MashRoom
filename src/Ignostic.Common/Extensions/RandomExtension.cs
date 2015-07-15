using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic
{
    public static class RandomExtension
    {
        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }


        // instead use extension method from sharpdx
        //public static float NextFloat(this Random random, float min, float max)
        //{
        //    return (float)(min + (max - min) * random.NextDouble());
        //}


        public static float NextFloat(this Random random, float max)
        {
            return (float)(max * random.NextDouble());
        }

        
        //public static Vector4 NextVector4(this Random random)
        //{
        //    return new Vector4(NextFloat(random), NextFloat(random), NextFloat(random), NextFloat(random));
        //}
    }
}
