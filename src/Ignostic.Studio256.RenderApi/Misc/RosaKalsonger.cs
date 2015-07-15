using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Studio256.RenderApi
{
    public class RosaKalsonger
    {
        public PrimitiveTopology PrimitiveTopology { get; set; }
        public InputLayout VanillaInputLayout { get; set; }
        public RasterizerState FillState { get; set ; }
        public DepthStencilView DepthStencilView { get; set; }
        public RenderTarget PostInputRenderTarget { get; set; }
        public ShaderEnvironment ShaderEnvironment { get; private set; }
    }
}
