using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Studio256.RenderApi
{
    public class SrvManager : AssetManager<IAsset<ShaderResourceView>>
    {
        private Demo _demo;

        public SrvManager(Demo demo)
        {
            _demo = demo;
        }

        public override IAsset<ShaderResourceView> Load(string name)
        {
            var resources = GetList(name);
            if (resources.Count > 0)
                return resources.First();

            var textureAsset = _demo.TextureManager.Load(name);
            var asset = new Asset<ShaderResourceView>(_demo.Device, name, new ShaderResourceView(_demo.Device, textureAsset.Texture));
            Add(asset);

            return asset;
        }
    }
}
