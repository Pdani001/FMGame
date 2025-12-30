using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGameGum;
using ReFMGame.GameHelper;
using ReFMGame.Network;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ReFMGame;

public class FMGame : Game
{
    public readonly Point WindowSize = new(1280, 720);
    private readonly GraphicsDeviceManager _graphics;
    private readonly ScreenManager _screenManager;
    public SpriteBatch SpriteBatch;
    public RenderTarget2D RenderTarget { get; private set; }
    public Rectangle RenderTargetDestination { get; private set; }
    private SpriteFont _font;
    private SmartFramerate smartFPS;
    static GumService GumUI => GumService.Default;
    public MouseStateWrapper MouseState { get; private set; }
    public AudioController Audio { get; private set; }

    public static bool Active { get; private set; }

    public NetworkClient Client { get; private set; }

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

    public static string ContentRoot { get; private set; }
    public static IServiceProvider ContentProvider { get; private set; }

    public KeyBind FullScreenBind = new(key: Keys.F11);
    public KeyBind DebugBind = new(key: Keys.F1);
    public bool DebugMode { get; private set; } = false;

    public FMGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / TargetFPS));
        ContentRoot = Content.RootDirectory;
        ContentProvider = Content.ServiceProvider;
        MouseState = new(true);
        _screenManager = new ScreenManager();
        Components.Add(_screenManager);
#if DEBUG
        DebugMode = true;
#endif
    }

    public bool IsFullscreen
    {
        get {
            if(_graphics == null)
                return false;
            return _graphics.IsFullScreen;
        }
    }

    public Point CurrentWindowSize
    {
        get
        {
            if (_graphics == null)
                return new(0,0);
            return new(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }
    }

    private void ToggleFullScreen()
    {
        Debug.WriteLine(_graphics.IsFullScreen ? "switching to windowed" : "switching to fullscreen");
        if (_graphics.IsFullScreen)
        {
            _graphics.PreferredBackBufferWidth = WindowSize.X;
            _graphics.PreferredBackBufferHeight = WindowSize.Y;
            GumUI.Cursor.TransformMatrix = Matrix.Identity;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            GumUI.Cursor.TransformMatrix = Matrix.CreateScale((float)WindowSize.X / CurrentWindowSize.X, (float)WindowSize.Y / CurrentWindowSize.Y, 1f);
        }
        _graphics.ToggleFullScreen();
        Debug.WriteLine($"W{_graphics.PreferredBackBufferWidth} H{_graphics.PreferredBackBufferHeight}");
        Vector2 oldPos = MouseState.Position;
        GumUI.CanvasWidth = _graphics.PreferredBackBufferWidth;
        GumUI.CanvasHeight = _graphics.PreferredBackBufferHeight;
        RenderTargetDestination = GetRenderTargetDestination(WindowSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        MouseState.SetRenderTargetDestination(RenderTargetDestination);
        MouseState.SetScreenScale(GetRenderTargetScale(WindowSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
        MouseState.SetGindowSize(WindowSize);
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
        if (Client != null && Client.IsConnected)
        {
            Client.Disconnect();
        }
        Client = new NetworkClient("142.132.195.36", 6121);
        if (!GumUI.IsInitialized)
        {
            GumUI.Initialize(this, DefaultVisualsVersion.V3);
            Audio = new AudioController();
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

    private bool IsRestart = false;

    protected override void LoadContent()
    {
        if (IsRestart)
        {
            _screenManager.ClearScreens();
            _screenManager.ShowScreen(new Scenes.Splash(this));
            return;
        }
        IsRestart = true;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        RenderTarget = new RenderTarget2D(GraphicsDevice, WindowSize.X, WindowSize.Y);
        RenderTargetDestination = GetRenderTargetDestination(WindowSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        MouseState.SetRenderTargetDestination(RenderTargetDestination);
        MouseState.SetScreenScale(GetRenderTargetScale(WindowSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
        _font = Content.Load<SpriteFont>("font/Consolas");
        _screenManager.ClearScreens();
        _screenManager.ShowScreen(new Scenes.Splash(this));
    }

    protected override void UnloadContent()
    {
        // Dispose of the audio controller.
        Audio.Dispose();

        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        Active = IsActive;
        MouseState.SetMouseState(Mouse.GetState());
        MouseExtended.Update();
        KeyboardExtended.Update();
        var keyboard = KeyboardExtended.GetState();
        Audio.Update();
        Client.Update();

        if (DebugMode) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (keyboard.WasKeyPressed(Keys.F2)) {
                Debug.WriteLine("! RESTARTING GAME !");
                Initialize();
            }
        }

        if (FullScreenBind.Key != Keys.None)
        {
            if (FullScreenBind.IsValid())
            {
                ToggleFullScreen();
            }
        }
        if (DebugBind.Key != Keys.None)
        {
            if (DebugBind.IsValid())
            {
                DebugMode = !DebugMode;
                Debug.WriteLine($"Debug mode set to '{DebugMode}'");
            }
        }

        GumUI.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        Color BaseColor = Color.Black;
        GumUI.Draw();
        GraphicsDevice.SetRenderTarget(null);
        SpriteBatch.Begin();
        SpriteBatch.Draw(RenderTarget, RenderTargetDestination, Color.White);
        smartFPS.Update(gameTime.ElapsedGameTime.TotalSeconds);

        if (DebugMode)
        {
            Color textColor = new((byte)~BaseColor.R, (byte)~BaseColor.G, (byte)~BaseColor.B);
            SpriteBatch.DrawString(_font, $"FPS: {FPS}", Vector2.One, textColor);
            Vector2 mouse = new(Mouse.GetState().X + 10, Mouse.GetState().Y);
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

    public Texture2D GetScreenshot()
    {
        Texture2D screenshot = new Texture2D(GraphicsDevice, WindowSize.X, WindowSize.Y);
        Color[] colors = new Color[WindowSize.X * WindowSize.Y];
        RenderTarget.GetData(colors);
        screenshot.SetData(colors);
        return screenshot;
    }
}
