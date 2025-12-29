using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace ReFMGame.Network
{
    public class NetworkClient
    {
        private readonly TcpGameClient _client;
        private readonly ConcurrentQueue<Message> _incoming = new();

        public Channel Channel { get; private set; }

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
            _client.DisconnectAsync();
            Channel = null;
        }

        // ---- Identity ----

        public void SetNickname(string nick)
        {
            Send(new { type = "set_nick", nick });
        }

        // ---- Channels ----

        public void CreateChannel(string name, bool hidden = false, bool autoClose = false)
        {
            Send(new
            {
                type = "create_channel",
                channel = name,
                hidden,
                autoClose
            });
        }

        public void JoinChannel(string name, bool hidden = false, bool autoClose = false)
        {
            Send(new { type = "join_channel", channel = name, hidden, autoClose });
        }

        public void LeaveChannel()
        {
            Send(new { type = "leave_channel" });
        }

        public void RequestChannelList()
        {
            Send(new { type = "list_channels" });
        }

        // ---- Messaging ----

        public void SendChannelText(byte subchannel, string text, bool echo = false)
        {
            Send(new { subchannel, type = "channel_text", text, echo });
        }

        public void SendChannelNumber(byte subchannel, int value, bool echo = false)
        {
            Send(new { subchannel, type = "channel_number", value, echo });
        }

        public void SendPrivateText(byte subchannel, string to, string text)
        {
            Send(new { subchannel, type = "private_text", to, text });
        }

        public void SendPrivateNumber(byte subchannel, string to, int value)
        {
            Send(new { subchannel, type = "private_number", to, value });
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
                switch (msg.type)
                {
                    case "error":
                        Error?.Invoke(msg.error);
                        break;

                    case "connected":
                        Connected?.Invoke();
                        break;

                    case "channel_joined":
                        Channel = msg.channel;
                        JoinedChannel?.Invoke(msg.channel, msg.error);
                        break;

                    case "channel_left":
                        Channel = null;
                        LeftChannel?.Invoke(msg.channelname);
                        break;

                    case "channel_list":
                        ChannelListReceived?.Invoke(msg.channels);
                        break;

                    case "channel_user_joined":
                        ChannelUserJoined?.Invoke(msg.client);
                        break;

                    case "channel_user_left":
                        ChannelUserLeft?.Invoke(msg.client);
                        break;

                    case "channel_text":
                        ChannelTextReceived?.Invoke(msg.subchannel, msg.client, msg.text);
                        break;

                    case "channel_number":
                        ChannelNumberReceived?.Invoke(msg.subchannel, msg.client, msg.value ?? 0);
                        break;

                    case "private_text":
                        PrivateTextReceived?.Invoke(msg.subchannel, msg.client, msg.text);
                        break;

                    case "private_number":
                        PrivateNumberReceived?.Invoke(msg.subchannel, msg.client, msg.value ?? 0);
                        break;

                    case "set_nick":
                        NicknameUpdate?.Invoke(msg.success ?? false, msg.error);
                        break;
                }
            }
        }
    }
}
