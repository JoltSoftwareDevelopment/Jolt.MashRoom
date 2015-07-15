using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX;

namespace Ignostic.Studio256.RenderApi
{
    public class RenderContext : IDisposable
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Disposer _disposer;
        private RenderTarget _renderTarget;

        
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public RenderContext()
        {
            _disposer = new Disposer();
            ShaderEnvironment = _disposer.Add(new ShaderEnvironment());
        }


        void IDisposable.Dispose()
        {
            Disposer.Dispose(ref _disposer);
            // TODO
        }

        
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public PrimitiveTopology PrimitiveTopology { get; set; }
        public InputLayout InputLayout { get; set; }
        public VertexShader VertexShader { get; set; }
        public PixelShader PixelShader { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public DepthStencilView DepthStencilView { get; set; }
        public ShaderEnvironment ShaderEnvironment { get; private set; }
        public RenderTarget RenderTarget { get; set; }
    }
}
