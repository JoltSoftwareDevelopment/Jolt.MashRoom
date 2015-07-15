using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace Ignostic.Audio
{
    public class AudioStreamException : Exception
    {
        public AudioStreamException(string message)
            : base(message) 
        {
        }
        
        
        internal AudioStreamException(BASSError errorCode)
             : this("Bass error: " + errorCode)
        {
        }
    }
}
