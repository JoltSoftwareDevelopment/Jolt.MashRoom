using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Extensions
{
    public static class StringExtensions
    {
        public static string TrimEnd(this string input, string suffix)
        {
            if (input == null)
                return input;

            if (suffix == null)
                return input;

            if (!input.EndsWith(suffix))
                return input;

            return input.Substring(0, input.Length - suffix.Length);
        }
    }
}
