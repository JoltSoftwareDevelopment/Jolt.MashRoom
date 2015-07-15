using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using Ignostic.Studio256.RenderApi;

namespace Ignostic.Studio256.RenderApi
{
    public class ShaderManager : AssetManager<ShaderAsset>
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Device _device;
        private ShaderIncludeHandler _shaderIncludeHandler;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public ShaderManager(Device device)
        {
            RootPath = @"resources\shaders";
            _device = device;
            _shaderIncludeHandler = new ShaderIncludeHandler(this);
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        // todo remove runtime compiling (i.e. hlsl loading)
        public override ShaderAsset Load(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            switch (ext)
            {
                case ".cso":
                    return LoadPreCompiled(fileName);
                case ".hlsl":
                    return LoadSourceCode(fileName);
                default:
                    throw new NotSupportedException();
            }
        }


        // todo remove
        [Obsolete]
        private ShaderAsset LoadSourceCode(string fileName)
        {
            var shader = new ShaderAsset(_device, _shaderIncludeHandler);
            var list = GetList(fileName);
            list.Add(shader);
            shader.LoadFromFile(fileName);
            return shader;
        }

        
        private ShaderAsset LoadPreCompiled(string fileName)
        {
            var shader = new ShaderAsset(_device, _shaderIncludeHandler);
            var list = GetList(fileName);
            list.Add(shader);

            var path = Path.Combine(RootPath, fileName);
            shader.LoadCompiledShader(path);
            return shader;
        }
    }
}