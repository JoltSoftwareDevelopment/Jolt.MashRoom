﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }
    }
}
