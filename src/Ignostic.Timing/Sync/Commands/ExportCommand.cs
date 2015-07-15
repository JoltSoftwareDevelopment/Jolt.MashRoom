using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Timing.Sync.Commands
{
    class ExportCommand : ISyncCommand
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public CommandId Id { get; private set; }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public ExportCommand()
        {
            Id = CommandId.Export;
        }
    }
}
