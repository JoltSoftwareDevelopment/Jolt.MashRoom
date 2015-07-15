//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.InteropServices;

//namespace Jolt
//{
//    public partial struct Vertex
//    {
//        [StructLayout(LayoutKind.Sequential, Pack = 1)]
//        public struct Position4
//        {
//            private SharpDX.Vector4 _value;
//            public static implicit operator Position4(SharpDX.Vector3 v3)
//            {
//                return new Position4 { _value = new SharpDX.Vector4(v3, 1) };
//            }
//        }
//    }
//}
