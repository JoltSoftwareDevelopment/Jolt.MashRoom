using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T item) where T : class
        {
            var count = (item != null) ? 1 : 0;
            var enumerable = Enumerable.Repeat(item, count);
            return enumerable;
        }

        
        public static IEnumerable<TOut> AsEnumerable<TIn, TOut>(this TIn item, Func<TIn, TOut> selector) where TIn : class
        {
            return item
                .AsEnumerable()
                .Select(selector);
        }
    }
}
