using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Timing.Sync.Commands
{
    class SetPauseCommand : ISyncCommand
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public CommandId Id       { get; private set; }
        public bool      IsPaused { get; private set; }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public SetPauseCommand(bool isPaused)
        {
            Id = CommandId.SetPause;
            IsPaused = isPaused;
        }
    }
}
