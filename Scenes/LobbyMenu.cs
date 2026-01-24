using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGameGum;
using ReFMGame.Network;
using System;
using System.Diagnostics;
using System.Linq;

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
        listClick -= (float)(listClick > 0 ? Math.Min(listClick, gameTime.ElapsedGameTime.TotalSeconds) : 0);
        _mouseListener.Update(gameTime);
        if ((backButton.Contains(game.MouseState.Position) && MouseExtended.GetState().WasButtonPressed(MouseButton.Left)) || KeyboardExtended.GetState().WasKeyPressed(Keys.Escape))
        {
            ScreenManager.CloseScreen();
            game.Client.Disconnect();
        }
    }

    private float listClick = 0;
    private MouseListener _mouseListener;
    private TextBox nickname;
    private TextBox channelname;
    private PasswordBox password;
    private ListBox lobbylist;
    private Channel[] channels = [];
    private Button refresh;
    private Button create;
    private Button join;
    public override void LoadContent()
    {
        game.Client.Connected += Client_Connected;
        game.Client.ConnectFailed += Client_ConnectFailed;
        game.Client.JoinedChannel += Client_JoinedChannel;
        game.Client.LeftChannel += Client_LeftChannel;
        game.Client.ChannelListReceived += Client_ChannelListReceived;
        game.Client.Error += Client_Error;
        game.Client.ServerSecretAccepted += Client_ServerSecretAccepted;

        if (!game.Client.IsConnected)
            game.Client.Connect();
        else
        {
            InfoText = "";
            if (game.Client.Channel != null)
                game.Client.LeaveChannel();
            else
                game.Client.RequestChannelList();
        }

        var settings = new MouseListenerSettings
        {
            DragThreshold = int.MaxValue
        };
        _mouseListener = new MouseListener(settings);
        _mouseListener.MouseDoubleClicked += MouseDoubleClicked;

        small = Content.Load<BitmapFont>("font/nunito20");
        //var test = new RenderingLibrary.Graphics.BitmapFont("font/vui20.fnt");
        smallB = Content.Load<BitmapFont>("font/nunito20b");
        SizeF backSize = smallB.MeasureString("Back");
        backPos = new(64 - backSize.Width / 2, 24 - backSize.Height / 2);

        lobbylist = new ListBox
        {
            X = 546,
            Y = 249,
            Width = 448,
            Height = 344,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            IsEnabled = false,
            GamepadTabbingFocusBehavior = TabbingFocusBehavior.SkipOnTab,
        };
        var listVisual = (ListBoxVisual)lobbylist.Visual;
        listVisual.Background.ApplyState(Styling.ActiveStyle.NineSlice.OutlinedHeavy);

        lobbylist.SelectionChanged += Lobbylist_SelectionChanged;
        lobbylist.ItemClicked += Lobbylist_ItemClicked;

        password = new PasswordBox
        {
            X = 611,
            Y = 593,
            Width = 318,
            Height = 32,
            Placeholder = "Password",
            IsEnabled = false,
        };
        var passwordVisual = (PasswordBoxVisual)password.Visual;
        passwordVisual.TextInstance.CustomFontFile = "font/ui20.fnt";
        passwordVisual.TextInstance.UseCustomFont = true;
        passwordVisual.PlaceholderTextInstance.CustomFontFile = "font/ui20.fnt";
        passwordVisual.PlaceholderTextInstance.UseCustomFont = true;
        passwordVisual.BackgroundColor = Color.Azure;
        passwordVisual.ForegroundColor = Color.Black;

        password.KeyDown += Password_KeyDown;

        nickname = new TextBox
        {
            X = 64,
            Y = 249,
            Width = 319,
            Height = 32,
            Placeholder = "Nickname",
            MaxLength = 24,
            IsEnabled = false,
            Text = game.Client.Self?.Nick ?? "",
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
            X = lobbylist.X,
            Y = lobbylist.Y + lobbylist.ActualHeight,
            Width = 64,
            Height = 6,
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
            Y = 325,
            Width = 64,
            Height = 6,
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
            X = lobbylist.X + lobbylist.ActualWidth - 64,
            Y = lobbylist.Y + lobbylist.ActualHeight,
            Width = 64,
            Height = 6,
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

        password.AddToRoot();
        lobbylist.AddToRoot();
        join.AddToRoot();
        create.AddToRoot();
        refresh.AddToRoot();
        nickname.AddToRoot();
        channelname.AddToRoot();
        base.LoadContent();
    }

    private void Password_KeyDown(object _, KeyEventArgs e)
    {
        if (e.Key == Keys.Enter)
        {
            join.PerformClick();
        }
    }

    private void MouseDoubleClicked(object _, MouseEventArgs e)
    {
        if (listClick > 0)
        {
            join.PerformClick();
        }
    }

    private void Lobbylist_ItemClicked(object _, EventArgs e)
    {
        listClick = _mouseListener.DoubleClickMilliseconds / 1000f;
    }

    private void Lobbylist_SelectionChanged(object _, Gum.Wireframe.SelectionChangedEventArgs e)
    {
        if(lobbylist.SelectedIndex < 0) return;
        join.IsEnabled = true;
        Debug.WriteLine(lobbylist.SelectedIndex);
        Debug.WriteLine($"[{string.Join(", ",channels.Select(e=>$"'{e.ToString().Replace("\n","\\n")}' => {e.Password}"))}]");
        if (lobbylist.SelectedIndex >= channels.Length)
            return;
        password.Password = "";
        password.IsEnabled = channels[lobbylist.SelectedIndex].Password;
    }

    private void Channelname_KeyDown(object sender, KeyEventArgs e)
    {
        if(e.Key == Keys.Enter)
        {
            create.PerformClick();
        }
    }

    private bool joining = false;

    private void Join_Click(object sender, EventArgs e)
    {
        InfoText = "Joining lobby...";
        joining = true;
        Channel channel = channels[lobbylist.SelectedIndex];
        game.Client.JoinChannel(channel.Name, nickname.Text, password: password.Password);
        lobbylist.IsEnabled = false;
        join.IsEnabled = false;
        create.IsEnabled = false;
        refresh.IsEnabled = false;
        nickname.IsEnabled = false;
        channelname.IsEnabled = false;
        password.IsEnabled = false;
    }

    private void Create_Click(object sender, EventArgs e)
    {
        InfoText = "Creating lobby...";
        joining = true;
        game.Client.CreateChannel(channelname.Text, nickname.Text);
        lobbylist.IsEnabled = false;
        join.IsEnabled = false;
        create.IsEnabled = false;
        refresh.IsEnabled = false;
        nickname.IsEnabled = false;
        channelname.IsEnabled = false;
        password.IsEnabled = false;
    }

    private void Refresh_Click(object sender, EventArgs e)
    {
        InfoText = "";
        join.IsEnabled = false;
        refresh.IsEnabled = false;
        lobbylist.Items.Clear();
        channels = [];
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
        game.Client.ServerSecretAccepted -= Client_ServerSecretAccepted;

        _mouseListener.MouseDoubleClicked -= MouseDoubleClicked;

        lobbylist.SelectionChanged -= Lobbylist_SelectionChanged;
        lobbylist.ItemClicked -= Lobbylist_ItemClicked;
        channelname.KeyDown -= Channelname_KeyDown;
        password.KeyDown -= Password_KeyDown;
        refresh.Click -= Refresh_Click;
        create.Click -= Create_Click;
        join.Click -= Join_Click;

        password.RemoveFromRoot();
        lobbylist.RemoveFromRoot();
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
            join.IsEnabled = lobbylist.SelectedObject != null;
            if(lobbylist.SelectedObject != null)
                password.IsEnabled = channels[lobbylist.SelectedIndex].Password;
            lobbylist.IsEnabled = true;
            create.IsEnabled = true;
            refresh.IsEnabled = true;
            nickname.IsEnabled = true;
            nickname.IsFocused = nickname.Text.Length == 0;
            channelname.IsEnabled = true;
            channelname.IsFocused = nickname.Text.Length != 0;
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
        game.Client.SendServerSecret(Environment.GetEnvironmentVariable("SERVER_SECRET") ?? "");
#else
        if(game.ServerSecret == "")
            game.Client.RequestChannelList();
        else
            game.Client.SendServerSecret(game.ServerSecret);
#endif
    }

    private void Client_ChannelListReceived(Channel[] list)
    {
        if (joining)
            return;
        channels = list;
        lobbylist.Items.Clear();
        for (int i = 0; i < list.Length; i++)
        {
            ListBoxItem item = new();
            item.UpdateToObject(list[i]);
            var visual = (ListBoxItemVisual)item.Visual;
            visual.TextInstance.CustomFontFile = "font/vui20.fnt";
            visual.TextInstance.UseCustomFont = true;
            lobbylist.Items.Add(item);
        }
        password.Password = "";
        password.IsEnabled = false;
        join.IsEnabled = false;
        lobbylist.IsEnabled = true;
        create.IsEnabled = true;
        refresh.IsEnabled = true;
        nickname.IsEnabled = true;
        nickname.IsFocused = nickname.Text.Length == 0;
        channelname.IsEnabled = true;
        channelname.IsFocused = nickname.Text.Length != 0;
    }

    private void Client_LeftChannel(string name)
    {
        game.Client.RequestChannelList();
    }

    private void Client_ServerSecretAccepted()
    {
        game.Client.RequestChannelList();
    }
}
