using Gum.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using ReFMGame.GameHelper;
using ReFMGame.Network;
using ReFMGame.Scenes;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ReFMGame;

public class FMGame : GameExtended
{
    private readonly KeyboardListener _keyboardListener;
    private readonly ScreenManager _screenManager;
    public Rectangle RenderTargetDestination { get; private set; }
    private BitmapFont _font;
    private SmartFramerate smartFPS;

    /**
    * <summary>Get the current FPS</summary>
    */
    public int FPS {
        get
        {
            if (smartFPS != null)
                return (int)Math.Round(smartFPS.Framerate, MidpointRounding.AwayFromZero);
            return 0;
        }
    }

    /**
    * <summary>The target number of frames per seconds to achieve</summary>
    */
    private const int TargetFPS = 60;

    public FMGame() : base(new(1280,720))
    {
        string key = "";
        foreach (string value in Environment.GetCommandLineArgs()[1..])
        {
            if(value.StartsWith('-'))
            {
                if(key != "")
                {
                    LaunchParameters.Add(key, "");
                }
                key = value[1..];
                continue;
            }
            if(key != "")
            {
                LaunchParameters.Add(key, value);
                key = "";
            }
        }
        _keyboardListener = new KeyboardListener();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / TargetFPS));
        MouseState = new(true);
        _screenManager = new ScreenManager();
        Components.Add(_screenManager);
#if DEBUG
        DebugMode = true;
#endif
        settings = MethodHelper.EnsureJson("settings.json", SettingsContext.Default.Settings);
        WarningShown = settings.WarningDismissed;
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        Version = version[..(version.Length - 2)];
        Debug.WriteLine($"Running version {Version}+{NetworkClient.PROTOCOL_VERSION}");
    }

    public bool IsFullscreen
    {
        get {
            if(_graphics == null)
                return false;
            return _graphics.IsFullScreen;
        }
    }

    private void ToggleFullScreen()
    {
        if (_graphics.IsFullScreen)
        {
            _graphics.PreferredBackBufferWidth = WindowSize.X;
            _graphics.PreferredBackBufferHeight = WindowSize.Y;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        }
        GumUI.Cursor.TransformMatrix = Matrix.CreateScale((float)WindowSize.X / CurrentWindowSize.X, (float)WindowSize.Y / CurrentWindowSize.Y, 1f);
        _graphics.ToggleFullScreen();
        Vector2 oldPos = MouseState.Position;
        GumUI.CanvasWidth = _graphics.PreferredBackBufferWidth;
        GumUI.CanvasHeight = _graphics.PreferredBackBufferHeight;
        RenderTargetDestination = GetRenderTargetDestination(WindowSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        MouseState.SetRenderTargetDestination(RenderTargetDestination);
        MouseState.SetScreenScale(GetRenderTargetScale(WindowSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
        MouseState.SetWindowSize(WindowSize);
        Point newPos = MouseState.ScalePositionUp(oldPos);
        Mouse.SetPosition(newPos.X, newPos.Y);
    }

    protected override void Initialize()
    {
        smartFPS = new SmartFramerate(4);
        Window.Title = "Fazbear Multiplayer";
        Window.AllowUserResizing = false;
        _graphics.IsFullScreen = false;
        _graphics.SynchronizeWithVerticalRetrace = false;
        _graphics.HardwareModeSwitch = false;
        _graphics.PreferredBackBufferWidth = WindowSize.X;
        _graphics.PreferredBackBufferHeight = WindowSize.Y;
        _graphics.ApplyChanges();
        UpdateKeyBind = false;
        if (Client != null && Client.IsConnected)
        {
            Client.Disconnect();
        }
        UpdateClient();
        if (!GumUI.IsInitialized)
        {
            GumUI.Initialize(this, DefaultVisualsVersion.V3);
            GumUI.UseKeyboardDefaults();
            Audio = new AudioController();
            _keyboardListener.KeyPressed += KeyPressed;
        }
        else
        {
            GumUI.CanvasWidth = _graphics.PreferredBackBufferWidth;
            GumUI.CanvasHeight = _graphics.PreferredBackBufferHeight;
            GumUI.Cursor.TransformMatrix = Matrix.Identity;
            GumUI.Root.Children.Clear();
        }
        base.Initialize();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            WindowSubclass.Subclass(Window.Handle);
        }
    }

    private void KeyPressed(object sender, KeyboardEventArgs e)
    {
        if (!UpdateKeyBind)
        {
            var bind = settings.KeyBinds.Where(kvp => kvp.Value.IsValid(e.Character)).Select(kvp=>kvp.Key);
            if(bind.Any())
                switch (bind.First())
                {
                    case BindKey.Fullscreen:
                        ToggleFullScreen();
                        break;
                    case BindKey.Debug:
                        DebugMode = !DebugMode;
                        break;
                    case BindKey.Screenshot:
                        var path = MethodHelper.GetPath($"screenshots{Path.DirectorySeparatorChar}{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_ffff")}.png");
                        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                        var stream = new StreamWriter(path).BaseStream;
                        GetScreenshot(true).SaveAsPng(stream, CurrentWindowSize.X, CurrentWindowSize.Y);
                        stream.Close();
                        stream.Dispose();
                        break;
                    default:
                        return;
                }
        }
    }

    private bool IsRestart = false;

    protected override void LoadContent()
    {
        if (IsRestart)
        {
            _screenManager.ClearScreens();
            _screenManager.ShowScreen(new Splash(this));
            return;
        }
        Audio.Volume = settings.Volume;
        IsRestart = true;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        RenderTarget = new RenderTarget2D(GraphicsDevice, WindowSize.X, WindowSize.Y);
        RenderTargetDestination = GetRenderTargetDestination(WindowSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        MouseState.SetRenderTargetDestination(RenderTargetDestination);
        MouseState.SetScreenScale(GetRenderTargetScale(WindowSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
        _font = Content.Load<BitmapFont>("font/debug");
        _screenManager.ClearScreens();
        _screenManager.ShowScreen(new Splash(this));
    }

    protected override void UnloadContent()
    {
        // Dispose of the audio controller.
        Audio.Dispose();
        MethodHelper.SaveJson("settings.json", settings, SettingsContext.Default.Settings);
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        MouseState.SetMouseState(Mouse.GetState());
        MouseExtended.Update();
        KeyboardExtended.Update();
        Audio.Update();
        Client.Update();

        _keyboardListener.Update(gameTime);

        GumUI.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        Color BaseColor = Color.Black;
        GraphicsDevice.SetRenderTarget(null);
        SpriteBatch.Begin();
        SpriteBatch.Draw(RenderTarget, RenderTargetDestination, Color.White);
        smartFPS.Update(gameTime.ElapsedGameTime.TotalSeconds);

        if (DebugMode)
        {
            Color textColor = new((byte)~BaseColor.R, (byte)~BaseColor.G, (byte)~BaseColor.B);
            SpriteBatch.DrawString(_font, $"FPS: {FPS}", Vector2.One, textColor);
            Vector2 mouse = new(Mouse.GetState().X + 12, Mouse.GetState().Y);
            SpriteBatch.DrawString(_font, $"{MouseState.Position.X};{MouseState.Position.Y}", mouse, textColor);
        }

        SpriteBatch.End();
    }

    private static Rectangle GetRenderTargetDestination(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
    {
        Point bounds = new(preferredBackBufferWidth, preferredBackBufferHeight);
        float scale = GetRenderTargetScale(resolution, preferredBackBufferWidth, preferredBackBufferHeight);
        Rectangle rectangle = new();

        if (scale == 1.0f)
        {
            // Resolution and window/screen share aspect ratio
            rectangle.Size = bounds;
            return rectangle;
        }
        rectangle.Width = (int)(resolution.X * scale);
        rectangle.Height = (int)(resolution.Y * scale);
        return CenterRectangle(new Rectangle(Point.Zero, bounds), rectangle);
    }

    private static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
    {
        Point delta = outerRectangle.Center - innerRectangle.Center;
        innerRectangle.Offset(delta);
        return innerRectangle;
    }

    private static float GetRenderTargetScale(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
    {
        float resolutionRatio = (float)resolution.X / resolution.Y;
        float screenRatio;
        Point bounds = new(preferredBackBufferWidth, preferredBackBufferHeight);
        screenRatio = (float)bounds.X / bounds.Y;
        float scale = 1.0f;

        if (resolutionRatio < screenRatio)
            scale = (float)bounds.Y / resolution.Y;
        else if (resolutionRatio > screenRatio)
            scale = (float)bounds.X / resolution.X;
        return scale;
    }
}
