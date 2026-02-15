using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGameGum;
using MonoGameGum.ExtensionMethods;
using ReFMGame.GameHelper;
using ReFMGame.Network;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ReFMGame.Scenes;
public class Select(FMGame game, bool lobby = true) : GameScreen(game)
{
    private OrthographicCamera _camera;
    private NumberRenderer CustomNightFont;
    SizeF backSize;
    readonly Rectangle[] charPos =
    {
        new(1038, 64, 200, 200),
        new(42, 64, 200, 200),
        new(291, 64, 200, 200),
        new(540, 64, 200, 200),
        new(789, 64, 200, 200),
    };
    readonly Vector2[] textPos =
    {
        new(1070, 256),
        new(74, 256),
        new(323, 256),
        new(572, 256),
        new(821, 256),
    };
    readonly Vector2[] customTextPos =
    {
        Vector2.Zero,
        new(118, 1627),
        new(403, 1627),
        new(682, 1627),
        new(957, 1627),
    };
    Rectangle[] customMinUpBtn =
    {
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
    };
    Rectangle[] customMinDownBtn =
    {
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
    };
    Rectangle[] customMaxUpBtn =
    {
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
    };
    Rectangle[] customMaxDownBtn =
    {
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
    };
    Rectangle[] aiLeftBtn =
    {
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
    };
    Rectangle[] aiRightBtn =
    {
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
        Rectangle.Empty,
    };
    readonly Vector2 MinTextOffset = new(-5, 35);
    readonly Vector2 MaxTextOffset = new(91, 35);
    readonly Vector2 AITextOffset = new(0, 210);
    readonly Vector2 UpBtnOffset = new(0, 29);
    readonly Vector2 DownBtnOffset = new(0, 125);
    readonly Vector2 LeftBtnOffset = new(0, 46);
    readonly Vector2 RightBtnOffset = new(106, 46);
    readonly Vector2 CustomNumberOffset = new(50, 80);
    readonly Vector2 AINumberOffset = new(90, 58);
    readonly Rectangle readyPos = new(1020, 600, 186, 56);
    readonly Rectangle backButton = new(0, 0, 128, 48);
    static readonly Point settingsButtonSize = new(36);
    Rectangle settingsButtonPos = new(new(1216, 608), settingsButtonSize);
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
    Texture2D[] charTexts;
    Texture2D check;
    Texture2D cross;
    Texture2D ready;
    Texture2D cogwheel;
    string gamemodesDescription = "If you can read this, something went wrong...";
    Texture2D upArrow;
    Texture2D downArrow;
    Texture2D leftArrow;
    Texture2D rightArrow;
    Texture2D minText;
    Texture2D maxText;
    Texture2D aiText;
    readonly Color selectColor = new(255, 255, 255, 127);

    Character Character = Character.None;
    bool selfReady => Character != Character.None && isReady[(int)Character];

    private MouseListener _mouseListener;
    #region Draw and Update
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        Matrix transformMatrix = _camera.GetViewMatrix();
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied, transformMatrix: transformMatrix);
        if (!selfReady)
            game.SpriteBatch.Draw(cogwheel, settingsButtonPos.Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
        #region Selection view
        game.SpriteBatch.DrawString(largeUIFont, "Back", new(64-backSize.Width/2,24-backSize.Height/2), Color.White);

        for(int i = 0; i < charIcons.Length; i++)
        {
            game.SpriteBatch.Draw(charIcons[i], charPos[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            game.SpriteBatch.Draw(charTexts[i], textPos[i], null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            if (selected[i] != Guid.Empty)
                game.SpriteBatch.Draw(check, charPos[i].Location.ToVector2(), null, !isReady[i] ? selectColor : Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
        }

        game.SpriteBatch.Draw(ready, readyPos, Color.White);

        if (game.DebugMode)
        {
            game.SpriteBatch.DrawRectangle(backButton, new(163, 87, 171));
            if(!selfReady)
                game.SpriteBatch.DrawRectangle(settingsButtonPos, new(139, 195, 119));
            if(game.Client.Channel.Gamemodes.CustomNight)
                game.SpriteBatch.DrawRectangle(CustomNightButtonPos, new(139, 195, 119));
        }
        #endregion
        #region Gamemode view
        //game.SpriteBatch.Draw(gamemodesTitle, new(64, 768), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
        game.SpriteBatch.DrawString(gamemodesFont, "Toggle Gamemodes", gamemodesPos, Color.White);
        game.SpriteBatch.DrawString(largeUIFont, gamemodesDescription, new(64, 1344), Color.White);
        if (game.Client.Channel.Gamemodes.CustomNight)
        {
            game.SpriteBatch.Draw(cogwheel, CustomNightButtonPos.Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }
        #endregion
        #region Custom Night
        game.SpriteBatch.DrawString(gamemodesFont, "Custom Night", customnightPos, Color.White);
        for (int i = 1; i < charIcons.Length; i++)
        {
            game.SpriteBatch.Draw(charTexts[i], customTextPos[i], null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            game.SpriteBatch.Draw(minText, customTextPos[i] + MinTextOffset, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            game.SpriteBatch.Draw(maxText, customTextPos[i] + MaxTextOffset, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            game.SpriteBatch.Draw(upArrow, customMinUpBtn[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            game.SpriteBatch.Draw(downArrow, customMinDownBtn[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            game.SpriteBatch.Draw(upArrow, customMaxUpBtn[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            game.SpriteBatch.Draw(downArrow, customMaxDownBtn[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            CustomNightFont.DrawNumber(game.SpriteBatch, game.Client.Channel.MoveTimes.Find(mt => (int)mt.Character == i)?.Min.ToString() ?? "0", customTextPos[i] + MinTextOffset + CustomNumberOffset, 0f);
            CustomNightFont.DrawNumber(game.SpriteBatch, game.Client.Channel.MoveTimes.Find(mt => (int)mt.Character == i)?.Max.ToString() ?? "0", customTextPos[i] + MaxTextOffset + CustomNumberOffset, 0f);
            if (game.Client.Channel.Gamemodes.AnimatronicAI)
            {
                game.SpriteBatch.Draw(aiText, customTextPos[i] + AITextOffset, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
                game.SpriteBatch.Draw(leftArrow, aiLeftBtn[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
                game.SpriteBatch.Draw(rightArrow, aiRightBtn[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
                CustomNightFont.DrawNumber(game.SpriteBatch, game.Client.Channel.AILevels.Find(mt => (int)mt.Character == i)?.Level.ToString() ?? "0", customTextPos[i] + AITextOffset + AINumberOffset, 0f);
            }
        }
        #endregion
        game.GumUI.Draw();
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        _mouseListener.Update(gameTime);
        game.GumUI.CanvasHeight = game.CurrentWindowSize.Y * 3;
        if (KeyboardExtended.GetState().WasKeyPressed(Keys.Escape))
        {
            if(_camera.Position.Y > 0)
            {
                if(_camera.Position.Y > 720)
                    ToggleCustomNight();
                else
                    ToggleSettings();
            }
            else
            {
                ScreenManager.ReplaceScreen(new MainMenu(game, true));
            }
        }
    }
    #endregion

    private void ToggleSettings()
    {
        if(_camera.Position.Y > 0)
        {
            _camera.Position = Vector2.Zero;
            game.GumUI.Renderer.Camera.Position = _camera.Position.ToSystemNumerics();
            settingsButtonPos.Location = new(1216, 608);
        }
        else
        {
            if (selfReady)
                return;
            _camera.Position = new Vector2(0, 720);
            game.GumUI.Renderer.Camera.Position = _camera.Position.ToSystemNumerics();
            settingsButtonPos.Location = returnSettingsPos.ToPoint();
        }
    }

    private void ToggleCustomNight()
    {
        if (_camera.Position.Y > 720)
        {
            _camera.Position = new Vector2(0, 720);
            game.GumUI.Renderer.Camera.Position = _camera.Position.ToSystemNumerics();
            CustomNightButtonPos.Location = new((int)(CustomNightBox.X + CustomNightBox.Width + 8), (int)CustomNightBox.Y);
        }
        else
        {
            if (!game.Client.Channel.Gamemodes.CustomNight)
                return;
            _camera.Position = new Vector2(0, 1440);
            game.GumUI.Renderer.Camera.Position = _camera.Position.ToSystemNumerics();
            CustomNightButtonPos.Location = customSettingsPos.ToPoint();
        }
    }

    List<string> ChatHistory = [];
    int HistoryIndex = -1;

    CheckBox AIBox;
    CheckBox CustomNightBox;
    Rectangle CustomNightButtonPos;
    ScrollViewer ScrollView;
    TextBox MessageBox;
    Vector2 gamemodesPos = new(64, 768);
    Vector2 customnightPos = new(64, 1488);
    Vector2 returnSettingsPos = new(576, 786);
    Vector2 customSettingsPos = new(576, 786);
    SizeF gamemodesTextSize;
    SizeF customnightTextSize;
    MonoGame.Extended.BitmapFonts.BitmapFont gamemodesFont;
    MonoGame.Extended.BitmapFonts.BitmapFont largeUIFont;
    MonoGame.Extended.BitmapFonts.BitmapFont smallUIFont;
    public override void Initialize()
    {
        _camera = new OrthographicCamera(GraphicsDevice);
        
        base.Initialize();
    }
    public override void LoadContent()
    {
        gamemodesFont = Content.Load<MonoGame.Extended.BitmapFonts.BitmapFont>("font/bbs72");
        largeUIFont = Content.Load<MonoGame.Extended.BitmapFonts.BitmapFont>("font/nunito20b");
        smallUIFont = Content.Load<MonoGame.Extended.BitmapFonts.BitmapFont>("font/nunito16");
        backSize = largeUIFont.MeasureString("Back");
        charIcons = [
            Content.Load<Texture2D>("select/guard"),
            Content.Load<Texture2D>("select/freddy"),
            Content.Load<Texture2D>("select/bonnie"),
            Content.Load<Texture2D>("select/chica"),
            Content.Load<Texture2D>("select/foxy"),
        ];
        charTexts = [
            Content.Load<Texture2D>("select/guard_text"),
            Content.Load<Texture2D>("select/freddy_text"),
            Content.Load<Texture2D>("select/bonnie_text"),
            Content.Load<Texture2D>("select/chica_text"),
            Content.Load<Texture2D>("select/foxy_text"),
        ];
        check = Content.Load<Texture2D>("select/checkmark");
        cross = Content.Load<Texture2D>("select/crossmark");
        ready = Content.Load<Texture2D>("select/ready");
        cogwheel = Content.Load<Texture2D>("select/settings");
        upArrow = Content.Load<Texture2D>("select/up");
        downArrow = Content.Load<Texture2D>("select/down");
        leftArrow = Content.Load<Texture2D>("select/left");
        rightArrow = Content.Load<Texture2D>("select/right");
        minText = Content.Load<Texture2D>("select/min");
        maxText = Content.Load<Texture2D>("select/max");
        aiText = Content.Load<Texture2D>("select/ai_text");
        CustomNightFont = new NumberRenderer(game, "custom");

        gamemodesTextSize = gamemodesFont.MeasureString("Toggle Gamemodes");
        customnightTextSize = gamemodesFont.MeasureString("Custom Night");
        returnSettingsPos = new Vector2(gamemodesPos.X + gamemodesTextSize.Width + 16, gamemodesPos.Y + gamemodesTextSize.Height/2 - cogwheel.Height/4);
        customSettingsPos = new Vector2(customnightPos.X + customnightTextSize.Width + 16, customnightPos.Y + customnightTextSize.Height / 2 - cogwheel.Height / 4);

        for (int i = 1; i < charIcons.Length; i++)
        {
            customMinUpBtn[i] = new Rectangle((int)(customTextPos[i].X + MinTextOffset.X + UpBtnOffset.X), (int)(customTextPos[i].Y + MinTextOffset.Y + UpBtnOffset.Y), upArrow.Width, upArrow.Height);
            customMinDownBtn[i] = new Rectangle((int)(customTextPos[i].X + MinTextOffset.X + DownBtnOffset.X), (int)(customTextPos[i].Y + MinTextOffset.Y + DownBtnOffset.Y), downArrow.Width, downArrow.Height);
            customMaxUpBtn[i] = new Rectangle((int)(customTextPos[i].X + MaxTextOffset.X + UpBtnOffset.X), (int)(customTextPos[i].Y + MaxTextOffset.Y + UpBtnOffset.Y), upArrow.Width, upArrow.Height);
            customMaxDownBtn[i] = new Rectangle((int)(customTextPos[i].X + MaxTextOffset.X + DownBtnOffset.X), (int)(customTextPos[i].Y + MaxTextOffset.Y + DownBtnOffset.Y), downArrow.Width, downArrow.Height);
            aiLeftBtn[i] = new Rectangle((int)(customTextPos[i].X + AITextOffset.X + LeftBtnOffset.X), (int)(customTextPos[i].Y + AITextOffset.Y + LeftBtnOffset.Y), leftArrow.Width, leftArrow.Height);
            aiRightBtn[i] = new Rectangle((int)(customTextPos[i].X + AITextOffset.X + RightBtnOffset.X), (int)(customTextPos[i].Y + AITextOffset.Y + RightBtnOffset.Y), rightArrow.Width, rightArrow.Height);
        }


        var settings = new MouseListenerSettings();
        settings.DoubleClickMilliseconds = int.MinValue;
        settings.DragThreshold = int.MaxValue;
        _mouseListener = new MouseListener(settings);
        _mouseListener.MouseClicked += MouseClicked;
        _mouseListener.MouseMoved += MouseMoved;

        game.Client.ChannelUserJoined += Client_ChannelUserJoined;
        game.Client.ChannelUserLeft += Client_ChannelUserLeft;
        game.Client.LeftChannel += Client_Disconnected;
        game.Client.UserReady += Client_UserReady;
        game.Client.CharacterSelected += Client_CharacterSelected;
        game.Client.GameCountdown += Client_GameCountdown;
        game.Client.GameStart += Client_GameStart;
        game.Client.ChatMessageReceived += Client_ChatMessageReceived;
        game.Client.Disconnected += Client_Disconnected;
        game.Client.GamemodeChanged += Client_GamemodeChanged;
        game.Client.OwnerChange += Client_OwnerChange;

        AIBox = new CheckBox
        {
            X = 64,
            Y = 896,
            Width = 272,
            Height = 32,
            Text = "Animtaronic AI",
            IsEnabled = game.Client.Channel.Owner == game.Client.Self.Id,
            IsChecked = game.Client.Channel.Gamemodes.AnimatronicAI
        };
        UpdateCheckBox(AIBox);
        AIBox.AddToRoot();
        CustomNightBox = new CheckBox
        {
            X = 64,
            Y = 960,
            Width = 272,
            Height = 32,
            Text = "Custom Night",
            IsEnabled = game.Client.Channel.Owner == game.Client.Self.Id,
            IsChecked = game.Client.Channel.Gamemodes.CustomNight
        };
        UpdateCheckBox(CustomNightBox);
        CustomNightBox.AddToRoot();

        CustomNightButtonPos = new(new((int)(CustomNightBox.X + CustomNightBox.Width + 8), (int)CustomNightBox.Y), settingsButtonSize);

        ScrollView = new ScrollViewer
        {
            X = 96,
            Y = 320,
            Width = 672,
            Height = 286,
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
            IsEnabled = false,
        };
        var scrollviewvisual = (ScrollViewerVisual)ScrollView.Visual;
        //scrollviewvisual.BackgroundColor = Color.Transparent;
        scrollviewvisual.Background.ApplyState(Styling.ActiveStyle.NineSlice.OutlinedHeavy);

        MessageBox = new TextBox
        {
            X = 96,
            Y = 606,
            Width = 672,
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
            if (args.Key == Keys.Up)
            {
                HistoryIndex = HistoryIndex == -1 ? ChatHistory.Count - 1 : HistoryIndex > 0 ? HistoryIndex - 1 : 0;
                if (HistoryIndex > -1)
                {
                    MessageBox.Text = ChatHistory[HistoryIndex];
                    MessageBox.CaretIndex = MessageBox.Text.Length;
                }
                return;
            }
            if (args.Key == Keys.Down)
            {
                if (HistoryIndex > -1 && HistoryIndex < ChatHistory.Count - 1)
                {
                    HistoryIndex++;
                    MessageBox.Text = ChatHistory[HistoryIndex];
                    MessageBox.CaretIndex = MessageBox.Text.Length;
                }
                else
                {
                    HistoryIndex = -1;
                    MessageBox.Text = "";
                }
                return;
            }
            HistoryIndex = -1;
            if (MessageBox.Text?.Length > 0 && args.Key == Keys.Enter)
            {
                ChatHistory.Add(MessageBox.Text);
                game.Client.SendMessage(MessageBox.Text);
                MessageBox.Text = "";
            }
        };

        MessageBox.TextChanged += (_, args) =>
        {
            if (MessageBox.Text.Contains('\n'))
                MessageBox.Text = MessageBox.Text.Replace('\n', ' ');
        };

        MessageBox.PreviewTextInput += (sender, args) =>
        {
            if (args.Text.Length >= 256)
            {
                args.Handled = true;
            }
        };

        if (lobby)
            ChatMessage("Welcome to Fazbear Multiplayer!\nFor a list of commands type in /help");
        else
        {
            game.Audio.StopAll();
            ChatMessage("> Game aborted.");
        }

        ScrollView.AddToRoot();
        MessageBox.AddToRoot();

        base.LoadContent();
    }

    private void Client_OwnerChange()
    {
        checkBoxes.ForEach(box =>
        {
            box.IsEnabled = game.Client.Channel.Owner == game.Client.Self.Id;
        });
    }

    private void Client_GamemodeChanged(string gamemode, bool enabled)
    {
        switch(gamemode.ToLower())
        {
            case "animatronicai":
                if (AIBox.IsChecked != enabled)
                    AIBox.IsChecked = enabled;
                game.Client.Channel.Gamemodes.AnimatronicAI = enabled;
                ChatMessage("> " + AIBox.Text + " is now " + (enabled ? "enabled" : "disabled"));
                break;
            case "customnight":
                if (CustomNightBox.IsChecked != enabled)
                    CustomNightBox.IsChecked = enabled;
                game.Client.Channel.Gamemodes.CustomNight = enabled;
                ChatMessage("> " + CustomNightBox.Text + " is now " + (enabled ? "enabled" : "disabled"));
                if (!enabled && _camera.Position.Y > 720)
                    ToggleCustomNight();
                break;
        }
    }

    List<CheckBox> checkBoxes = [];

    private void UpdateCheckBox(CheckBox checkBox)
    {
        var visual = (CheckBoxVisual)checkBox.Visual;
        visual.BackgroundColor = Color.White;
        visual.CheckColor = Color.Transparent;
        visual.CheckBoxBackground.Texture = Content.Load<Texture2D>("ui/checkbox_unchecked");
        visual.CheckBoxBackground.TextureAddress = TextureAddress.EntireTexture;
        visual.CheckBoxBackground.Width = 64;
        visual.CheckBoxBackground.TextureWidth = 64;
        visual.CheckBoxBackground.Height = 32;
        visual.CheckBoxBackground.TextureHeight = 32;
        visual.TextInstance.CustomFontFile = "font/vui20.fnt";
        visual.TextInstance.UseCustomFont = true;
        visual.TextInstance.X = visual.CheckBoxBackground.Width;
        checkBox.Checked += (_, _) => visual.CheckBoxBackground.Texture = Content.Load<Texture2D>("ui/checkbox_checked");
        checkBox.Unchecked += (_, _) => visual.CheckBoxBackground.Texture = Content.Load<Texture2D>("ui/checkbox_unchecked");
        visual.ClickPreview += (_, @event) =>
        {
            @event.Handled = true;
            switch (checkBox)
            {
                case var _ when checkBox == AIBox:
                    game.Client.SetGamemode("AnimatronicAI", !checkBox.IsChecked ?? false);
                    break;
                case var _ when checkBox == CustomNightBox:
                    game.Client.SetGamemode("CustomNight", !checkBox.IsChecked ?? false);
                    break;
            }
        };
        checkBoxes.Add(checkBox);
    }

    private void MouseMoved(object sender, MouseEventArgs e)
    {
        CheckBox hovered = null;
        foreach (var box in checkBoxes)
        {
            var visual = (CheckBoxVisual)box.Visual;
            bool cursor = visual.HasCursorOver(game.GumUI.Cursor);
            if (game.Client.Channel.Owner == game.Client.Self.Id)
                visual.CheckBoxBackground.Texture = (!box.IsChecked ?? false) ? (cursor ? Content.Load<Texture2D>("ui/checkbox_unchecked_hover") : Content.Load<Texture2D>("ui/checkbox_unchecked")) : (cursor ? Content.Load<Texture2D>("ui/checkbox_checked_hover") : Content.Load<Texture2D>("ui/checkbox_checked"));
            if(cursor)
            {
                hovered = box;
            }
        }
        gamemodesDescription = hovered switch
        {
            var _ when hovered == AIBox => "Enables AI for unselected animatronics.",
            var _ when hovered == CustomNightBox => "Allows you to set the move times and AI level of each animatronic.",
            _ => "[ Hover over the gamemode, to view it's description ]",
        };
    }

    private void Client_Disconnected(string obj)
    {
        ScreenManager.ReplaceScreen(new MainMenu(game, true));
    }

    private void Client_ChatMessageReceived(Client user, string text)
    {
        if(user != null && user.Id != Guid.Empty)
            ChatMessage($"{user.Nick}: {text}");
        else
            ChatMessage($"> {text}");
    }

    private void ChatMessage(string message)
    {
        foreach(var line in smallUIFont.WrapString(message, ScrollView.ActualWidth))
        {
            var label = new Label
            {
                Text = line
            };
            var visual = (LabelVisual)label.Visual;
            visual.CustomFontFile = "font/ui16.fnt";
            visual.UseCustomFont = true;

            ScrollView.AddChild(label);

            // Scroll to the bottom:
            ScrollView.VerticalScrollBarValue = ScrollView.VerticalScrollBarMaximum;
        }
    }

    private void Client_ChannelUserLeft(Client user, string reason = "")
    {
        ChatMessage($"> {user.Nick} left." + (reason != "" ? " (" + reason + ")" : ""));
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
        if(seconds == -1)
        {
            ChatMessage($"> Game start aborted.");
            return;
        }
        var append = seconds > 1 ? "s" : "";
        ChatMessage($"> The game begins in {seconds} second{append}");
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
        if (e.Button != MouseButton.Left || !game.IsActive)
        {
            return;
        }
        Vector2 position = _camera.ScreenToWorld(game.MouseState.Position);
        if(CustomNightButtonPos.Contains(position))
        {
            ToggleCustomNight();
            return;
        }
        if (settingsButtonPos.Contains(position))
        {
            ToggleSettings();
            return;
        }
        if(_camera.Position.Y > 720)
        {
            for (int i = 1; i < charIcons.Length; i++)
            {
                if(customMinUpBtn[i].Contains(position))
                {
                    game.Client.SetMoveTime((Character)i, game.Client.Channel.MoveTimes.Find(mt=>(int)mt.Character==i).Min+1, "min");
                    return;
                }
                if (customMinDownBtn[i].Contains(position))
                {
                    game.Client.SetMoveTime((Character)i, Math.Max(0, game.Client.Channel.MoveTimes.Find(mt => (int)mt.Character == i).Min - 1), "min");
                    return;
                }
                if (customMaxUpBtn[i].Contains(position))
                {
                    game.Client.SetMoveTime((Character)i, game.Client.Channel.MoveTimes.Find(mt => (int)mt.Character == i).Max + 1, "max");
                    return;
                }
                if (customMaxDownBtn[i].Contains(position))
                {
                    game.Client.SetMoveTime((Character)i, Math.Max(0, game.Client.Channel.MoveTimes.Find(mt => (int)mt.Character == i).Max - 1), "max");
                    return;
                }
                if (aiLeftBtn[i].Contains(position))
                {
                    game.Client.SetAILevel((Character)i, game.Client.Channel.AILevels.Find(al => (int)al.Character == i).Level - 1);
                    return;
                }
                if (aiRightBtn[i].Contains(position))
                {
                    game.Client.SetAILevel((Character)i, Math.Max(0, game.Client.Channel.AILevels.Find(al => (int)al.Character == i).Level + 1));
                    return;
                }
            }
            return;
        }
        if (backButton.Contains(position))
        {
            ScreenManager.ReplaceScreen(new MainMenu(game, true));
            return;
        }
        if (readyPos.Contains(position) && Character != Character.None)
        {
            game.Client.SetReady(!isReady[(int)Character]);
            return;
        }
        for (int i = 0; i < charPos.Length; i++)
        {
            if (charPos[i].Contains(position))
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
                break;
            }
        }
    }

    public override void UnloadContent()
    {
        game.GumUI.Renderer.Camera.Position = Vector2.Zero.ToSystemNumerics();
        game.GumUI.CanvasHeight = game.CurrentWindowSize.Y;
        _mouseListener.MouseClicked -= MouseClicked;
        _mouseListener.MouseMoved -= MouseMoved;
        game.Client.ChannelUserJoined -= Client_ChannelUserJoined;
        game.Client.ChannelUserLeft -= Client_ChannelUserLeft;
        game.Client.LeftChannel -= Client_Disconnected;
        game.Client.UserReady -= Client_UserReady;
        game.Client.CharacterSelected -= Client_CharacterSelected;
        game.Client.GameCountdown -= Client_GameCountdown;
        game.Client.GameStart -= Client_GameStart;
        game.Client.ChatMessageReceived -= Client_ChatMessageReceived;
        game.Client.Disconnected -= Client_Disconnected;
        game.Client.GamemodeChanged -= Client_GamemodeChanged;
        game.Client.OwnerChange -= Client_OwnerChange;

        ScrollView.RemoveFromRoot();
        MessageBox.RemoveFromRoot();
        base.UnloadContent();
    }
}
