using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace ReFMGame.Network
{
    public class NetworkClient
    {
        private readonly WsGameClient _client;
        private readonly ConcurrentQueue<Message> _incoming = new();

        public bool IsConnected => _client.IsConnected;

        // Events for game layer
        public event Action Connected;
        public event Action<string> ConnectFailed;
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
            _client = new WsGameClient(host, port, _incoming);
        }

        // ---- Connection ----

        public void Connect()
        {
            Debug.WriteLine("NetworkClient: Attempting to connect...");
            if (IsConnected) return;
            if (!_client.ConnectAsync())
            {
                ConnectFailed?.Invoke(_client.ErrorMessage);
            }
        }
        public void Disconnect()
        {
            if(!IsConnected) return;
            _client.DisconnectAsync();
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
            _client.SendTextAsync(json);
        }

        // ---- Call from MonoGame Update() ----

        public void Update()
        {
            while (_incoming.TryDequeue(out var msg))
            {
                switch (msg.Type)
                {
                    case "connected":
                        Connected?.Invoke();
                        break;

                    case "channel_joined":
                        JoinedChannel?.Invoke(msg.Channel, msg.Error);
                        break;

                    case "channel_left":
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
                }
            }
        }
    }
}
