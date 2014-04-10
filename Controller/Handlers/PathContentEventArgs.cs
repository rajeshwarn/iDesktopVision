using System;
using System.Collections.Generic;

namespace Controller.Handlers
{
    public class PathContentEventArgs : EventArgs
    {
        public string Path { get; private set; }
        public Dictionary<string, long> PathListing { get; private set; }

        public PathContentEventArgs(string path, Dictionary<string, long> listing)
        {
            Path = path;
            PathListing = listing;
        }
    }
}