using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Timing.Sync.Commands
{
    class RequestTrackCommand : ISyncCommand
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public CommandId Id        { get; private set; }
        public string    TrackName { get; private set; }

        
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public RequestTrackCommand(string trackName)
        {
            Id = CommandId.RequestTrack;
            TrackName = trackName;
        }
    }
}
