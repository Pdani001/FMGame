using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ReFMGame.Network
{
    public class NetworkClient
    {
        private readonly TcpGameClient _client;
        private readonly ConcurrentQueue<Message> _incoming = new();

        public Channel Channel { get; private set; }

        private readonly string Secret = "[9edp!J3qWd4)XWtW#sa@s@>PJaXEW]Ns0FzYi5{WEA4pfCjgbeEU3+exR)+ww2(";
        private string Session { get; set; }

        public bool IsConnected => _client.IsConnected;

        // Events for game layer
        public event Action Connected;
        public event Action<string> ConnectFailed;
        public event Action<string> Error;
        public event Action Disconnected;
        public event Action<Channel[]> ChannelListReceived;
        public event Action<Channel, string> JoinedChannel;
        public event Action<string> LeftChannel;
        public event Action<string> ChannelUserJoined;
        public event Action<string> ChannelUserLeft;
        public event Action<byte, string, string> ChannelTextReceived;
        public event Action<byte, string, int> ChannelNumberReceived;
        public event Action<byte, string, string> PrivateTextReceived;
        public event Action<byte, string, int> PrivateNumberReceived;
        public event Action<bool, string> NicknameUpdate;

        public NetworkClient(string host, int port)
        {
            _client = new TcpGameClient(host, port, _incoming);
        }

        // ---- Connection ----

        public void Connect()
        {
            if (IsConnected) return;
            Debug.WriteLine("NetworkClient: Attempting to connect...");
            if (!_client.ConnectAsync())
            {
                ConnectFailed?.Invoke(_client.ErrorMessage);
            }
        }
        public void Disconnect()
        {
            if(!IsConnected) return;
            _client.Dispose();
            Channel = null;
        }

        // ---- Identity ----

        public void SetNickname(string nick)
        {
            Send(new { Session, type = "set_nick", nick });
        }

        // ---- Channels ----

        public void CreateChannel(string name, bool hidden = false, bool autoClose = false)
        {
            Send(new
            {
                Session,
                type = "create_channel",
                channel = name,
                hidden,
                autoClose
            });
        }

        public void JoinChannel(string name, bool hidden = false, bool autoClose = false)
        {
            Send(new { Session, type = "join_channel", channel = name, hidden, autoClose });
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

        public void SendChannelText(byte subchannel, string text, bool echo = false)
        {
            Send(new { Session, subchannel, type = "channel_text", text, echo });
        }

        public void SendChannelNumber(byte subchannel, int value, bool echo = false)
        {
            Send(new { Session, subchannel, type = "channel_number", value, echo });
        }

        public void SendPrivateText(byte subchannel, string to, string text)
        {
            Send(new { Session, subchannel, type = "private_text", to, text });
        }

        public void SendPrivateNumber(byte subchannel, string to, int value)
        {
            Send(new { Session, subchannel, type = "private_number", to, value });
        }

        public void SendServerSecret(string secret)
        {
            Send(new { Session, type = "server_secret", text = secret });
        }

        // ---- Internal Send ----

        private void Send(object payload)
        {
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
                        Session = msg.Text;
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
                        ChannelUserJoined?.Invoke(msg.Client);
                        break;

                    case "channel_user_left":
                        ChannelUserLeft?.Invoke(msg.Client);
                        break;

                    case "channel_text":
                        ChannelTextReceived?.Invoke(msg.SubChannel, msg.Client, msg.Text);
                        break;

                    case "channel_number":
                        ChannelNumberReceived?.Invoke(msg.SubChannel, msg.Client, msg.Value ?? 0);
                        break;

                    case "private_text":
                        PrivateTextReceived?.Invoke(msg.SubChannel, msg.Client, msg.Text);
                        break;

                    case "private_number":
                        PrivateNumberReceived?.Invoke(msg.SubChannel, msg.Client, msg.Value ?? 0);
                        break;

                    case "set_nick":
                        NicknameUpdate?.Invoke(msg.Success ?? false, msg.Error);
                        break;

                    case "server_secret":
                        PrivateNumberReceived?.Invoke(0, "FMServer", msg.Success ?? false ? 1 : 0);
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
