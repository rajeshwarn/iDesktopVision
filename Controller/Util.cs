using System;

namespace Controller
{
    internal static class Util
    {
        public static string GetSIPrefix(double bytes)
        {
            var prefixes = new[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" }; //Tera, Peta, Exa, Zotta, Yotta
            var steps = 0;
            while (bytes / 1024 > 0.1)
            {
                bytes /= 1024;
                steps++;
            }

            return String.Format("{0:0.00} {1}", bytes, prefixes[steps]);
        }
    }
}