using System;
using System.Drawing;

namespace Controller.Handlers
{
    public class DesktopEventArgs : EventArgs
    {
        public Bitmap Image { get; private set; }
        public Point Block { get; private set; }

        public DesktopEventArgs(Bitmap image, Point block)
        {
            Image = image;
            Block = block;
        }
    }
}