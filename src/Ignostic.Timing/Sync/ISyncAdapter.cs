using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Ignostic.Timing.Sync.Commands;

namespace Ignostic.Timing.Sync
{
    interface ISyncAdapter : IDisposable
    {
        void Update();
        void SetRowIndex(int rowIndex);
        SyncTrack RequestTrack(string trackName);
    }
}
