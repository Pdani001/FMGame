using NetCoreServer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReFMGame.Network
{
    class WsGameClient : WsClient
    {
        private readonly ConcurrentQueue<Message> _incoming;
        public string ErrorMessage { get; private set; } = string.Empty;

        public WsGameClient(
            string address,
            int port,
            ConcurrentQueue<Message> incoming)
            : base(address, port)
        {
            _incoming = incoming;
        }

        public override void OnWsConnected(HttpResponse response)
        {
            Debug.WriteLine("Connected to server");
            ErrorMessage = string.Empty;
        }

        public override void OnWsDisconnected()
        {
            Debug.WriteLine("Disconnected from server");
            _incoming.Clear();
            _incoming.Enqueue(new Message { Type = "disconnected" });
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            var json = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            var msg = JsonSerializer.Deserialize<Message>(json);
            _incoming.Enqueue(msg);
        }

        public override void OnWsError(string error)
        {
            Debug.WriteLine($"WebSocket error: {error}");
            ErrorMessage = error;
        }
    }
}
