using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGameGum.Input;
using ReFMGame.Animations;
using ReFMGame.GameHelper;
using System;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace ReFMGame.Scenes;
public class Menu(FMGame game, bool lobby = false) : GameScreen(game)
{
    public Texture2D bg_texture { get; private set; }
    public Texture2D logo { get; private set; }
    private SoundEffectInstance music = null;
    public TextureAnimation bg_animation { get; private set; }
    public TextureAnimation static_animation { get; private set; }
    public BitmapFont bmfont { get; private set; }
    public BitmapFont verfont { get; private set; }
    public string vertext { get; private set; }
    public Vector2 verpos { get; private set; }
    private Rectangle start = new(128, 352, 138, 32);
    private Rectangle settings = new(128, 400, 196, 32);
    private Rectangle credits = new(128, 448, 172, 32);
    private string startPadding = "    ";
    private Vector2 startPadSize = Vector2.Zero;
    private string settingsPadding = "    ";
    private Vector2 settingsPadSize = Vector2.Zero;
    private string creditsPadding = "    ";
    private Vector2 creditsPadSize = Vector2.Zero;
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        Color bgcolor = Color.White;
        bgcolor.A = (byte)(255 - BGOpacity);
        game.SpriteBatch.Draw(bg_texture, Vector2.Zero, null, bgcolor, 0, Vector2.Zero , 1, SpriteEffects.None, 0);
        
        game.SpriteBatch.DrawString(bmfont, startPadding+"Start", start.Location.ToVector2() - startPadSize, Color.White);
        game.SpriteBatch.DrawString(bmfont, settingsPadding+"Settings", settings.Location.ToVector2() - settingsPadSize, Color.White);
        game.SpriteBatch.DrawString(bmfont, creditsPadding+"Credits", credits.Location.ToVector2() - creditsPadSize, Color.White);
        game.SpriteBatch.Draw(logo, new(68, 50), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
        if(RareBGM)
            game.SpriteBatch.DrawString(bmfont, "57", new(79, 190), Color.Yellow);
        game.SpriteBatch.DrawString(verfont, vertext, verpos, Color.White);
        game.SpriteBatch.End();

        Color staticcolor = Color.White;
        staticcolor.A = (byte)(255 - StaticOpacity);
        game.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
        game.SpriteBatch.Draw(static_animation[static_animation.Index], Vector2.Zero, null, staticcolor, 0, Vector2.Zero , 1, SpriteEffects.None, .4f);
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        if (lobby)
        {
            lobby = false;
            ScreenManager.ShowScreen(new Lobby(game, this));
            return;
        }
        Mouse.SetCursor(MouseCursor.Arrow);
        startPadding = "    ";
        static_animation.Animate(gameTime);
        if (start.Contains(game.MouseState.Position))
        {
            if (MouseExtended.GetState().WasButtonPressed(MouseButton.Left) && game.IsActive)
            {
                ScreenManager.ShowScreen(new Lobby(game, this));
            }
            else
            {
                startPadding = " >> ";
            }
        }
        if (game.DebugMode && KeyboardExtended.GetState().WasKeyPressed(Keys.S) && game.IsActive)
        {
            ScreenManager.ReplaceScreen(new Loading(game));
        }
        startPadSize = bmfont.MeasureString(startPadding);
        startPadSize.Y = 0;

        settingsPadding = "    ";
        if (settings.Contains(game.MouseState.Position))
        {
            if (MouseExtended.GetState().WasButtonPressed(MouseButton.Left) && game.IsActive)
            {
                ScreenManager.ShowScreen(new Settings(game, this));
            }
            else
            {
                settingsPadding = " >> ";
            }
        }
        settingsPadSize = bmfont.MeasureString(settingsPadding);
        settingsPadSize.Y = 0;

        creditsPadding = "    ";
        if (credits.Contains(game.MouseState.Position))
        {
            if (MouseExtended.GetState().WasButtonPressed(MouseButton.Left) && game.IsActive)
            {
                ScreenManager.ShowScreen(new Credits(game, this));
            }
            else
            {
                creditsPadding = " >> ";
            }
        }
        creditsPadSize = bmfont.MeasureString(creditsPadding);
        creditsPadSize.Y = 0;

        if (game.DebugMode)
        {
            if (KeyboardExtended.GetState().WasKeyPressed(Keys.Escape))
                game.Exit();
        }
    }

    public bool RareBGM { get; private set; } = false;
    public int BGOpacity { get; private set; } = 0;
    public int StaticOpacity { get; private set; } = 0;
    private Timer BGOpacityTimer;
    private Timer StaticOpacityTimer;
    private Timer SpriteTimer;
    private readonly Random rng = new(Guid.NewGuid().GetHashCode());
    public override void Initialize()
    {
        Mouse.SetCursor(MouseCursor.Arrow);
        if (game.DebugMode)
        {
            RareBGM = rng.Next(3) == 0;
        }
        else
        {
            RareBGM = rng.Next(6) == 1;
        }
        BGOpacityTimer = new Timer(90);
        BGOpacityTimer.Elapsed += (_, _) =>
        {
            BGOpacity = rng.Next(250);
        };
        BGOpacityTimer.AutoReset = true;
        BGOpacityTimer.Enabled = true;

        SpriteTimer = new Timer(300);
        SpriteTimer.Elapsed += (_, _) =>
        {
            int bg = rng.Next(100);
            int index = (bg > 96) ? bg - 96 : 0;
            bg_texture = bg_animation[index];
        };
        SpriteTimer.AutoReset = true;
        SpriteTimer.Enabled = false;

        StaticOpacity = 50 + rng.Next(100);
        StaticOpacityTimer = new Timer(80);
        StaticOpacityTimer.Elapsed += (_, _) =>
        {
            StaticOpacity = 50 + rng.Next(100);
        };
        StaticOpacityTimer.AutoReset = true;
        StaticOpacityTimer.Enabled = true;
        base.Initialize();
    }

    public override void LoadContent()
    {
        if (game.Client.IsConnected && !lobby)
        {
            game.Client.Disconnect();
        }
        game.Audio.StopAll();
        bmfont = Content.Load<BitmapFont>("font/b_volter32");
        verfont = Content.Load<BitmapFont>("font/nunito16");
        vertext = $"v{game.Version}";
        var versize = verfont.MeasureString(vertext);
        verpos = new(game.WindowSize.X - versize.Width - 5, game.WindowSize.Y - versize.Height);
        logo = Content.Load<Texture2D>("menu/logo");
        bg_animation = new MenuBgAnim(Content);
        try
        {
            if (RareBGM)
            {
                Debug.WriteLine("I ❤ FNaF57");
                music = Content.Load<SoundEffect>("menu/fnaf57")?.CreateInstance();
                music.Volume = 0.5f;
            }
            else
            {
                music = Content.Load<SoundEffect>("menu/ambience")?.CreateInstance();
                music.Volume = 0.2f;
            }
        }
        catch (Exception)
        {
            game.Audio.NoAudio = true;
        }
        bg_texture = bg_animation[0];
        BGOpacity = rng.Next(250);
        static_animation = new StaticAnim(Content);
        if (music != null)
        {
            music.IsLooped = true;
            music.Play();
        }
        startPadSize = bmfont.MeasureString(startPadding);
        startPadSize.Y = 0;
        creditsPadSize = bmfont.MeasureString(creditsPadding);
        creditsPadSize.Y = 0;
        SpriteTimer.Enabled = true;
        base.LoadContent();
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
        music?.Dispose();
        BGOpacityTimer?.Dispose();
        SpriteTimer?.Dispose();
        StaticOpacityTimer.Dispose();
    }
}
