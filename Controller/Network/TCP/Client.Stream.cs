using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Network.TCP
{
    public partial class Client
    {
        private readonly Encoding _stringEncoding = Encoding.Default;
        
        internal Rectangle ReadRectangle()
        {
            return new Rectangle(ReadInt(), ReadInt(), ReadInt(), ReadInt());
        }

        internal void WriteRectangle(Rectangle rect)
        {
            WriteInt(rect.X);
            WriteInt(rect.Y);
            WriteInt(rect.Width);
            WriteInt(rect.Height);
        }

        internal Size ReadSize()
        {
            return new Size(ReadInt(), ReadInt());
        }

        internal void WriteSize(Size size)
        {
            WriteInt(size.Width);
            WriteInt(size.Height);
        }

        internal Point ReadPoint()
        {
            return new Point(ReadInt(), ReadInt());
        }

        internal void WritePoint(Point point)
        {
            WriteInt(point.X);
            WriteInt(point.Y);
        }

        internal Dictionary<string, long> ReadDictionaryStringLong()
        {
            var count = ReadInt();
            var stringLongs = new Dictionary<string, long>(count);
            for (var i = 0; i < count; i++)
            {
                stringLongs[ReadString()] = ReadLong();
            }

            return stringLongs;
        }

        internal void WriteDictionaryStringLong(Dictionary<string, long> stringLongs)
        {
            WriteInt(stringLongs.Count);
            foreach (var keyPair in stringLongs)
            {
                WriteString(keyPair.Key);
                WriteLong(keyPair.Value);
            }
        }

        internal async Task<byte> ReadByteAsync()
        {
            var buffer = new byte[1];
            return await ReadAsync(buffer, 0, 1) != 0 ? buffer[0] : (byte) 0xFF;
        }

        internal async Task WriteByteAsync(byte value)
        {
            var buffer = new[] {value};
            await WriteAsync(buffer, 0, 1);
        }

        internal byte ReadByte()
        {
            var buffer = new byte[1];
            return Read(buffer, 0, 1) != 0 ? buffer[0] : (byte) 0xFF;
        }

        internal void WriteByte(byte value)
        {
            var buffer = new[] {value};
            Write(buffer, 0, 1);
        }
        
        internal unsafe int ReadInt()
        {
            var buf = new byte[4];
            Read(buf, 0, buf.Length);
            fixed (byte* b = buf)
                return *(int*) b;
        }

        internal unsafe void WriteInt(int value)
        {
            var buf = new byte[4];
            fixed (byte* b = buf)
                *(int*) b = value;

            Write(buf, 0, buf.Length);
        }

        internal unsafe long ReadLong()
        {
            var buf = new byte[8];
            Read(buf, 0, buf.Length);
            fixed (byte* b = buf)
                return *(long*)b;
        }

        internal unsafe void WriteLong(long value)
        {
            var buf = new byte[8];
            fixed (byte* b = buf)
                *(long*)b = value;

            Write(buf, 0, buf.Length);
        }
        
        internal byte[] ReadByteArray()
        {
            var length = ReadInt();
            if (length == 0) return null;
            var result = new byte[length];
            var read = 0;
            while ((read += Read(result, read, length - read)) < length) ;
            return result;
        }

        internal void WriteByteArray(byte[] value)
        {
            WriteByteArray(value, 0, value.Length);
        }

        internal void WriteByteArray(byte[] value, int offset, int count)
        {
            WriteInt(count);
            var sent = 0;
            while ((sent += Write(value, offset + sent, count - sent)) < count) ;
        }
        
        internal bool ReadBool()
        {
            return ReadByte() != 0;
        }

        internal void WriteBool(bool value)
        {
            WriteByte(value ? (byte)1 : (byte)0);
        }

        internal string ReadString()
        {
            var data = ReadByteArray();
            return _stringEncoding.GetString(data);
        }

        internal void WriteString(string value)
        {
            WriteByteArray(_stringEncoding.GetBytes(value));
        }

        internal int Read(byte[] buffer, int offset, int count, bool writeAll = true)
        {
            int read;
            try
            {
                read = _socket.Receive(buffer, offset, count, SocketFlags.None);
                if (writeAll && read < count)
                    while ((read += _socket.Receive(buffer, offset + read, count - read, SocketFlags.None)) < count) ;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(String.Format("Exception: " + ex), "Client.Read");
                Disconnect();
                read = -1;
            }

            return read;
        }

        internal int Write(byte[] buffer, int offset, int count, bool writeAll = true)
        {
            int sent;
            try
            {
                sent = _socket.Send(buffer, offset, count, SocketFlags.None);
                if (writeAll && sent < count)
                    while ((sent += _socket.Send(buffer, offset + sent, count - sent, SocketFlags.None)) < count) ;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(String.Format("Exception: " + ex), "Client.Write");
                Disconnect();
                sent = -1;
            }

            return sent;
        }

        internal async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            var eventArgs = new SocketAsyncEventArgs();
            var tcs = new TaskCompletionSource<int>();
            EventHandler<SocketAsyncEventArgs> eventHandler = null;
            eventHandler = (sender, args) => HandlerAsync(tcs, args, eventHandler);
            eventArgs.SetBuffer(buffer, offset, count);
            eventArgs.Completed += eventHandler;

            if (_socket.ReceiveAsync(eventArgs))
                return await tcs.Task;

            var result = eventArgs.BytesTransferred;
            eventArgs.Completed -= eventHandler;
            return result;
        }

        internal async Task<int> WriteAsync(byte[] buffer, int offset, int count)
        {
            var eventArgs = new SocketAsyncEventArgs();
            var tcs = new TaskCompletionSource<int>();
            EventHandler<SocketAsyncEventArgs> eventHandler = null;
            eventHandler = (sender, args) => HandlerAsync(tcs, args, eventHandler);
            eventArgs.SetBuffer(buffer, offset, count);
            eventArgs.Completed += eventHandler;

            if (_socket.SendAsync(eventArgs))
                return await tcs.Task;

            var result = eventArgs.BytesTransferred;
            eventArgs.Completed -= eventHandler;
            return result;
        }

        internal void HandlerAsync(TaskCompletionSource<int> tcs, SocketAsyncEventArgs eventArgs, EventHandler<SocketAsyncEventArgs> onCompletion)
        {
            var error = eventArgs.SocketError;
            if (error != SocketError.Success)
                tcs.SetException(new SocketException((int)error));
            else
                tcs.SetResult(eventArgs.BytesTransferred);
            
            eventArgs.Completed -= onCompletion;
        }
    }
}
