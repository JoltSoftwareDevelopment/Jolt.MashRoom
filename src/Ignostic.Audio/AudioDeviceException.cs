using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace Ignostic.Audio
{
    public class AudioDeviceException : Exception
    {
        public AudioDeviceException(string message)
            : base(message) 
        {
        }
    }
}
