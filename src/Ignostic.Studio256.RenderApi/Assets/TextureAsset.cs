using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Studio256.RenderApi
{
    public class TextureAsset : IAsset
    {
        private Device _device;
        private Texture2D _texture;

        public TextureAsset(Device device, string name, Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
                
            _device = device;
            _texture = texture;
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public Texture2D Texture
        {
            get { return _texture; }
        }

        public void Dispose()
        {
            Disposer.Dispose(ref _texture);
        }
    }
}
