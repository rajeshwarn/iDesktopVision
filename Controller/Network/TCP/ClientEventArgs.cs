using System;

namespace Controller.Network.TCP
{
    public class ClientEventArgs : EventArgs
    {
        public Client Client { get; private set; }

        public ClientEventArgs(Client client)
        {
            Client = client;
        }
    }
}