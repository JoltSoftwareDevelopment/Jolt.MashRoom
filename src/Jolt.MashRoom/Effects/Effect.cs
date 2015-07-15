using System.Linq;
using Ignostic;
using Ignostic.Studio256.RenderApi;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace Jolt.Cuberick.Effects
{
    public class Effect : IEffect
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public VanillaInputLayout InputLayout;
        public ShaderAsset VertexShader;
        public ShaderAsset PixelShader;
        public PrimitiveTopology PrimitiveTopology;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        protected Demo _demo;
        protected Disposer _disposer;
        protected TextureAsset[] _textures;
        protected ShaderResourceView[] _resourceViews;
        protected SamplerState[] _samplers;
        private EffectDescription description;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public Effect(Demo demo)
        {
            _demo = demo;
            _disposer = new Disposer();
        }


        public virtual Effect Init(EffectDescription description)
        {
            // vertex stuff
            VertexShader = _demo.ShaderManager[description.VertexShaderName ?? "vanillaPlane.vs.cso"];
            InputLayout = _disposer.Add(new VanillaInputLayout(VertexShader));
            PrimitiveTopology = PrimitiveTopology.TriangleList;

            // pixel stuff
            var addressMode = GetTextureAddressMode();
            PixelShader = _demo.ShaderManager[description.PixelShaderName];
            _textures = (description.TextureNames ?? new string[0])
                .Select(textureName => string.IsNullOrEmpty(textureName) 
                    ? null 
                    : _demo.TextureManager[textureName])
                .ToArray();
            _resourceViews = _textures
                .Select(texture => (texture == null) 
                    ? null
                    : _disposer.Add(new ShaderResourceView(_demo.Device, texture.Texture)))
                .ToArray();
            _samplers = _textures
                .Select(texture => (texture == null)
                    ? null
                    : _disposer.Add(new SamplerState(_demo.Device, new SamplerStateDescription() {
                        Filter = Filter.Anisotropic,
                        AddressU = addressMode,
                        AddressV = addressMode,
                        AddressW = addressMode,
                        BorderColor = Color.Black,
                        ComparisonFunction = Comparison.Never,
                        MaximumAnisotropy = 1, // 16
                        MipLodBias = 0,
                        MinimumLod = 0,
                        MaximumLod = 1, // 16
                })))
                .ToArray();

            //
            return this;
        }

        protected virtual TextureAddressMode GetTextureAddressMode()
        {
            return TextureAddressMode.Wrap;
        }

        public void Update()
        {
        }


        // todo: perhaps render should be part of some Scene rather than Effect?
        public virtual void Render()
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
            var env = _demo.RenderContext.ShaderEnvironment;
            _demo.Cameras[0].ViewAngle = FMath.PI / 16;
            _demo.Cameras[0].LookAt(new Vector3(0, 0, -7), new Vector3(0, 0, 90), Vector3.UnitY);
            _demo.Draw(model, _demo.Cameras[0]);
        }


        public void Dispose()
        {
            _disposer.DisposeAll();
        }
    }
}
