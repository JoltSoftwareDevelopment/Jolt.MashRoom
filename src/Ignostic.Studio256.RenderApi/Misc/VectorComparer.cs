using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Ignostic.Studio256.RenderApi
{
    public class Vector3Comparer : IEqualityComparer<Vector3>
    {
        private float _maximumDistance;

        public Vector3Comparer(int maximumDistance)
        {
        }

        public bool Equals(Vector3 x, Vector3 y)
        {
            return Vector3.Distance(x, y) < _maximumDistance;
        }

        public int GetHashCode(Vector3 obj)
        {
            return EqualityComparer<Vector3>.Default.GetHashCode(obj);
        }
    }
}
