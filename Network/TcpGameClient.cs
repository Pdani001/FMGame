using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;

namespace ReFMGame.Network
{
    class TcpGameClient : NetCoreServer.TcpClient
    {
        private readonly ConcurrentQueue<Message> _incoming;
        private readonly MemoryStream _buffer = new();
        public string ErrorMessage { get; private set; } = string.Empty;

        public TcpGameClient(
            string address,
            int port,
            ConcurrentQueue<Message> incoming)
            : base(address, port)
        {
            _incoming = incoming;
        }

        protected override void OnConnected()
        {
            Debug.WriteLine("TCP connected");
            ErrorMessage = string.Empty;
        }

        protected override void OnDisconnected()
        {
            Debug.WriteLine("TCP disconnected");
            _incoming.Clear();
            if(ErrorMessage != string.Empty)
                _incoming.Enqueue(new Message { type = "error", error = ErrorMessage });
            _incoming.Enqueue(new Message { type = "disconnected" });
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            Debug.WriteLine($"Received buffer [{string.Join(", ",buffer)}]");
            _buffer.Write(buffer, (int)offset, (int)size);

            while (true)
            {
                if (_buffer.Length < 4)
                    return;

                _buffer.Position = 0;
                int length = BitConverter.ToInt32(_buffer.GetBuffer(), 0);

                if (_buffer.Length < length + 4)
                    return;

                var jsonBytes = _buffer.GetBuffer().AsSpan(4, length);
                var str = System.Text.Encoding.Default.GetString(jsonBytes);
                Debug.WriteLine($"Valid message: {str}");
                var msg = JsonSerializer.Deserialize<Message>(jsonBytes);


                _incoming.Enqueue(msg);

                var remaining = _buffer.Length - (length + 4);
                var temp = new byte[remaining];
                Array.Copy(_buffer.GetBuffer(), length + 4, temp, 0, remaining);

                _buffer.SetLength(0);
                _buffer.Write(temp, 0, temp.Length);
            }
        }

        protected override void OnError(SocketError error)
        {
            Debug.WriteLine($"TCP error: {error}");
            ErrorMessage = error.ToString();
        }
    }
}
