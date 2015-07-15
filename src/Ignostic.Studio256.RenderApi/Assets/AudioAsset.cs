using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Studio256.RenderApi
{
    public class AudioAsset : IAsset<Stream>
    {
        private Stream _stream;

        public AudioAsset(string name, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
                
            _stream = stream;
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public Stream Value
        {
            get { return _stream; }
        }

        public void Dispose()
        {
            Disposer.Dispose(ref _stream);
        }
    }
}
