using Ignostic.Studio256.RenderApi;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Studio256.RenderApi
{
    public class VanillaInputLayout : IDisposable
    {
        public InputLayout InputLayout { get; private set; }


        public VanillaInputLayout(ShaderAsset shader)
        {
            InputLayout  = new InputLayout(shader.Device, shader.Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 32, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 48, 0),
            });
        }


        public void Dispose()
        {
            InputLayout.Dispose();
            InputLayout = null;
        }
    }
}
