using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable Safe(this IEnumerable source)
        {
            return source ?? Enumerable.Empty<object>();
        }

        
        public static IEnumerable<T> Safe<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        
        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childSelector)
        {
            return source
                .SelectMany(item => Enumerable
                    .Repeat(item, 1)
                    .Concat(SelectRecursive(childSelector(item), childSelector)));
        }
    }
}
