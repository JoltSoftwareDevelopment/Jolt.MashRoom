using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Timing.Sync.Commands
{
    class AddEntryCommand : ISyncCommand
    {
        public CommandId  Id          { get; private set; }
        public int        TrackIndex  { get; private set; }
        public TrackEntry TrackEntry  { get; private set; }


        public AddEntryCommand(int trackIndex, TrackEntry trackEntry)
        {
            Id = CommandId.AddValue;
            TrackIndex = trackIndex;
            TrackEntry = trackEntry;
        }


        public void Execute(ICommandHandler handler)
        {
            handler.AddValue(TrackIndex, TrackEntry);
        }   
    }
}
