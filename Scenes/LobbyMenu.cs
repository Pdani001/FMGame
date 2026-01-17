using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGameGum;
using ReFMGame.Network;
using System;
using System.Diagnostics;

namespace ReFMGame.Scenes;
public class LobbyMenu(FMGame game, MainMenu menu) : GameScreen(game)
{
    readonly Rectangle backButton = new(0, 0, 128, 48);
    Vector2 backPos;
    BitmapFont small;
    BitmapFont smallB;
    string InfoText = "Connecting to server...";
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        Color bgcolor = Color.White;
        bgcolor.A = (byte)(255 - menu.BGOpacity);
        game.SpriteBatch.Draw(menu.bg_texture, Vector2.Zero, null, bgcolor, 0, Vector2.Zero , 1, SpriteEffects.None, 0f);

        game.SpriteBatch.DrawString(smallB, "Back", backPos, Color.White);
        game.SpriteBatch.Draw(menu.logo, new(68, 50), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
        if(menu.RareBGM)
            game.SpriteBatch.DrawString(menu.bmfont, "57", new(79, 190), Color.Yellow);
        if (game.DebugMode)
        {
            game.SpriteBatch.DrawRectangle(backButton, new(163, 87, 171));
        }
        game.SpriteBatch.DrawString(small, InfoText, new(32, 640), Color.White);
        game.SpriteBatch.DrawString(menu.verfont, menu.vertext, menu.verpos, Color.White);
        game.SpriteBatch.End();

        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        game.GumUI.Draw();
        game.SpriteBatch.End();

        Color staticcolor = Color.White;
        staticcolor.A = (byte)(255 - menu.StaticOpacity);
        game.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
        game.SpriteBatch.Draw(menu.static_animation[menu.static_animation.Index], Vector2.Zero, null, staticcolor, 0, Vector2.Zero , 1, SpriteEffects.None, .4f);
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        menu.static_animation.Animate(gameTime);
        if ((backButton.Contains(game.MouseState.Position) && MouseExtended.GetState().WasButtonPressed(MouseButton.Left)) || KeyboardExtended.GetState().WasKeyPressed(Keys.Escape))
        {
            ScreenManager.CloseScreen();
            game.Client.Disconnect();
        }
    }

    TextBox nickname;
    TextBox channelname;
    Button refresh;
    Button create;
    Button join;
    public override void LoadContent()
    {
        game.Client.Connected += Client_Connected;
        game.Client.ConnectFailed += Client_ConnectFailed;
        game.Client.JoinedChannel += Client_JoinedChannel;
        game.Client.LeftChannel += Client_LeftChannel;
        game.Client.ChannelListReceived += Client_ChannelListReceived;
        game.Client.Error += Client_Error;
#if DEBUG
        game.Client.ServerSecretAccepted += Client_ServerSecretAccepted;
#endif

        if (!game.Client.IsConnected)
            game.Client.Connect();
        else
        {
            InfoText = "";
            game.Client.LeaveChannel();
        }

        small = Content.Load<BitmapFont>("font/nunito20");
        //var test = new RenderingLibrary.Graphics.BitmapFont("font/vui20.fnt");
        smallB = Content.Load<BitmapFont>("font/nunito20b");
        SizeF backSize = smallB.MeasureString("Back");
        backPos = new(64 - backSize.Width / 2, 24 - backSize.Height / 2);
        nickname = new TextBox
        {
            X = 64,
            Y = 249,
            Width = 319,
            Height = 32,
            Placeholder = "Nickname",
            MaxLength = 24,
            IsEnabled = false,
        };
        var nicknameVisual = (TextBoxVisual)nickname.Visual;
        nicknameVisual.TextInstance.CustomFontFile = "font/ui20.fnt";
        nicknameVisual.TextInstance.UseCustomFont = true;
        nicknameVisual.PlaceholderTextInstance.CustomFontFile = "font/ui20.fnt";
        nicknameVisual.PlaceholderTextInstance.UseCustomFont = true;
        nicknameVisual.BackgroundColor = Color.Azure;
        nicknameVisual.ForegroundColor = Color.Black;

        channelname = new TextBox
        {
            X = 64,
            Y = 287,
            Width = 319,
            Height = 32,
            Placeholder = "Lobby name",
            MaxLength = 24,
            IsEnabled = false,
        };
        var channelnameVisual = (TextBoxVisual)channelname.Visual;
        channelnameVisual.TextInstance.CustomFontFile = "font/ui20.fnt";
        channelnameVisual.TextInstance.UseCustomFont = true;
        channelnameVisual.PlaceholderTextInstance.CustomFontFile = "font/ui20.fnt";
        channelnameVisual.PlaceholderTextInstance.UseCustomFont = true;
        channelnameVisual.BackgroundColor = Color.Azure;
        channelnameVisual.ForegroundColor = Color.Black;

        channelname.KeyDown += Channelname_KeyDown;

        refresh = new Button
        {
            X = 64,
            Y = 324,
            Width = 64,
            Height = 10,
            Text = "Refresh",
            IsEnabled = false,
            GamepadTabbingFocusBehavior = TabbingFocusBehavior.SkipOnTab,
        };
        var refreshVisual = (ButtonVisual)refresh.Visual;
        refreshVisual.TextInstance.CustomFontFile = "font/ui16.fnt";
        refreshVisual.TextInstance.UseCustomFont = true;
        refreshVisual.BackgroundColor = Color.DimGray;

        create = new Button
        {
            X = 319,
            Y = 324,
            Width = 64,
            Height = 10,
            Text = "Create",
            IsEnabled = false,
            GamepadTabbingFocusBehavior = TabbingFocusBehavior.SkipOnTab,
        };
        var createVisual = (ButtonVisual)create.Visual;
        createVisual.TextInstance.CustomFontFile = "font/ui16.fnt";
        createVisual.TextInstance.UseCustomFont = true;
        createVisual.BackgroundColor = Color.Red;

        join = new Button
        {
            X = 319,
            Y = 326 + create.ActualHeight,
            Width = 64,
            Height = 10,
            Text = "Join",
            IsEnabled = false,
            GamepadTabbingFocusBehavior = TabbingFocusBehavior.SkipOnTab,
        };
        var joinVisual = (ButtonVisual)join.Visual;
        joinVisual.TextInstance.CustomFontFile = "font/ui16.fnt";
        joinVisual.TextInstance.UseCustomFont = true;
        joinVisual.BackgroundColor = Color.Green;

        refresh.Click += Refresh_Click;
        create.Click += Create_Click;
        join.Click += Join_Click;

        join.AddToRoot();
        create.AddToRoot();
        refresh.AddToRoot();
        nickname.AddToRoot();
        channelname.AddToRoot();
        base.LoadContent();
    }

    private void Channelname_KeyDown(object sender, KeyEventArgs e)
    {
        if(e.Key == Keys.Enter)
        {
            join.PerformClick();
        }
    }

    private bool joining = false;

    private void Join_Click(object sender, EventArgs e)
    {
        InfoText = "Joining lobby...";
        joining = true;
        game.Client.JoinChannel(channelname.Text, nickname.Text);
        join.IsEnabled = false;
        create.IsEnabled = false;
        refresh.IsEnabled = false;
        nickname.IsEnabled = false;
        channelname.IsEnabled = false;
    }

    private void Create_Click(object sender, EventArgs e)
    {
        InfoText = "Creating lobby...";
        joining = true;
        game.Client.CreateChannel(channelname.Text, nickname.Text);
        join.IsEnabled = false;
        create.IsEnabled = false;
        refresh.IsEnabled = false;
        nickname.IsEnabled = false;
        channelname.IsEnabled = false;
    }

    private void Refresh_Click(object sender, EventArgs e)
    {
        InfoText = "";
        refresh.IsEnabled = false;
        game.Client.RequestChannelList();
    }

    public override void UnloadContent()
    {
        game.Client.Connected -= Client_Connected;
        game.Client.ConnectFailed -= Client_ConnectFailed;
        game.Client.JoinedChannel -= Client_JoinedChannel;
        game.Client.LeftChannel -= Client_LeftChannel;
        game.Client.ChannelListReceived -= Client_ChannelListReceived;
        game.Client.Error -= Client_Error;
#if DEBUG
        game.Client.ServerSecretAccepted -= Client_ServerSecretAccepted;
#endif

        channelname.KeyDown -= Channelname_KeyDown;
        refresh.Click -= Refresh_Click;
        create.Click -= Create_Click;
        join.Click -= Join_Click;

        join.RemoveFromRoot();
        create.RemoveFromRoot();
        refresh.RemoveFromRoot();
        nickname.RemoveFromRoot();
        channelname.RemoveFromRoot();
        base.UnloadContent();
    }

    private void Client_Error(string error)
    {
        InfoText = error;
        if (joining)
        {
            joining = false;
            join.IsEnabled = true;
            create.IsEnabled = true;
            refresh.IsEnabled = true;
            nickname.IsEnabled = true;
            channelname.IsEnabled = true;
        }
    }

    private void Client_ConnectFailed(string error)
    {
        Debug.WriteLine($"Connection failed: {error}");
        InfoText = $"Failed to connect to server: {error}";
    }

    private void Client_JoinedChannel(Channel channel, string error)
    {
        if (error == null)
        {
            Debug.WriteLine($"Joined channel: {channel.Name}");
            ScreenManager.ClearScreens();
            ScreenManager.ShowScreen(new Select(game));
        }
        else
        {
            joining = false;
            game.Client.RequestChannelList();
            InfoText = $"Failed to join: {error}";
        }
    }

    private void Client_Connected()
    {
        InfoText = "";
#if DEBUG
        Debug.WriteLine("Connected to server in DEBUG mode.");
        game.Client.SendServerSecret(Environment.GetEnvironmentVariable("SERVER_SECRET") ?? "");
#else
        game.Client.RequestChannelList();
#endif
    }

    private void Client_ChannelListReceived(Channel[] list)
    {
        if (joining)
            return;
        join.IsEnabled = true;
        create.IsEnabled = true;
        refresh.IsEnabled = true;
        nickname.IsEnabled = true;
        nickname.IsFocused = true;
        channelname.IsEnabled = true;
    }

    private void Client_LeftChannel(string name)
    {
        game.Client.RequestChannelList();
    }

#if DEBUG
    private void Client_ServerSecretAccepted()
    {
        game.Client.RequestChannelList();
    }
#endif
}
