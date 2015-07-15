using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Timing.Sync.Commands
{
    class SetRowIndexCommand : ISyncCommand
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public CommandId Id         { get; private set; }
        public int       RowIndex   { get; private set; }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public SetRowIndexCommand(int rowIndex)
        {
            Id = CommandId.SetRowIndex;
            RowIndex = rowIndex;
        }
    }
}
