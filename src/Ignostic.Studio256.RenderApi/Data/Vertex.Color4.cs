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
//        public struct Color4
//        {
//            private SharpDX.Vector4 _value;
//            public static implicit operator Color4(SharpDX.Vector4 v4)
//            {
//                return new Color4 { _value = v4 };
//            }
//        }
//    }
//}
