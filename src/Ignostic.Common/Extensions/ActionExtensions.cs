using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic
{
    public static class ActionExtensions
    {
        public static void InvokeIfNotNull(this Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        
        public static void InvokeIfNotNull<T0>(this Action<T0> action, T0 arg0)
        {
            if (action != null)
            {
                action(arg0);
            }
        }


        public static void InvokeIfNotNull<T0, T1>(this Action<T0, T1> action, T0 arg0, T1 arg1)
        {
            if (action != null)
            {
                action(arg0, arg1);
            }
        }
    }
}
