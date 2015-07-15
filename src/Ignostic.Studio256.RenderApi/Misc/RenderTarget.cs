using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Drawing;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Ignostic.Studio256.RenderApi
{
    public class RenderTarget : IDisposable
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public int                  Width               { get { return Texture.Description.Width; } }
        public int                  Height              { get { return Texture.Description.Height; } }
        public float                AspectRatio         { get { return (float)Width / Height; } }
        public Texture2D            Texture             { get; set; }
        public RenderTargetView     RenderTargetView    { get; set; }
        public ShaderResourceView   ShaderResourceView  { get; set; }
        public Viewport             Viewport            { get; set; }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Disposer _disposer;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private RenderTarget()
        {
            _disposer = new Disposer();
        }


        public RenderTarget(Device device, int width, int height, int sampleCount, int sampleQuality, Format format)
            : this()
        {
            Texture = _disposer.Add(new Texture2D(device, new Texture2DDescription
            {
                Format              = format,
                Width               = width,
                Height              = height,
                ArraySize           = 1,
                MipLevels           = 1,
                BindFlags           = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags      = CpuAccessFlags.None,
                OptionFlags         = ResourceOptionFlags.None,
                Usage               = ResourceUsage.Default,
                SampleDescription   = new SampleDescription(sampleCount, sampleQuality),
            }));
            RenderTargetView = _disposer.Add(new RenderTargetView(device, Texture, new RenderTargetViewDescription
            {
                Format = format,
                Dimension = RenderTargetViewDimension.Texture2DMultisampled,
                //MipSlice = 0,
            }));
            
            ShaderResourceView = _disposer.Add(new ShaderResourceView(device, Texture));
            Viewport = new Viewport(0, 0, width, height, 0.0f, 1.0f);
        }


        public RenderTarget(Device device, SwapChain swapChain)
            : this()
        {
            Texture = _disposer.Add(Texture2D.FromSwapChain<Texture2D>(swapChain, 0));
            RenderTargetView = _disposer.Add(new RenderTargetView(device, Texture));
            ShaderResourceView = null;
            Viewport = new Viewport(0, 0, Width, Height, 0.0f, 1.0f);
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public void Clear(DeviceContext context, Color4 color4)
        {
            context.ClearRenderTargetView(RenderTargetView, color4);
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        void IDisposable.Dispose()
        {
            Disposer.Dispose(ref _disposer);
        }
    }
}
