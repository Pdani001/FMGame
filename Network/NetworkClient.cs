using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ReFMGame.Network
{
    public class NetworkClient
    {
        private TcpGameClient _client;
        private readonly ConcurrentQueue<Message> _incoming = new();

        public Channel Channel { get; private set; } = null;

        private readonly string Secret = "[9edp!J3qWd4)XWtW#sa@s@>PJaXEW]Ns0FzYi5{WEA4pfCjgbeEU3+exR)+ww2(";

        public Client Self { get; private set; } = null;
        private string Session => Self.Id.ToString();

        public const int PROTOCOL_VERSION = 1;

        public bool IsConnected => _client?.IsConnected ?? false;

        long lastServerTick;

        // Events for game layer
        public event Action ServerSecretAccepted;
        public event Action Connected;
        public event Action<string> ConnectFailed;
        public event Action<string> Error;
        public event Action Disconnected;
        public event Action<Channel[]> ChannelListReceived;
        public event Action<Channel, string> JoinedChannel;
        public event Action<string> LeftChannel;
        public event Action<string> ChannelUserJoined;
        public event Action<string> ChannelUserLeft;
        public event Action<string, string> ChatMessageReceived;

        public NetworkClient(string host, int port)
        {
            _client = new TcpGameClient(host, port, _incoming);
        }

        // ---- Connection ----

        public void Connect(string? host, int port = 7121)
        {
            if (IsConnected) return;
            Debug.WriteLine("NetworkClient: Attempting to connect...");
            if(host != null)
                _client = new TcpGameClient(host, port, _incoming);
            if (!_client.ConnectAsync())
            {
                ConnectFailed?.Invoke(_client.ErrorMessage);
            }
            else
                Send(new
                {
                    type = "hello",
                    value = PROTOCOL_VERSION
                });
        }
        public void Disconnect()
        {
            if(!IsConnected) return;
            _client.Dispose();
            Channel = null;
        }

        // ---- Channels / Lobbys ----

        public void CreateChannel(string name, string nick, bool hidden = false)
        {
            Send(new
            {
                Session,
                type = "create_channel",
                channel = name,
                nick,
                hidden
            });
        }

        public void JoinChannel(string name, string nick, bool hidden = false)
        {
            Send(new { Session, type = "join_channel", channel = name, nick, hidden });
        }

        public void LeaveChannel()
        {
            Send(new { Session, type = "leave_channel" });
        }

        public void RequestChannelList()
        {
            Send(new { Session, type = "list_channels" });
        }

        // ---- Messaging ----

        public void SendMessage(string text, bool echo = false)
        {
            Send(new { Session, type = "chat", text, echo });
        }

        public void SendServerSecret(string secret)
        {
            Send(new { Session, type = "server_secret", text = secret });
        }

        // ---- Internal Send ----

        private void Send(object payload)
        {
            if (_client == null || !_client.IsConnected)
                return;
            var json = JsonSerializer.Serialize(payload);
            var data = Encoding.UTF8.GetBytes(json);

            var packet = new byte[data.Length + 4];
            BitConverter.GetBytes(data.Length).CopyTo(packet, 0);
            data.CopyTo(packet, 4);

            _client.SendAsync(packet);
        }

        // ---- Call from MonoGame Update() ----

        public void Update()
        {
            while (_incoming.TryDequeue(out var msg))
            {
                switch (msg.Type)
                {
                    case "auth":
                        Self = msg.Client;
                        break;

                    case "challenge":
                        Debug.WriteLine("NetworkClient: Received challenge, sending auth...");
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            string hash = GetHash(sha256Hash, msg.Text+Secret);
                            Send(new { type = "auth", text = hash });
                            Debug.WriteLine("NetworkClient: Sent auth.");
                        }
                        break;

                    case "error":
                        Error?.Invoke(msg.Error);
                        break;

                    case "connected":
                        Connected?.Invoke();
                        break;

                    case "disconnected":
                        Disconnected?.Invoke();
                        break;

                    case "channel_joined":
                        Channel = msg.Channel;
                        JoinedChannel?.Invoke(msg.Channel, msg.Error);
                        break;

                    case "channel_left":
                        Channel = null;
                        LeftChannel?.Invoke(msg.ChannelName);
                        break;

                    case "channel_list":
                        ChannelListReceived?.Invoke(msg.Channels);
                        break;

                    case "channel_user_joined":
                        ChannelUserJoined?.Invoke(msg.Client.Nick);
                        Channel.Clients.Add(msg.Client);
                        break;

                    case "channel_user_left":
                        ChannelUserLeft?.Invoke(msg.Client.Nick);
                        Channel.Clients.RemoveAll(c => c.Id == msg.Client.Id);
                        break;

                    case "chat":
                        ChatMessageReceived?.Invoke(msg.Client.Nick, msg.Text);
                        break;

                    case "change_owner":
                        Channel.Owner = msg.Client.Id;
                        break;

                    case "server_secret":
                        ServerSecretAccepted?.Invoke();
                        break;
                }
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
