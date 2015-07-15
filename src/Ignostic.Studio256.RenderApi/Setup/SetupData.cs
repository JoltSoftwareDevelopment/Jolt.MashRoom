using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.DXGI;

namespace Jolt
{
    public class SetupData
    {
        public bool             DeviceDebugMode         { get; set; }
        public bool             SyncRecordMode          { get; set; }
        public bool             UseVerticalSync         { get; set; }
        public bool             UseAudio                { get; set; }
        public bool             FullScreen              { get; set; }
        public bool             UseOculus               { get; set; }
        public int              MultiSampleCount        { get; set; }
        public int              MultiSampleQuality      { get; set; }
        public double           StartTime               { get; set; }
        public ModeDescription  Mode                    { get; set; }
        public string           BaseRegistrationEmail   { get; set; }
        public string           BaseRegistrationKey     { get; set; }
    }
}
