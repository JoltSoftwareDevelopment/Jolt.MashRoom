using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Ignostic.Timing
{
    public class NaiveTimerDevice : ITimerDevice
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Stopwatch   _stopwatch;
        private double      _offset;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public double Length { get; set; }
        public double Bpm { get; set; }
        
        
        public bool IsPlaying
        {
            get { return _stopwatch.IsRunning; }
            set
            {
                if (value)
                    _stopwatch.Start();
                else
                    _stopwatch.Stop();
            }
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public NaiveTimerDevice()
        {
            _offset = 0;
            _stopwatch = new Stopwatch();
            Length = 10 * 60;
            Bpm = 120;
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public double Time 
        {
            get { return _offset + 0.001D * _stopwatch.ElapsedMilliseconds; }
            set
            {
                _offset = value;
                _stopwatch = new Stopwatch();
            }
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public void StartPlaying()
        {
            _stopwatch.Start();
        }


        public void StopPlaying()
        {
            _stopwatch.Stop();
        }
    }
}
