using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Studio256.RenderApi
{
    public class AudioManager : AssetManager<AudioAsset>
    {
        public AudioManager()
        {
            // todo
            RootPath = @"resources\audio";
        }

        public override AudioAsset Load(string name)
        {
            var resources = GetList(name);
            if (resources.Count > 0)
                return resources.First();

            var path = string.Format(@"{0}\{1}", RootPath, name);
            var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var resource = new AudioAsset(name, stream);
            return resource;
        }
    }
}
