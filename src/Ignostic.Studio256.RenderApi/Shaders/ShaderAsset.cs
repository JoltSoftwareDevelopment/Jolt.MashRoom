using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

namespace Ignostic.Studio256.RenderApi
{
    // TODO rename Shader class
    public class PixelShaderAsset : ShaderAsset
    {
        public PixelShaderAsset(Device device, ShaderIncludeHandler includeHandler)
            : base (device, includeHandler) { }
    }

    public class VertexShaderAsset : ShaderAsset
    {
        public VertexShaderAsset(Device device, ShaderIncludeHandler includeHandler)
            : base (device, includeHandler) { }
    }

    // todo make abstract
    public class ShaderAsset : IAsset, IDisposable
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Disposer _disposer;
        private ShaderBytecode _vertexShaderByteCode;
        private ShaderBytecode _pixelShaderByteCode;
        private ShaderIncludeHandler _includeHandler;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public ShaderAsset(Device device, ShaderIncludeHandler includeHandler)
        {
            Device = device;
            _includeHandler = includeHandler;
            _disposer = new Disposer();
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public Device           Device          { get; private set; }
        public string           Name            { get; private set; }
        // todo refactor
        public VertexShader     VertexShader    { get; private set; }
        // todo refactor
        public PixelShader      PixelShader     { get; private set; }
        public ShaderSignature  Signature       { get; private set; }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private ShaderBytecode Compile(byte[] bytes, string functionName, string profile)
        {
            // TODO optimize shader compilation for release mode
            // TODO perhaps use fxc.exe for shader compilation
            
            return ShaderBytecode.Compile(
                shaderSource:   bytes,
                entryPoint:     functionName,
                profile:        profile,
                shaderFlags:    ShaderFlags.None, 
                effectFlags:    EffectFlags.None, 
                defines:        null,
                include:        _includeHandler
            );
        }


        public void LoadFromFile(string fileName)
        {
            var bytes = _includeHandler.ReadAllBytes(fileName);
            _vertexShaderByteCode = _disposer.Add(Compile(bytes, "VS", "vs_4_0"));
            _pixelShaderByteCode = _disposer.Add(Compile(bytes, "PS", "ps_4_0"));
            VertexShader = _disposer.Add(new VertexShader(Device, _vertexShaderByteCode.Data));
            PixelShader = _disposer.Add(new PixelShader(Device, _pixelShaderByteCode.Data));
            Signature = ShaderSignature.GetInputSignature(_vertexShaderByteCode);
        }

        
        public void LoadCompiledShader(string path)
        {
            var shaderByteCode = _disposer.Add(ShaderBytecode.FromFile(path));
            if (path.EndsWith(".vs.cso"))
            {
                _vertexShaderByteCode = shaderByteCode;
                VertexShader = _disposer.Add(new VertexShader(Device, _vertexShaderByteCode.Data));
                Signature = ShaderSignature.GetInputSignature(_vertexShaderByteCode);
            }
            else if (path.EndsWith(".ps.cso"))
            {
                _pixelShaderByteCode = shaderByteCode;
                PixelShader = _disposer.Add(new PixelShader(Device, _pixelShaderByteCode.Data));
            }
            else
            {
                throw new NotSupportedException();
            }
        }


        void IDisposable.Dispose()
        {
            Disposer.Dispose(ref _disposer);
        }
    }
}
