using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using Ignostic.Studio256.RenderApi;
using Jolt.MashRoom.Effects.ProceduralModels;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Jolt.Cuberick.Effects
{
    public class GreetingsEffect : Effect
    {
        private float _textureHeight;
        private InputLayout _tubeInputLayout;
        private ShaderAsset _tubeVertexShader;
        private ShaderAsset _tubePixelShader;
        private Vertex[] _tubeVerticies;
        private int[] _tubeIndices;
        private RenderTarget _greetingsRenderTarget;
        private DepthStencilView _greetingsDepthView;
        private Buffer _tubeVertexBuffer;
        private Buffer _tubeIndicesBuffer;
        private ShaderResourceView _greetingsTexture;
        private ShaderResourceView _renderedCylinderTexture;

        public GreetingsEffect(Demo demo)
            : base(demo)
        {
        }

        private void CreateCylinderBuffers()
        {
            //128 segments per ring, 128 height segments.
            Cylinder cyl = new Cylinder(0.5f, 2, 128, 128, false);
            _tubeVerticies = new Vertex[cyl.Vertices.Count];
            for (var i = 0; i < cyl.Vertices.Count; i++)
            {
                _tubeVerticies[i].Position = (cyl.Vertices[i] - Vector3.Up) * new Vector3(1, 1.5F, 1);
                _tubeVerticies[i].TexCoord = cyl.TexCoords[i];
            }
            _tubeIndices = cyl.Indices.ToArray();
            _tubeVertexBuffer = _disposer.Add(Buffer.Create(_demo.Device, BindFlags.VertexBuffer, _tubeVerticies));
            _tubeIndicesBuffer = _disposer.Add(Buffer.Create(_demo.Device, BindFlags.IndexBuffer, _tubeIndices));
        }

        public override Effect Init(EffectDescription description)
        {
            base.Init(description);

            var mode = _demo.SetupModel.Mode;

            _greetingsRenderTarget = _disposer.Add(new RenderTarget(
                device: _demo.Device,
                width: mode.Width,                    // TODO(mstrandh): Honor setupmodel?
                height: mode.Height,                   // TODO(mstrandh): Honor setupmodel?
                sampleCount: 1,                 // TODO(mstrandh): Honor setupmodel?
                sampleQuality: 0,               // TODO(mstrandh): Honor setupmodel?
                format: Format.R8G8B8A8_UNorm   // TODO(mstrandh): Honor setupmodel?
            ));

            CreateCylinderBuffers();
            var texture = _textures[0];
            _textureHeight = texture.Texture.Description.Height;

            _tubeVertexShader = _demo.ShaderManager["greetingsTube.vs.cso"];
            _tubePixelShader = _demo.ShaderManager["greetingsTube.ps.cso"];

            _tubeInputLayout = _disposer.Add(new InputLayout(_demo.Device, _tubeVertexShader.Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0),
            }));

            _greetingsTexture = _resourceViews[0];
            _renderedCylinderTexture = _greetingsRenderTarget.ShaderResourceView;

            // Create the depth buffer
            var depthBuffer = _disposer.Add(new Texture2D(_demo.Device, new Texture2DDescription
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = mode.Width,
                Height = mode.Height,
                SampleDescription = new SampleDescription { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            }));

            // Create the depth buffer view
            _greetingsDepthView = _disposer.Add(new DepthStencilView(_demo.Device, depthBuffer));

            return this;
        }

        protected override TextureAddressMode GetTextureAddressMode()
        {
            return TextureAddressMode.Border;
        }

        public void Update()
        {
        }


        // todo: perhaps render should be part of some Scene rather than Effect?
        public override void Render()
        {
            var RotZ = (float)_demo.SyncManager.Data.RotZ; //0.4
            var YLerp = (float)_demo.SyncManager.Data.YLerp; //0.4
            var posX = (float)_demo.SyncManager.Data.PosX; //0.4
            var posY = (float)_demo.SyncManager.Data.PosY; //0 
            var posZ = (float)_demo.SyncManager.Data.PosZ; //-1.8
            var FoV = (float)_demo.SyncManager.Data.FOV;  // Pi / 4
            var YAngle = (float)_demo.SyncManager.Data.YAngle;  // 180

            var lookAtX = (float)_demo.SyncManager.Data.LookAtX;  // 0
            var lookAtY = (float)_demo.SyncManager.Data.LookAtY;  // 40
            var lookAtZ = (float)_demo.SyncManager.Data.LookAtZ;  // 100

            var radScale = (float)_demo.SyncManager.Data.RadiusScale;  // 100

            _demo.Cameras[0].ViewAngle = FoV;//FMath.PI / 6;
            _demo.Cameras[0].LookAt(new Vector3(posX, posY, posZ), new Vector3(lookAtX, lookAtY, lookAtZ), Vector3.UnitY);

            // vertex stuff
            _demo.DeviceContext.VertexShader.Set(_tubeVertexShader.VertexShader);
            _demo.DeviceContext.InputAssembler.InputLayout = _tubeInputLayout;
            _demo.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // pixel stuff
            _demo.DeviceContext.PixelShader.Set(_tubePixelShader.PixelShader);
            _demo.DeviceContext.PixelShader.SetShaderResources(0, new[] { _greetingsTexture });
            _demo.DeviceContext.PixelShader.SetSamplers(0, _samplers);

            //
            var outroTime = (float)_demo.SyncManager.Data.OutroTime;
            var outroFade = (float)_demo.SyncManager.Data.OutroFade;
            var env = _demo.RenderContext.ShaderEnvironment;
            env.Matrices[10] = new Matrix(new[]
            {
                _textureHeight, outroTime, outroFade, radScale,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
            });

            var dc = _demo.DeviceContext;
            //NOTE(mstrandh): Since we're trying to bind this resource as a Render Target later, we need to unbind it first.

            //Setup for cylinder render
            var depth = _demo.RenderContext.DepthStencilView;

            _demo.DeviceContext.OutputMerger.SetTargets(_greetingsDepthView, _greetingsRenderTarget.RenderTargetView);

            dc.ClearDepthStencilView(_greetingsDepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            dc.ClearRenderTargetView(_greetingsRenderTarget.RenderTargetView, Color.Black);

            var matrix =
                Matrix.RotationZ((float)((MathUtil.TwoPi / 360.0) * RotZ)) *
                Matrix.RotationY((float)((MathUtil.TwoPi / 360.0) * YAngle));
            //Matrix.RotationZ(RotZ);// *
            //Matrix.RotationY((float)(MathUtil.Lerp(0, MathUtil.TwoPi, YLerp)));
            //Matrix.RotationX((float) (180.0 / MathUtil.Pi));


            _demo.DrawIndexed(matrix, _tubeVertexBuffer, _tubeIndicesBuffer, _tubeIndices.Length, _demo.Cameras[0], Utilities.SizeOf<Vertex>());

            //Reset previous state
            _demo.DeviceContext.OutputMerger.SetTargets(depth, _demo.RenderContext.RenderTarget.RenderTargetView);
            _demo.DeviceContext.Rasterizer.SetViewport(_demo.RenderContext.RenderTarget.Viewport);

            // vertex stuff
            _demo.DeviceContext.VertexShader.Set(VertexShader.VertexShader);
            _demo.DeviceContext.InputAssembler.InputLayout = InputLayout.InputLayout;
            _demo.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology;

            // pixel stuff
            _demo.DeviceContext.PixelShader.Set(PixelShader.PixelShader);
            _demo.DeviceContext.PixelShader.SetShaderResources(0, _renderedCylinderTexture);
            _demo.DeviceContext.PixelShader.SetSamplers(0, _samplers);

            // model stuff
            var model = _demo.ModelManager["plane"];
            model.Matrix = Matrix.RotationX(FMath.PI / 2);

            //
            _demo.Cameras[0].ViewAngle = FMath.PI / 16;
            _demo.Cameras[0].LookAt(new Vector3(0, 0, -7), new Vector3(0, 0, 90), Vector3.UnitY);
            _demo.Draw(model, _demo.Cameras[0]);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Vertex
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public Vector3 Position { get; set; }
        public Vector2 TexCoord { get; set; }
    }
}
