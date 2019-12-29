using System;

namespace DGen.Generation.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string s)
        {
            return Char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
    }
}
