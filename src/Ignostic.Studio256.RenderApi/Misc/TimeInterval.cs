using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic.Studio256.RenderApi
{
    public struct TimeInterval
    {
        public double StartTime;
        public double EndTime;

        
        public double Duration 
        {
            get { return EndTime - StartTime; }
            set { EndTime = StartTime + value; }
        }


        public bool IsBetween(double time)
        {
            return StartTime <= time && time < EndTime;
        }
    }
}
