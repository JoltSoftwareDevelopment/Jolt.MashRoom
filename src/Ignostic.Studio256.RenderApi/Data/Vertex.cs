using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SharpDX;

namespace Jolt
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    // todo perhaps rename to VanillaVertex
    public partial struct Vertex
    {
        public Vector4  Position;
        public Vector4  Normal;
        public Color4   Color;
        public Vector4  Custom0;

        public static int Size { get { return Utilities.SizeOf<Vertex>(); } }
    }
}
