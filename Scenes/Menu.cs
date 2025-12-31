using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGameGum;
using ReFMGame.Animations;
using ReFMGame.GameHelper;
using ReFMGame.Network;
using System;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace ReFMGame.Scenes;
public class Menu(FMGame game) : GameScreen(game)
{
    private Texture2D bg_texture;
    private Texture2D logo;
    private SoundEffectInstance music;
    private TextureAnimation bg_animation;
    private TextureAnimation static_animation;
    private BitmapFont bmfont;
    private float elapsed = 0;
    private Rectangle start = new(128, 352, 138, 32);
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        Color bgcolor = Color.White;
        bgcolor.A = (byte)(255 - BGOpacity);
        game.SpriteBatch.Draw(bg_texture, Vector2.Zero, null, bgcolor, 0, Vector2.Zero , 1, SpriteEffects.None, 0);
        string padding = "    ";
        if (start.Contains(Mouse.GetState().Position))
            padding = " >> ";
        Vector2 padSize = bmfont.MeasureString(padding);
        padSize.Y = 0;
        game.SpriteBatch.DrawString(bmfont, padding+"Start", start.Location.ToVector2() - padSize, Color.White);
        if (!RareBG)
            game.SpriteBatch.Draw(logo, new(68, 50), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
        else
        {
            Color color = Color.White;
            string text = "- Press Start -";
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= 1)
                elapsed = 0;
            if (elapsed >= .5)
                color.A = 0;
            Vector2 size = bmfont.MeasureString(text);
            game.SpriteBatch.DrawString(bmfont, text, new(640 - size.X / 2, 620), color);
        }
        game.SpriteBatch.End();

        if (!RareBG)
        {
            Color staticcolor = Color.White;
            staticcolor.A = (byte)(255 - StaticOpacity);
            game.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
            game.SpriteBatch.Draw(static_animation[static_animation.Index], Vector2.Zero, null, staticcolor, 0, Vector2.Zero , 1, SpriteEffects.None, .4f);
            game.SpriteBatch.End();
        }
    }

    public override void Update(GameTime gameTime)
    {
        static_animation.Animate(gameTime);
        if (start.Contains(Mouse.GetState().Position) && MouseExtended.GetState().WasButtonPressed(MouseButton.Left))
        {
            ScreenManager.ReplaceScreen(new Loading(game));
            //ScreenManager.ReplaceScreen(new NextDay(game, game.GetScreenshot()));
        }

    }

    private bool RareBG = false;
    private int BGOpacity = 0;
    private int StaticOpacity = 0;
    private Timer BGOpacityTimer;
    private Timer StaticOpacityTimer;
    private Timer SpriteTimer;
    private readonly Random rng = new(Guid.NewGuid().GetHashCode());
    public override void Initialize()
    {
        if (game.DebugMode)
        {
            RareBG = rng.Next(3) == 0;
        }
        else
        {
            RareBG = rng.Next(6) == 1;
        }
        BGOpacityTimer = new Timer(90);
        BGOpacityTimer.Elapsed += (_, _) =>
        {
            BGOpacity = rng.Next(250);
        };
        BGOpacityTimer.AutoReset = !RareBG;
        BGOpacityTimer.Enabled = !RareBG;

        SpriteTimer = new Timer(300);
        SpriteTimer.Elapsed += (_, _) =>
        {
            int bg = rng.Next(100);
            int index = (bg > 96) ? bg - 96 : 0;
            bg_texture = bg_animation[index];
        };
        SpriteTimer.AutoReset = !RareBG;
        SpriteTimer.Enabled = false;

        StaticOpacity = 50 + rng.Next(100);
        StaticOpacityTimer = new Timer(80);
        StaticOpacityTimer.Elapsed += (_, _) =>
        {
            StaticOpacity = 50 + rng.Next(100);
        };
        StaticOpacityTimer.AutoReset = !RareBG;
        StaticOpacityTimer.Enabled = !RareBG;
        base.Initialize();
    }

    private void Client_ConnectFailed(string error)
    {
        Debug.WriteLine($"Connection failed: {error}");
    }

    private void Client_JoinedChannel(Channel channel, string error)
    {
        if(error == null)
        {
            Debug.WriteLine($"Joined channel: {channel.Name}");
        }
        else
        {
            Debug.WriteLine($"Failed to join channel: {error}");
        }
    }

    private void Client_NicknameUpdate(bool success, string error)
    {
        if (success)
        {
            Debug.WriteLine("Nickname set successfully.");
            game.Client.JoinChannel("asd");
        }
        else
        {
            Debug.WriteLine($"Failed to set nickname: {error}");
        }
    }

    private void Client_Connected()
    {
#if DEBUG
        Debug.WriteLine("Connected to server in DEBUG mode.");
        game.Client.SendServerSecret(Environment.GetEnvironmentVariable("SERVER_SECRET") ?? "");
#else
        game.Client.SetNickname($"Player{rng.Next(100,9999)}");
#endif
    }

    public override void LoadContent()
    {
        game.Client.Connected += Client_Connected;
        game.Client.ConnectFailed += Client_ConnectFailed;
        game.Client.NicknameUpdate += Client_NicknameUpdate;
        game.Client.JoinedChannel += Client_JoinedChannel;
#if DEBUG
        game.Client.ServerSecretAccepted += Client_ServerSecretAccepted;
#endif

        //if (!game.Client.IsConnected)
        //    game.Client.Connect();
        //else
        //{
        //    game.Client.LeaveChannel();
        //}
        game.Audio.StopAll();
        bmfont = Content.Load<BitmapFont>("font/b_volter32");
        logo = Content.Load<Texture2D>("menu/logo");
        bg_animation = new MenuBgAnim(Content);
        if (RareBG){
            bg_texture = Content.Load<Texture2D>("menu/rare");
            Debug.WriteLine("I ❤ FNaF57");
            music = Content.Load<SoundEffect>("menu/fnaf57")?.CreateInstance();
            music.Volume = 0.5f;
            BGOpacity = 0;
        } else {
            bg_texture = bg_animation[0];
            music = Content.Load<SoundEffect>("menu/ambience")?.CreateInstance();
            music.Volume = 0.2f;
            BGOpacity = rng.Next(250);
            SpriteTimer.Enabled = true;
        }
        music.IsLooped = true;
        static_animation = new StaticAnim(Content);
        music.Play();
        base.LoadContent();
    }

#if DEBUG
    private void Client_ServerSecretAccepted()
    {
        game.Client.SetNickname(Environment.GetEnvironmentVariable("NICKNAME") ?? $"Player{rng.Next(100, 9999)}");
    }
#endif

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
        music.Dispose();
        BGOpacityTimer?.Dispose();
        SpriteTimer?.Dispose();
        StaticOpacityTimer.Dispose();
        game.Client.Connected -= Client_Connected;
        game.Client.NicknameUpdate -= Client_NicknameUpdate;
        game.Client.JoinedChannel -= Client_JoinedChannel;
#if DEBUG
        game.Client.ServerSecretAccepted -= Client_ServerSecretAccepted;
#endif
    }
}
