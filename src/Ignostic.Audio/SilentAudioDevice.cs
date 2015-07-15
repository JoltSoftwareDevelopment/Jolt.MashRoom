using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ignostic.Timing;


namespace Ignostic.Audio
{
    internal class SilentAudioDevice : NaiveTimerDevice, IAudioDevice
    {
        public void Init() { }
        public void Load(Stream stream) { }
        public void Dispose() { }

        public double Volume { get; set; }
        public double PlayPosition
        {
            get { return Time; }
            set { Time = value; }
        }
    }
}
