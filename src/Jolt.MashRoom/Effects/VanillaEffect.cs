using Ignostic;
using Ignostic.Studio256.RenderApi;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jolt.Cuberick.Effects
{
    public class VanillaEffect : IEffect
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
        private Demo _demo;
        private Disposer _disposer;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public VanillaEffect(Demo demo)
        {
            _demo = demo;
            _disposer = new Disposer();
        }


        public VanillaEffect Init()
        {
            // vertex stuff
            VertexShader = _demo.ShaderManager["vanillaPlane.vs.cso"];
            InputLayout = _disposer.Add(new VanillaInputLayout(VertexShader));
            PrimitiveTopology = PrimitiveTopology.TriangleList;

            // pixel stuff
            // todo: perhaps add pixelshader
            PixelShader = null;

            //
            return this;
        }


        public void Update()
        {
        }


        // todo: perhaps render should be part of some Scene rather than Effect?
        public void Render()
        {
            // vertex stuff
            _demo.DeviceContext.VertexShader.Set(VertexShader.VertexShader);
            _demo.DeviceContext.InputAssembler.InputLayout = InputLayout.InputLayout;
            _demo.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology;

            // pixel stuff
            _demo.DeviceContext.PixelShader.Set(PixelShader.PixelShader);

            var env = _demo.RenderContext.ShaderEnvironment;
            //var lerp = Math.Min(time.Lerp, 1);
            //env.View = Matrix.LookAtLH(new Vector3(0, 0, -7), new Vector3(0, 0, 90), Vector3.UnitY);
            _demo.Cameras[0].ViewAngle = (float)Math.PI / 16;
            _demo.Cameras[0].LookAt(new Vector3(0, 0, -7), new Vector3(0, 0, 90), Vector3.UnitY);

            // draw plane
            var model = _demo.ModelManager["plane"];
            model.Matrix = Matrix.RotationX(FMath.PI / 2);
            _demo.Draw(model, _demo.Cameras[0]);
        }


        public void Dispose()
        {
            _disposer.DisposeAll();
        }
    }
}
