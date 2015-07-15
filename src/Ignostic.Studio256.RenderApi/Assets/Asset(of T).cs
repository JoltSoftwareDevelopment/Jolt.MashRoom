using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Studio256.RenderApi
{
    public class Asset<T> : IAsset<T> where T : class, IDisposable
    {
        private Device _device;

        public Asset(Device device, string name, T asset)
        {
            if (asset == null)
                throw new ArgumentNullException("asset");
                
            _device = device;
            Value = asset;
            Name = name;
        }

        public string Name { get; private set; }
        public T Value { get; private set; }

        public void Dispose()
        {
            if (Value != null)
            {
                Value.Dispose();
                Value = null;
            }
        }
    }
}
