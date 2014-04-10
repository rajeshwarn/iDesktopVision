using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Controller.Handlers;
using Controller.Network.TCP;

namespace Controller
{
    class Slave
    {
        public event EventHandler<SlaveEventArgs> SlaveInfoUpdated = delegate { };

        public Client Client { get; private set; }
        public Explorer Explorer { get; private set; }
        public Desktop Desktop { get; private set; }
        public EndPoint RemoteEndPoint { get; private set; }
        public EndPoint LocalEndPoint { get; private set; }
        public string ComputerName { get; private set; }
        public string UserName { get; private set; }
        public string OS { get; private set; }

        public IntPtr Handle
        {
            get { return Client.ClientHandle; }
        }

        public Slave(Client client)
        {
            Client = client;
            RemoteEndPoint = client.RemoteEndPoint;
            LocalEndPoint = client.LocalEndPoint;

            Explorer = new Explorer(Client);
            Desktop = new Desktop(Client);
        }

        public void UpdateInfo(string computerName, string userName, string os)
        {
            ComputerName = computerName;
            UserName = userName;
            OS = os;

            SlaveInfoUpdated(this, new SlaveEventArgs(this));
        }

        public void Disconnect()
        {
            Client.Disconnect();
        }
    }
}
