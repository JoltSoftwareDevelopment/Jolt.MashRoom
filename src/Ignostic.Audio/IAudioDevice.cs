using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ignostic.Timing;

namespace Ignostic.Audio
{
    public interface IAudioDevice : ITimerDevice, IDisposable
    {
        // Position and Length in seconds
        double PlayPosition { get; set; }
        double Length       { get; }
        double Volume       { get; set; }

        void Init();
        void Load(Stream stream);
        void StartPlaying();
        void StopPlaying();
    }
}
