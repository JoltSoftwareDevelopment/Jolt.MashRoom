using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Timing.Sync.Commands
{
    public interface ICommandHandler
    {
        void AddValue(int trackIndex, TrackEntry trackEntry);
        void DeleteValue(int trackIndex, int rowIndex);
        void SetRowIndex(int rowIndex);
        void SetPause(bool isPaused);
        void Export();
    }
}
