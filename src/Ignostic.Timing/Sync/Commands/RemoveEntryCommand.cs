using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Timing.Sync.Commands
{
    class RemoveEntryCommand : ISyncCommand
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public CommandId Id         { get; private set; }
        public int       TrackIndex { get; private set; }
        public int       RowIndex   { get; private set; }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public RemoveEntryCommand(int trackIndex, int rowIndex)
        {
            Id = CommandId.DeleteValue;
            TrackIndex = trackIndex;
            RowIndex = rowIndex;
        }
    }
}
