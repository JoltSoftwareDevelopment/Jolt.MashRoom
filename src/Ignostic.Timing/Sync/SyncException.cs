using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Timing.Sync
{
    public class SyncException : Exception
    {
        public SyncException(string message)
            : base(message)
        {
        }
    }
}
