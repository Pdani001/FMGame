using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using ReFMGame.Network;
using System.Diagnostics;
using System.Net;

namespace ReFMGame.GameHelper
{
    public abstract class GameExtended : Game
    {
        protected readonly GraphicsDeviceManager _graphics;
        public readonly Point WindowSize;
        public string Version { get; protected set; }

        private GameExtended() { }
        public GameExtended(Point windowSize)
        {
            _graphics = new GraphicsDeviceManager(this);
            WindowSize = windowSize;
        }

        public RenderTarget2D RenderTarget { get; protected set; }
        public SpriteBatch SpriteBatch { get; protected set; }
        public AudioController Audio { get; protected set; }
        public MouseStateWrapper MouseState { get; protected set; }
        public bool UpdateKeyBind { get; set; } = false;
        public GumService GumUI => GumService.Default;
        public NetworkClient Client { get; protected set; }
        public bool DebugMode { get; protected set; } = false;
        public Settings settings { get; protected set; } = new Settings();
        public bool WarningShown { get; set; } = false;

        public Point CurrentWindowSize
        {
            get
            {
                if (_graphics == null)
                    return new(0, 0);
                return new(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            }
        }

        public int ServerIndex
        {
            get => settings.ServerIndex; set
            {
                settings.ServerIndex = value;
                UpdateClient();
            }
        }
        public string CustomAddress
        {
            get => settings.CustomAddress; set
            {
                settings.CustomAddress = value;
                if (ServerIndex == 2)
                    UpdateClient();
            }
        }
        public string ServerSecret { get; private set; }

        protected void UpdateClient()
        {
            if (Client != null && Client.IsConnected)
            {
                return;
            }
            ServerSecret = "";
            switch (ServerIndex)
            {
                case 0:
                    Client = new NetworkClient("pghost.org", 7121);
                    CustomAddress = "";
                    break;
                case 1:
                    Client = new NetworkClient("pghost.org", 7122);
                    CustomAddress = "";
                    break;
                default:
                    ServerSecret = LaunchParameters.TryGetValue("-secret", out string value) ? value : "";
                    var host = "";
                    var port = 7121;
                    if (IPEndPoint.TryParse(CustomAddress, out IPEndPoint ip))
                    {
                        host = ip.Address.ToString();
                        port = ip.Port;
                    }
                    else
                    {
                        var split = CustomAddress.Split(':');
                        host = split[0];
                        if (split.Length > 1 && !string.IsNullOrEmpty(split[1].Trim()))
                            int.TryParse(split[1], out port);
                    }
                    Debug.WriteLine($"host = '{host}'; port = {port}");
                    Client = new NetworkClient(host, port);
                    break;
            }
        }
        public Texture2D GetScreenshot(bool screen = false)
        {
            Texture2D screenshot;
            Color[] colors;
            if (screen)
            {
                screenshot = new Texture2D(GraphicsDevice, CurrentWindowSize.X, CurrentWindowSize.Y);
                colors = new Color[CurrentWindowSize.X * CurrentWindowSize.Y];
                GraphicsDevice.GetBackBufferData(colors);
            }
            else
            {
                screenshot = new Texture2D(GraphicsDevice, WindowSize.X, WindowSize.Y);
                colors = new Color[WindowSize.X * WindowSize.Y];
                RenderTarget.GetData(colors);
            }
            screenshot.SetData(colors);
            return screenshot;
        }
    }
}
