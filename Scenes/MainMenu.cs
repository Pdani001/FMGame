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
public class MainMenu(GameExtended game, bool lobby = false, bool rare = false) : GameScreen(game)
{
    public Texture2D bg_texture { get; private set; }
    public Texture2D logo { get; private set; }
    private SoundEffect ambience;
    private SoundEffect theme;
    private SoundEffectInstance music = null;
    private int RandomTheme = 0;
    public TextureAnimation bg_animation { get; private set; }
    public TextureAnimation static_animation { get; private set; }
    public BitmapFont bmfont { get; private set; }
    public BitmapFont verfont { get; private set; }
    public string vertext { get; private set; }
    public Vector2 verpos { get; private set; }
    private sbyte active = 0;
    private bool mouseActive = false;
    private Rectangle start = new(128, 352, 108, 32);
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
        
        game.SpriteBatch.DrawString(bmfont, startPadding+"Play", start.Location.ToVector2() - startPadSize, Color.White);
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
        static_animation.Animate(gameTime);
        if(!game.Audio.NoAudio && !RareBGM && music?.State != SoundState.Playing)
        {
            RandomTheme = rng.Next(RandomTheme == 5 ? 4 : 5) + 1;
            music?.Dispose();
            music = RandomTheme == 5 ? theme.CreateInstance() : ambience.CreateInstance();
            music.Volume = 0.2f;
            music.Play();
        }
        if (!IsActive)
            return;
        if (lobby)
        {
            lobby = false;
            ScreenManager.ShowScreen(new LobbyMenu(game, this));
            return;
        }
        if(game.IsActive && KeyboardExtended.GetState().WasKeyPressed(Keys.Up))
        {
            active -= 1;
            if(active < 0)
                active = 2;
        }
        if(game.IsActive && KeyboardExtended.GetState().WasKeyPressed(Keys.Down))
        {
            active += 1;
            if(active > 2)
                active = 0;
        }

        mouseActive = game.IsActive && (start.Contains(game.MouseState.Position) || settings.Contains(game.MouseState.Position) || credits.Contains(game.MouseState.Position));
        Mouse.SetCursor(MouseCursor.Arrow);
        startPadding = active != 0 ? "    " : " >> ";
        if (game.IsActive && start.Contains(game.MouseState.Position))
        {
            active = 0;
            startPadding = " >> ";
        }
        startPadSize = bmfont.MeasureString(startPadding);
        startPadSize.Y = 0;

        settingsPadding = active != 1 ? "    " : " >> ";
        if (game.IsActive && settings.Contains(game.MouseState.Position))
        {
            active = 1;
            settingsPadding = " >> ";
        }
        settingsPadSize = bmfont.MeasureString(settingsPadding);
        settingsPadSize.Y = 0;

        creditsPadding = active != 2 ? "    " : " >> ";
        if (game.IsActive && credits.Contains(game.MouseState.Position))
        {
            active = 2;
            creditsPadding = " >> ";
        }
        creditsPadSize = bmfont.MeasureString(creditsPadding);
        creditsPadSize.Y = 0;

        if((mouseActive && MouseExtended.GetState().WasButtonPressed(MouseButton.Left)) || (game.IsActive && KeyboardExtended.GetState().WasKeyPressed(Keys.Enter)))
        {
            switch(active)
            {
                case 0:
                    ScreenManager.ShowScreen(new LobbyMenu(game, this));
                    break;
                case 1:
                    ScreenManager.ShowScreen(new SettingsMenu(game, this));
                    break;
                case 2:
                    ScreenManager.ShowScreen(new CreditsMenu(game, this));
                    break;
            }
        }

        if (game.DebugMode)
        {
            if (game.IsActive && KeyboardExtended.GetState().WasKeyPressed(Keys.Escape))
                game.Exit();
        }
    }

    public bool RareBGM { get; private set; } = rare;
    public int BGOpacity { get; private set; } = 0;
    public int StaticOpacity { get; private set; } = 0;
    private Timer BGOpacityTimer;
    private Timer StaticOpacityTimer;
    private Timer SpriteTimer;
    private readonly Random rng = new(Guid.NewGuid().GetHashCode());
    public override void Initialize()
    {
        UpdateWhenInactive = true;
        Mouse.SetCursor(MouseCursor.Arrow);
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
                music.IsLooped = true;
                music.Volume = 0.5f;
            }
            else
            {
                ambience = Content.Load<SoundEffect>("menu/ambience");
                theme = Content.Load<SoundEffect>("menu/theme");
                RandomTheme = rng.Next(RandomTheme == 5 ? 4 : 5) + 1;
                music = RandomTheme == 5 ? theme.CreateInstance() : ambience.CreateInstance();
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
