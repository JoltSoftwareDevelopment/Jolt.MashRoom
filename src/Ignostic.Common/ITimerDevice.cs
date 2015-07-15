using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic.Timing
{
    public interface ITimerDevice
    {
        double  Bpm         { get; set; }
        double  Time        { get; set; }
        double  Length      { get; }
        bool    IsPlaying   { get; }

        void StartPlaying();
        void StopPlaying();
    }
}
