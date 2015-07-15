using Ignostic.Studio256.RenderApi;
using Jolt.Cuberick.Effects;
using SharpDX;
using SharpDX.Direct3D11;

namespace Jolt.MashRoom.Effects
{
    public class EndCreditsEffect : Effect
    {
        private float _textureHeight;

        public EndCreditsEffect(Demo demo)
            : base(demo)
        {
        }

        public override Effect Init(EffectDescription description)
        {
            var effect = base.Init(description);
            var texture = _textures[0];
            _textureHeight = texture.Texture.Description.Height;
            return effect;
        }

        protected override TextureAddressMode GetTextureAddressMode()
        {
            return TextureAddressMode.Border;
        }

        // todo: perhaps render should be part of some Scene rather than Effect?
        public override void Render()
        {
            // vertex stuff
            _demo.DeviceContext.VertexShader.Set(VertexShader.VertexShader);
            _demo.DeviceContext.InputAssembler.InputLayout = InputLayout.InputLayout;
            _demo.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology;

            // pixel stuff
            _demo.DeviceContext.PixelShader.Set(PixelShader.PixelShader);
            _demo.DeviceContext.PixelShader.SetShaderResources(0, _resourceViews);
            _demo.DeviceContext.PixelShader.SetSamplers(0, _samplers);

            // model stuff
            var model = _demo.ModelManager["plane"];
            model.Matrix = Matrix.RotationX(FMath.PI / 2);

            //
            var outroTime = (float)_demo.SyncManager.Data.OutroTime;
            var outroFade = (float)_demo.SyncManager.Data.OutroFade;
            var env = _demo.RenderContext.ShaderEnvironment;
            env.Matrices[10] = new Matrix(new[]
            {
                _textureHeight, outroTime, outroFade, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
            });

            _demo.Cameras[0].ViewAngle = FMath.PI / 16;
            _demo.Cameras[0].LookAt(new Vector3(0, 0, -7), new Vector3(0, 0, 90), Vector3.UnitY);
            _demo.Draw(model, _demo.Cameras[0]);
        }
    }
}
