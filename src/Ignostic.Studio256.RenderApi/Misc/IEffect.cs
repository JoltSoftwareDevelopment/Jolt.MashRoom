using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Studio256.RenderApi
{
    public interface IEffect : IDisposable
    {
        void Update();
        void Render();
    }
}
