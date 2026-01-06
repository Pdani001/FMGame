using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGameGum;
using MonoGameGum.ExtensionMethods;
using MonoGameGum.GueDeriving;
using ReFMGame.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ReFMGame.Scenes;
public class Select(FMGame game) : GameScreen(game)
{
    BitmapFont nunito;
    SizeF size;
    readonly Rectangle[] charPos =
    {
        new(1038, 64, 200, 200),
        new(42, 64, 200, 200),
        new(291, 64, 200, 200),
        new(540, 64, 200, 200),
        new(789, 64, 200, 200),
    };
    readonly Rectangle readyPos = new(1020, 600, 186, 56);
    readonly Rectangle backButton = new(0, 0, 128, 48);
    readonly bool[] isCrossed =
    {
        false,
        false,
        true,
        true,
        true
    };
    readonly bool[] isReady =
    {
        false,
        false,
        false,
        false,
        false
    };
    readonly Guid[] selected =
    [
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
    ];
    Texture2D[] charIcons;
    Texture2D check;
    Texture2D cross;
    Texture2D ready;
    SoundEffect error;
    readonly Color selectColor = new(255, 255, 255, 127);

    Character Character = Character.None;

    private MouseListener _mouseListener;
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        game.SpriteBatch.DrawString(nunito, "Back", new(64-size.Width/2,24-size.Height/2), Color.White);

        for(int i = 0; i < charIcons.Length; i++)
        {
            game.SpriteBatch.Draw(charIcons[i], charPos[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            if(isCrossed[i])
                game.SpriteBatch.Draw(cross, charPos[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
            else if (selected[i] != Guid.Empty)
                game.SpriteBatch.Draw(check, charPos[i].Location.ToVector2(), null, !isReady[i] ? selectColor : Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
        }

        game.SpriteBatch.Draw(ready, readyPos, Color.White);

        if (game.DebugMode)
        {
            game.SpriteBatch.DrawRectangle(backButton, new(163, 87, 171));
        }

        game.GumUI.Draw();
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        _mouseListener.Update(gameTime);
    }

    TextBox ChatBox;
    ScrollBar ScrollBar;
    TextBox MessageBox;
    TextRuntime ChatText;
    BitmapFont ui;
    public override void LoadContent()
    {
        nunito = Content.Load<BitmapFont>("font/nunito20b");
        ui = Content.Load<BitmapFont>("font/nunito16");
        size = nunito.MeasureString("Back");
        charIcons = [
            Content.Load<Texture2D>("select/guard"),
            Content.Load<Texture2D>("select/freddy"),
            Content.Load<Texture2D>("select/bonnie"),
            Content.Load<Texture2D>("select/chica"),
            Content.Load<Texture2D>("select/foxy"),
        ];
        check = Content.Load<Texture2D>("select/checkmark");
        cross = Content.Load<Texture2D>("select/crossmark");
        ready = Content.Load<Texture2D>("select/ready");
        error = Content.Load<SoundEffect>("error");

        var settings = new MouseListenerSettings();
        settings.DoubleClickMilliseconds = int.MinValue;
        settings.DragThreshold = int.MaxValue;
        _mouseListener = new MouseListener(settings);
        _mouseListener.MouseClicked += MouseClicked;

        game.Client.ChannelUserJoined += Client_ChannelUserJoined;
        game.Client.ChannelUserLeft += Client_ChannelUserLeft;
        game.Client.UserReady += Client_UserReady;
        game.Client.CharacterSelected += Client_CharacterSelected;
        game.Client.GameCountdown += Client_GameCountdown;
        game.Client.GameStart += Client_GameStart;
        game.Client.ChatMessageReceived += Client_ChatMessageReceived;

        ScrollBar = new ScrollBar
        {
            X = 768,
            Y = 320,
            Height = 286,
            Minimum = 0,
            Maximum = 0,
        };
        var scrollbarvisual = (ScrollBarVisual)ScrollBar.Visual;
        scrollbarvisual.ThumbInstance.BackgroundColor = Color.LightGray;
        scrollbarvisual.UpButtonInstance.BackgroundColor = Color.LightGray;
        scrollbarvisual.UpButtonIcon.Color = Color.Black;
        scrollbarvisual.DownButtonInstance.BackgroundColor = Color.LightGray;
        scrollbarvisual.DownButtonIcon.Color = Color.Black;

        ScrollBar.ValueChanged += ScrollBar_ValueChanged;


        ChatBox = new TextBox
        {
            X = 96,
            Y = 320,
            Width = 672,
            Height = 286,
            IsReadOnly = true,
            Text = "Welcome to Fazbear Multiplayer!",
            TextWrapping = Gum.Forms.TextWrapping.Wrap,
        };

        var chatboxvisual = (TextBoxVisual)ChatBox.Visual;
        chatboxvisual.TextInstance.CustomFontFile = "font/ui16.fnt";
        chatboxvisual.TextInstance.UseCustomFont = true;
        ChatText = chatboxvisual.TextInstance;
        chatboxvisual.FocusedIndicatorColor = Color.Transparent;
        chatboxvisual.BackgroundColor = Color.LightGray;
        chatboxvisual.ForegroundColor = Color.Black;
        chatboxvisual.TextOverflowVerticalMode = RenderingLibrary.Graphics.TextOverflowVerticalMode.SpillOver;

        MessageBox = new TextBox
        {
            X = 96,
            Y = 606,
            Width = 672 + ScrollBar.Width,
            Height = 30,
            Placeholder = "Press ENTER to send"
        };

        var messageboxvisual = (TextBoxVisual)MessageBox.Visual;
        messageboxvisual.TextInstance.CustomFontFile = "font/ui16.fnt";
        messageboxvisual.TextInstance.UseCustomFont = true;
        messageboxvisual.PlaceholderTextInstance.CustomFontFile = "font/ui16.fnt";
        messageboxvisual.PlaceholderTextInstance.UseCustomFont = true;
        messageboxvisual.FocusedIndicatorColor = Color.Transparent;
        messageboxvisual.BackgroundColor = Color.LightGray;
        messageboxvisual.ForegroundColor = Color.Black;

        MessageBox.KeyDown += (_, args) =>
        {
            if(MessageBox.Text?.Length > 0 && args.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                game.Client.SendMessage(MessageBox.Text);
                MessageBox.Text = "";
            }
        };

        ChatBox.AddToRoot();
        ScrollBar.AddToRoot();
        MessageBox.AddToRoot();

        base.LoadContent();
    }

    private void Client_ChatMessageReceived(Client user, string text)
    {
        ChatMessage($"{user.Nick}: {text}");
    }

    private void ScrollBar_ValueChanged(object sender, EventArgs e)
    {
        if(ScrollBar.Maximum > 0)
        {
            ChatText.Y = (float)-ScrollBar.Value;
        }
    }

    private void ChatMessage(string message)
    {
        ChatBox.Text += $"\n{message}";

        float textHeight = ui.MeasureString(ChatText.Text).Height;
        float containerHeight = ChatBox.ActualHeight;

        if (textHeight > containerHeight)
        {
            // Move the text up so the bottom of the text aligns with the bottom of the box
            ScrollBar.Maximum = textHeight - containerHeight + 4;
            ScrollBar.Value = ScrollBar.Maximum;
            ChatText.Y = -(float)ScrollBar.Maximum;
        }
        else
        {
            ChatText.Y = 0; // It fits, no scroll needed
        }
    }

    private void Client_ChannelUserLeft(Client user)
    {
        ChatMessage($"> {user.Nick} left.");
        if (selected.Contains(user.Id))
        {
            var index = selected.IndexOf(user.Id);
            isReady[index] = false;
            selected[index] = Guid.Empty;
        }
    }

    private void Client_GameStart(CharacterPosition[] _)
    {
        ScreenManager.ReplaceScreen(new Loading(game, Character));
    }

    private void Client_GameCountdown(int seconds)
    {
        Debug.WriteLine($"> Countdown: {seconds}");
    }

    private void Client_CharacterSelected(Client user, Character character)
    {
        var nick = user.Nick;
        var target = "their";
        if(user.Id == game.Client.Self.Id)
        {
            Character = character;
            nick = "You";
            target = "your";
        }
        if (character == Character.None)
        {
            if(selected.Contains(user.Id))
                selected[selected.IndexOf(user.Id)] = Guid.Empty;
            ChatMessage($"> {nick} unselected {target} character");
        }
        else
        {
            ChatMessage($"> {nick} selected {character}");
            selected[(int)character] = user.Id;
        }
    }

    private void Client_UserReady(Character character, bool ready)
    {
        isReady[(int)character] = ready;
        var print = ready ? "ready" : "NOT ready";
        var nick = $"{character} is";
        if (this.Character == character)
            nick = "You are";
        ChatMessage($"> {nick} {print}");
    }

    private void Client_ChannelUserJoined(Client user, List<Selected> selected)
    {
        ChatMessage($"> {user.Nick} joined.");
        if (user.Id == game.Client.Self.Id)
        {
            ChatMessage($"> Currently playing: {string.Join(", ", game.Client.Channel.Clients.Select(c => c.Nick))}");
            foreach(var item in selected)
            {
                int i = (int)item.Character;
                this.selected[i] = item.Id;
                this.isReady[i] = item.Ready;
            }
        }
    }

    private void MouseClicked(object sender, MouseEventArgs e)
    {
        Vector2 position = game.MouseState.Position;
        if (e.Button != MouseButton.Left)
        {
            return;
        }
        if (backButton.Contains(position))
        {
            ScreenManager.ReplaceScreen(new Menu(game, true));
            return;
        }
        if (readyPos.Contains(position) && Character != Character.None)
        {
            //isReady[(int)character] = !isReady[(int)character];
            game.Client.SetReady(!isReady[(int)Character]);
            return;
        }
        for (int i = 0; i < charPos.Length; i++)
        {
            if (charPos[i].Contains(position))
            {
                if (isCrossed[i])
                {
                    error.Play();
                }
                else
                {
                    if (Character != Character.None && i != (int)Character)
                        break;
                    if (selected[i] == Guid.Empty)
                    {
                        game.Client.SelectCharacter(i);
                    }
                    else if (selected[i] == game.Client.Self.Id && !isReady[i])
                    {
                        game.Client.SelectCharacter((int)Character.None);
                    }
                }
                break;
            }
        }
    }

    public override void UnloadContent()
    {
        _mouseListener.MouseClicked -= MouseClicked;
        game.Client.ChannelUserJoined -= Client_ChannelUserJoined;
        game.Client.ChannelUserLeft -= Client_ChannelUserLeft;
        game.Client.UserReady -= Client_UserReady;
        game.Client.CharacterSelected -= Client_CharacterSelected;
        game.Client.GameCountdown -= Client_GameCountdown;
        game.Client.GameStart -= Client_GameStart;
        game.Client.ChatMessageReceived -= Client_ChatMessageReceived;

        ScrollBar.RemoveFromRoot();
        ChatBox.RemoveFromRoot();
        MessageBox.RemoveFromRoot();
        base.UnloadContent();
    }
}
