using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic.Studio256.RenderApi
{
    public class TimeFrame
    {
        private double _time;

        public double From { get; set; }
        public double To { get; set; }
        public double Absolute { get { return (float)(_time); } }
        public float Relative { get { return (float)(_time - From); } }
        public float Lerp { get { return (float)((_time - From) / (To - From)); } }
        public TimeInterval LastInterval { get { return new TimeInterval { StartTime = From, EndTime = To }; } }


        public TimeFrame(double time)
        {
            _time = time;
        }

        
        public bool Within(double from, double to)
        {
            From = from;
            To = to;
            return (From <= _time && _time < To);
        }

        
        public bool Duration(double duration)
        {
            From = To;
            To = From + duration;
            return (From <= _time && _time < To);
        }

        
        public void InitFrame()
        {
            From = 0;
            To = 0;
        }


        public TimeFrame SubTime()
        {
            return new TimeFrame(_time)
            {
                From = this.From,
                To = this.From,
            };
        }
    }
}
