using System;

namespace Controller.Handlers
{
    public class FileEventArgs : EventArgs
    {
        public string TempPath { get; private set; }

        public FileEventArgs(string path)
        {
            TempPath = path;
        }
    }
}