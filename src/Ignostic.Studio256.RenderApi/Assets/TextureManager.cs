using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Studio256.RenderApi
{
    public class TextureManager : AssetManager<TextureAsset>
    {
        private Device _device;

        public TextureManager(Device device)
        {
            _device = device;
            // todo
            RootPath = @"resources";
        }

        public override TextureAsset Load(string name)
        {
            var resources = GetList(name);
            if (resources.Count > 0)
                return resources.First();

            var path = string.Format(@"{0}\textures\{1}", RootPath, name);
            var texture = Texture2D.FromFile<Texture2D>(_device, path);
            var resource = new TextureAsset(_device, name, texture);
            return resource;
        }
    }
}
