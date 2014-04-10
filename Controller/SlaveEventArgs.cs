using System;

namespace Controller
{
    class SlaveEventArgs : EventArgs
    {
        public Slave Slave { get; private set; }

        public SlaveEventArgs(Slave slave)
        {
            Slave = slave;
        }
    }
}