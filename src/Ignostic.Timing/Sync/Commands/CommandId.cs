using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Ignostic.Timing.Sync.Commands
{
    /****************************************************************************************************
     * 
     ****************************************************************************************************/
    public enum CommandId
    {
        AddValue = 0,
        DeleteValue = 1,
        RequestTrack = 2,
        SetRowIndex = 3,
        SetPause = 4,
        Export = 5,
    };
}
