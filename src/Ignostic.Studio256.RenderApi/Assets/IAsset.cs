using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace Ignostic.Studio256.RenderApi
{
    public interface IAsset : IDisposable
    {
        string Name { get; }
    }


    public interface IAsset<T> : IAsset
    {
        T Value { get; }
    }
}
