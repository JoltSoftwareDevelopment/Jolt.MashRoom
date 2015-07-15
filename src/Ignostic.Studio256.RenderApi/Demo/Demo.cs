using System;
using System.Threading;
using System.Windows.Forms;
using Ignostic.Timing;
using Jolt;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Ignostic.Timing.Sync;

namespace Ignostic.Studio256.RenderApi
{
    public class Demo : IDisposable
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Disposer _disposer;
        private RenderContext _renderContext;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public Demo()
        {
            _disposer = new Disposer();
            RenderHandle = _disposer.Add(new AutoResetEvent(false));
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public Form             Form                { get; set; }
        public Device           Device              { get; private set; }
        public ModelManager     ModelManager        { get; private set; }
        public TextureManager   TextureManager      { get; private set; }
        public SrvManager       SrvManager          { get; private set; }
        public ShaderManager    ShaderManager       { get; private set; }
        public AudioManager     AudioManager        { get; private set; }
        public bool             OutputWasResized    { get; set; }
        public Camera           ManualCamera        { get; set; }
        public Camera[]         Cameras             { get; private set; }
        public int              CameraIndex         { get; set; }
        public bool             UseManualCamera     { get; set; }
        public DeviceContext    DeviceContext       { get; set; }
        public SwapChain        SwapChain           { get; set; }
        //public TimeFrame        TimeFrame           { get; set; }
        public ITimerDevice     Timer               { get; set; }
        public ISetupModel      SetupModel          { get; set; }
        public WaitHandle       RenderHandle        { get; private set; }
        public SyncManager      SyncManager         { get; set; }


        /****************************************************************************************************
         * Refactored stuff that probably should be elsewhere
         ****************************************************************************************************/
        // todo
        #region elsewhere
        public RosaKalsonger RosaKalsonger { get; set; }
        #endregion


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public RenderContext RenderContext
        {
            get
            {
                return _renderContext;
            }
            set
            {
                _renderContext = value;
                DeviceContext.InputAssembler.InputLayout = _renderContext.InputLayout;
                DeviceContext.InputAssembler.PrimitiveTopology = _renderContext.PrimitiveTopology;
                DeviceContext.VertexShader.Set(_renderContext.VertexShader);
                DeviceContext.PixelShader.Set(_renderContext.PixelShader);
                DeviceContext.Rasterizer.State = _renderContext.RasterizerState;
                //deviceContext.OutputMerger.SetTargets(_renderContext.DepthStencilView);
                //deviceContext.OutputMerger.SetTargets(_renderContext.RenderTarget.RenderTargetView);
                DeviceContext.OutputMerger.SetTargets(_renderContext.DepthStencilView, _renderContext.RenderTarget.RenderTargetView);
                DeviceContext.Rasterizer.SetViewport(_renderContext.RenderTarget.Viewport);

                // TODO refactor
                _renderContext.ShaderEnvironment.ScreenAspectRatio = _renderContext.RenderTarget.AspectRatio;
            }
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public Demo Init(SetupModel setupModel, string title)
        {
            SetupModel = setupModel;
            Form = _disposer.Add(new Form1
            {
                Text = title,
                Width = SetupModel.Mode.Width / 1, // todo
                Height = SetupModel.Mode.Height / 1, // todo
            });
            Form.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                {
                    
                }
            };
            setupModel.Factory.MakeWindowAssociation(Form.Handle, WindowAssociationFlags.IgnoreAll);
            Device = new Device
            (
                setupModel.Adapter,
                setupModel.DeviceDebugMode ? DeviceCreationFlags.Debug : DeviceCreationFlags.None,
                setupModel.RequiredFeatureLevel.Value
            );
            DeviceContext = _disposer.Add(Device.ImmediateContext);
            ModelManager = _disposer.Add(new ModelManager());
            AudioManager = _disposer.Add(new AudioManager());
            TextureManager = _disposer.Add(new TextureManager(Device));
            SrvManager = _disposer.Add(new SrvManager(this));
            ShaderManager = _disposer.Add(new ShaderManager(Device));
            OutputWasResized = true;
            Cameras = new[] { new Camera(), new Camera(), new Camera(), new Camera() };
            Timer = new NaiveTimerDevice
            {
                Time = SetupModel.StartTime,
            };
            return this;
        }


        public void Draw(Model model, ICamera camera)
        {
            if (model.Buffer == null)
                throw new NullReferenceException();
                //return;

            // TODO refactor
            //RenderContext.Apply(DeviceContext);

            var env = RenderContext.ShaderEnvironment;
            env.Resolution = new Vector2(RenderContext.RenderTarget.Width, RenderContext.RenderTarget.Height);
            //env.LerpTime = TimeFrame.Lerp;
            //env.AbsoluteTime = (float)TimeFrame.Absolute;
            env.LerpTime = (float)SyncManager.Data.LerpTime;
            env.AbsoluteTime = (float)Timer.Time;
            env.BeatTime = (float)(Timer.Time / 60 * Timer.Bpm);
            env.UpdateBuffer(DeviceContext, model.Matrix, camera);
            env.Lead = (float)SyncManager.Data.Lead;
            env.Nisse0 = (float)SyncManager.Data.Nisse0;
            env.Nisse1 = (float)SyncManager.Data.Nisse1;
            env.Nisse2 = (float)SyncManager.Data.Nisse2;
            env.Nisse3 = (float)SyncManager.Data.Nisse3;

            DeviceContext.VertexShader.SetConstantBuffer(0, env.Buffer);
            DeviceContext.PixelShader.SetConstantBuffer(0, env.Buffer);
            DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(model.Buffer, Vertex.Size, 0));
            DeviceContext.Draw(model.Positions.Count, 0);
        }

        public void DrawIndexed(Matrix modelMatrix, Buffer vertexBuffer, Buffer indexBuffer, int count, ICamera camera, int size) 
        {
            var env = RenderContext.ShaderEnvironment;
            env.Resolution = new Vector2(RenderContext.RenderTarget.Width, RenderContext.RenderTarget.Height);
            env.AbsoluteTime = (float)Timer.Time;
            env.UpdateBuffer(DeviceContext, modelMatrix, camera);

            DeviceContext.VertexShader.SetConstantBuffer(0, env.Buffer);
            DeviceContext.PixelShader.SetConstantBuffer(0, env.Buffer);
            DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, size, 0));
            DeviceContext.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
            DeviceContext.DrawIndexed(count, 0, 0);
        }



        public void Draw(string modelName, ICamera camera)
        {
            Draw(ModelManager[modelName], camera);
        }


        public void Render()
        {
            // TODO
            SwapChain.Present(SetupModel.UseVerticalSync ? 1 : 0, PresentFlags.None);
        }


        void IDisposable.Dispose()
        {
            DeviceContext.ClearState();
            DeviceContext.Flush();

            Disposer.Dispose(ref _disposer);
        }
    }
}
