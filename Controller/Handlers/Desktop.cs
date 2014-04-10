using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller.Network.Packets;
using Controller.Network.TCP;

namespace Controller.Handlers
{
    public class Desktop
    {
        private readonly Client _client;
        private ulong _frames;
        private DateTime _startTime;
        private int _quality;
        private Dictionary<Point, uint> _blockHashes;
        private bool _stopped;

        public Size BlockSize { get; private set; }
        public Size Blocks { get; private set; }

        public Size Resolution
        {
            get
            {
                return new Size(BlockSize.Width*Blocks.Width, BlockSize.Height*Blocks.Height);
            }
        }

        public int Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                if (value > 100)
                    _quality = 100;
                else if (value < 0)
                    _quality = 0;

                _quality = value;
            }
        }

        public double FPS
        {
            get
            {
                var elapsed = DateTime.Now.Subtract(_startTime).TotalSeconds;
                if (_frames == 0)
                    return 0;

                if (elapsed < 5.5)
                    return _frames/elapsed;

                _startTime = DateTime.Now;
                _frames = 1;

                return _frames/elapsed;
            }
        }

        public event EventHandler<DesktopEventArgs> FrameUpdated = delegate { };
        public event EventHandler Acknowledged = delegate { }; 

        public Desktop(Client client)
        {
            _client = client;
            _frames = 0;

            Blocks = new Size(0, 0);
        }

        public void Start()
        {
            Synchronize(new Size(3, 3));
        }

        public void Stop()
        {
            _stopped = true;
        }

        public void MouseDown(Point point, MouseButtons buttons)
        {
            var packet = new ClickPacket(point, buttons, PressEvent.Down);
            _client.SendPacket(packet);
        }

        public void MouseUp(Point point, MouseButtons buttons)
        {
            var packet = new ClickPacket(point, buttons, PressEvent.Up);
            _client.SendPacket(packet);
        }

        public void ChangeBlocks(Size blocks)
        {
            Synchronize(blocks);
        }

        public void PacketReceived(Packet packet)
        {
            if (packet is AckScreensharePacket)
            {
                var ackPacket = (AckScreensharePacket) packet;

                Blocks = ackPacket.Blocks;
                BlockSize = ackPacket.BlockSize;

                _blockHashes = new Dictionary<Point, uint>(Blocks.Height * Blocks.Width);
                Acknowledged(this, EventArgs.Empty);
                
                for (var x = 0; x < Blocks.Width; x++)
                {
                    for (var y = 0; y < Blocks.Height; y++)
                    {
                        var point = new Point(x, y);
                        _blockHashes[point] = uint.MaxValue;
                    }
                }

                var random = new Random();
               // foreach (var block in _blockHashes.OrderBy(x => random.Next()).Select(x => x.Key))
               //     RequestBlock(block, Quality);
            }
            else if (packet is SendScreenshotBlockPacket)
            {
                var blockPacket = (SendScreenshotBlockPacket) packet;
                ++_frames;
                if (blockPacket.Image == null) //Same image
                {
                    RequestBlock(blockPacket.Block, Quality);
                    return;
                }

                Trace.WriteLine(String.Format("Compressed: {0}", blockPacket.Compressed));
                var frame = (Bitmap)blockPacket.Image.Clone();
                
                FrameUpdated(this, new DesktopEventArgs(frame, blockPacket.Block));
                blockPacket.Image.Dispose();

                _blockHashes[blockPacket.Block] = blockPacket.BlockHash;

               // RequestBlock(blockPacket.Block, Quality);
            }
        }

        public void ClearCache()
        {
            for (var x = 0; x < Blocks.Width; x++)
            {
                for (var y = 0; y < Blocks.Height; y++)
                {
                    _blockHashes[new Point(x, y)] = uint.MaxValue;
                }
            }
        }

        public void Synchronize(Size blocks)
        {
            _stopped = false;

            Packet packet = new SckScreensharePacket(blocks, (uint)Quality);
            _client.SendPacket(packet);
            _startTime = DateTime.Now;
        }

        private void RequestBlock(Point block, int quality)
        {
            if (_stopped) return; //Nooo, stop it!
            var packet = new RequestScreenshotBlockPacket(block, _blockHashes[block], (uint)quality);
            _client.SendPacket(packet);
        }
    }
}
