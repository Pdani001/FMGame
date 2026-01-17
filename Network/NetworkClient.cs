using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ReFMGame.Network
{
    public class NetworkClient(string host, int port)
    {
        private TcpGameClient _client;
        private readonly ConcurrentQueue<Message> _incoming = new();

        public Channel Channel { get; private set; } = null;

        private const string HashSalt = "[9edp!J3qWd4)XWtW#sa@s@>PJaXEW]Ns0FzYi5{WEA4pfCjgbeEU3+exR)+ww2(";

        public Client Self { get; private set; } = null;
        private string Session => Self?.Id.ToString() ?? "";

        public const int PROTOCOL_VERSION = 3;

        public bool IsConnected => _client?.IsConnected ?? false;

        private readonly string _host = host;
        private readonly int _port = port;

        long lastServerTick;

        public event Action ServerSecretAccepted;
        public event Action Connected;
        public event Action<string> ConnectFailed;
        public event Action<string> Error;
        public event Action<string> Disconnected;
        public event Action<Channel[]> ChannelListReceived;
        public event Action<Channel, string> JoinedChannel;
        public event Action<string> LeftChannel;
        public event Action<Client, List<Selected>> ChannelUserJoined;
        public event Action<Client> ChannelUserLeft;
        public event Action<Client, string> ChatMessageReceived;

        public event Action<Client, Character> CharacterSelected;
        public event Action<Character, bool> UserReady;
        public event Action<int> GameCountdown;
        public event Action<CharacterPosition[]> GameStart;
        public event Action GameAbort;
        public event Action<GameState> GameState;
        public event Action<int> GameMusicbox;
        public event Action<Character, short> RobotMove;
        public event Action<int> MoveTimer;
        public event Action<Character> JumpscareStart;
        public event Action JumpscareEnd;
        public event Action FoxyRun;

        public event Action<Message> GenericMessageReceived;

        // ---- Connection ----

        public void Connect(string host = null, int port = 7121)
        {
            if (IsConnected) return;
            Debug.WriteLine("NetworkClient: Attempting to connect...");
            if(host == null)
            {
                host = _host;
                port = _port;
            }
            if (IPAddress.TryParse(host, out var address))
            {
                _client = new TcpGameClient(address.ToString(), port, _incoming);
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(host))
                        throw new UriFormatException("Invalid host/address");
                    var entry = Dns.GetHostEntry(host);
                    if (entry.AddressList.Length == 0)
                        throw new IndexOutOfRangeException("AddressList is empty");
                    Debug.WriteLine($"Resolved host '{host}' to '{string.Join(',', entry.AddressList.ToList())}'");
                    host = entry.AddressList[0].ToString();
                    _client = new TcpGameClient(host, port, _incoming);
                }
                catch
                {
                    ConnectFailed?.Invoke("Failed to resolve host");
                    return;
                }
            }
            Debug.WriteLine($"Connecting to IP '{host}' on port {port}");
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

        // ---- Channels ----

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

        public void JoinChannel(string name, string nick, bool hidden = false, string password = "")
        {
            Send(new { Session, type = "join_channel", channel = name, nick, hidden, text = password });
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

        public void SendMessage(string text)
        {
            Send(new { Session, type = "chat", text });
        }

        public void SendServerSecret(string secret)
        {
            Send(new { Session, type = "server_secret", text = secret });
        }

        // ---- Lobby ----

        public void SelectCharacter(int character)
        {
            Send(new { Session, type = "select", value = character });
        }

        public void SetReady(bool ready)
        {
            Send(new { Session, type = "ready", value = ready ? 1 : 0 });
        }

        // ---- In-Game ----

        public void SetCameraActive(bool state)
        {
            Send(new { Session, type = "camera", value = state ? 1 : 0, tick = lastServerTick });
        }

        public void SetLight(bool left, bool state)
        {
            Send(new { Session, type = "light", LeftSide = left, value = state ? 1 : 0, tick = lastServerTick });
        }

        public void SetDoor(bool left, bool state)
        {
            Send(new { Session, type = "door", LeftSide = left, value = state ? 1 : 0, tick = lastServerTick });
        }

        public void ChangeCameraView(short target)
        {
            Send(new { Session, type = "move", value = target, tick = lastServerTick });
        }

        public void StartAttack()
        {
            Send(new { Session, type = "attack", tick = lastServerTick });
        }

        public void RunCheat(string type)
        {
            Send(new { Session, type = "cheat#" + type.ToLower(), tick = lastServerTick });
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
                    case "hello":
                        Send(new
                        {
                            type = "hello",
                            value = PROTOCOL_VERSION
                        });
                        break;

                    case "auth":
                        Self = msg.Client;
                        break;

                    case "challenge":
                        Debug.WriteLine("NetworkClient: Received challenge, sending auth...");
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            string hash = GetHash(sha256Hash, msg.Text+HashSalt);
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
                        if(_client.WasActive)
                            Disconnected?.Invoke(msg.Error);
                        else
                            ConnectFailed?.Invoke(msg.Error);
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
                        if(!Channel.Clients.Exists(c=>c.Id == msg.Client.Id))
                            Channel.Clients.Add(msg.Client);
                        ChannelUserJoined?.Invoke(msg.Client, msg.Selected);
                        break;

                    case "channel_user_left":
                        Channel.Clients.RemoveAll(c => c.Id == msg.Client.Id);
                        ChannelUserLeft?.Invoke(msg.Client);
                        break;

                    case "chat":
                        msg.Client.IsAdmin = msg.IsAdmin;
                        if (msg.Client.Id == Self.Id)
                            Self.IsAdmin = msg.IsAdmin;
                        ChatMessageReceived?.Invoke(msg.Client, msg.Text);
                        break;

                    case "change_owner":
                        Channel.Owner = msg.Client.Id;
                        break;

                    case "server_secret":
                        Self.IsAdmin = true;
                        ServerSecretAccepted?.Invoke();
                        break;

                    case "select":
                        CharacterSelected?.Invoke(msg.Client, (Character)msg.Value);
                        break;

                    case "ready":
                        UserReady?.Invoke((Character)msg.Value, msg.Ready);
                        break;

                    case "game_countdown":
                        GameCountdown?.Invoke(msg.Value ?? 0);
                        break;

                    case "game_start":
                        GameStart?.Invoke(msg.Positions ?? []);
                        break;

                    case "game_abort":
                        GameAbort?.Invoke();
                        break;

                    case "state":
                        lastServerTick = msg.Tick ?? 0;
                        GameState?.Invoke(msg.State);
                        break;

                    case "musicbox":
                        GameMusicbox?.Invoke(msg.Value ?? 0);
                        break;

                    case "move":
                        RobotMove?.Invoke(msg.Character ?? Character.Guard, (short)(msg.Value ?? -1));
                        break;

                    case "move_timer":
                        MoveTimer?.Invoke(msg.Value ?? 99);
                        break;

                    case "jumpscare":
                        JumpscareStart?.Invoke(msg.Character ?? Character.None);
                        break;

                    case "end_jumpscare":
                        JumpscareEnd?.Invoke();
                        break;

                    case "foxy_run":
                        FoxyRun?.Invoke();
                        break;

                    default:
                        GenericMessageReceived?.Invoke(msg);
                        break;
                }
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
