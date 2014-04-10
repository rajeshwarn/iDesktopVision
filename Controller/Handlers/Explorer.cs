using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Controller.Network.Packets;
using Controller.Network.TCP;

namespace Controller.Handlers
{
    public class Explorer
    {
        private readonly Client _client;

        public event EventHandler<PathContentEventArgs> PathContentReceived = delegate { };
        public event EventHandler<FileEventArgs> FileReceived = delegate { };

        public string CurrentDictionary { get; private set; }

        public Explorer(Client client)
        {
            _client = client;
        }

        public void RequestPathContent(string remotePath)
        {
            var packet = new RequestPathContentPacket(remotePath);
            _client.SendPacket(packet);
        }

        public void RequestFile(string remotePath)
        {
            var packet = new RequestFilePacket(remotePath);
            _client.SendPacket(packet);
        }

        public void SendFile(string localPath, int chunkSize = 8196)
        {
            var fileSize = new FileInfo(localPath).Length;
            long toRead;
            long totalRead = 0;
            var buffer = new byte[chunkSize];
            using (var fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read))
            {
                while ((toRead = fileSize - totalRead) > 0)
                {
                    toRead = toRead > chunkSize ? chunkSize : toRead;
                    var read= fileStream.Read(buffer, 0, (int) toRead);

                    var packet = new SendFilePacket(buffer, (ulong)totalRead, (ulong)read, localPath);
                    _client.SendPacket(packet);

                    totalRead += read;
                }
            }
        }

        public void PacketReceived(Packet packet)
        {
            if (packet is SendPathContentPacket) //controller
            {
                var path = ((SendPathContentPacket)packet).Path;
                var pathListing = ((SendPathContentPacket)packet).PathListing;

                CurrentDictionary = path;
                PathContentReceived(this, new PathContentEventArgs(path, pathListing));
            }
            else if (packet is SendFilePacket) //controller
            {
                var filePacket = (SendFilePacket)packet;
                
                var path = Path.GetTempPath() + Path.GetFileNameWithoutExtension(filePacket.Path) + ".txt";
                if (filePacket.Count == 0) //Mindfuck? No it is just magic numbers saying it is done :D
                {
                    Process.Start(path);
                }
                else
                {
                    using (var fileStream = File.Open(path, filePacket.Offset == 0 ? FileMode.Create : FileMode.Append))
                    {
                        fileStream.Write(filePacket.Content, 0, (int) filePacket.Count);
                    }
                }
            }
        }
    }
}
