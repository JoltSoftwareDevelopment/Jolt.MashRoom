using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;

namespace Ignostic.Studio256.RenderApi
{
    public class ShaderIncludeHandler : Include
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private ShaderManager _manager;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public ShaderIncludeHandler(ShaderManager manager)
        {
            _manager = manager;
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public IDisposable Shadow
        {
            get;
            set;
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public byte[] ReadAllBytes(string fileName)
        {
            var path = Path.Combine(_manager.RootPath, fileName);
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var text = reader.ReadToEnd();
                return Encoding.ASCII.GetBytes(text);
            }
        }


        public Stream Open(IncludeType type, string path, Stream parentStream)
        {
            var bytes = ReadAllBytes(path);
            var stream = new MemoryStream(bytes);
            return stream;
        }

        
        public void Close(Stream stream)
        {
            stream.Close();
        }


        void IDisposable.Dispose()
        {
        }
    }
}
