using System;
using System.Collections.Generic;
using System.IO;

namespace Ignostic.Timing.Sync
{
    public class TrackEntry
    {
        public int               RowIndex          { get; set; }
        public float             Value             { get; set; }
        public InterpolationType InterpolationType { get; set; }
    }
}
