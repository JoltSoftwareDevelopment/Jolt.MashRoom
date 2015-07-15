using Ignostic.Timing.Sync.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Timing.Sync
{
    class CommandDispatcher
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private ICommandHandler _handler;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public CommandDispatcher(ICommandHandler handler)
        {
            _handler = handler;
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        /// <summary>
        /// Calls the handler for the command
        /// </summary>
        public void DispatchCommand(ISyncCommand command)
        {
            { var arg = command as AddEntryCommand;     if (arg != null) { _handler.AddValue    (arg.TrackIndex, arg.TrackEntry); return; } }
            { var arg = command as RemoveEntryCommand;  if (arg != null) { _handler.DeleteValue (arg.TrackIndex, arg.RowIndex);   return; } }
            { var arg = command as SetRowIndexCommand;  if (arg != null) { _handler.SetRowIndex (arg.RowIndex);                   return; } }
            { var arg = command as SetPauseCommand;     if (arg != null) { _handler.SetPause    (arg.IsPaused);                   return; } }
            { var arg = command as ExportCommand;       if (arg != null) { _handler.Export      ();                               return; } }
            throw new SyncException(string.Format("Unknown Sync Command: {0}", command.GetType().Name));
        }
    }
}
