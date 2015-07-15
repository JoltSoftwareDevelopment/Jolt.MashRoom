using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Ignostic.Timing.Sync
{
    public class SyncData : DynamicObject
    {
        private SyncManager _manager;


        public SyncData(SyncManager manager)
        {
            _manager = manager;
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            switch (binder.Name)
            {
                case "Time":
                    result = (float)_manager.TimerDevice.Time;
                    return true;

                case "RowIndex":
                    result = (float)_manager.RowIndex;
                    return true;

                default:
                    result = _manager.GetValue(binder.Name);
                    return true;
            }
        }
    }
}
