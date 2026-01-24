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
using ReFMGame.GameHelper;
using System;
using System.Diagnostics;
using System.Linq;

namespace ReFMGame.Scenes;
public class SettingsMenu(FMGame game, MainMenu menu) : GameScreen(game)
{
    private KeyboardListener _keyboardListener;
    readonly Rectangle backButton = new(0, 0, 128, 48);
    Vector2 backPos;
    BitmapFont small;
    BitmapFont smallB;
    BitmapFont large;
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        Color bgcolor = Color.White;
        bgcolor.A = (byte)(255 - menu.BGOpacity);
        game.SpriteBatch.Draw(menu.bg_texture, Vector2.Zero, null, bgcolor, 0, Vector2.Zero , 1, SpriteEffects.None, 0);

        if(!game.UpdateKeyBind)
            game.SpriteBatch.DrawString(smallB, "Back", backPos, Color.White);
        game.SpriteBatch.DrawString(large, "Settings", new(64, 224), Color.White);

        game.SpriteBatch.Draw(menu.logo, new(68, 50), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
        if(menu.RareBGM)
            game.SpriteBatch.DrawString(menu.bmfont, "57", new(79, 190), Color.Yellow);
        if (game.DebugMode && !game.UpdateKeyBind)
        {
            game.SpriteBatch.DrawRectangle(backButton, new(163, 87, 171));
        }
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
        _keyboardListener.Update(gameTime);
        if (!game.UpdateKeyBind && backButton.Contains(game.MouseState.Position))
        {
            if (MouseExtended.GetState().WasButtonPressed(MouseButton.Left))
                ScreenManager.CloseScreen();
        }
    }

    ComboBox ServerSelect;
    TextBox CustomServer;
    ScrollViewer ScrollView;
    Button FullscreenBtn;
    Button ScreenshotBtn;
    Label VolumeLabel;
    Slider VolumeSlider;
    BindKey? update = null;
    public override void LoadContent()
    {
        _keyboardListener = new KeyboardListener(new KeyboardListenerSettings { RepeatPress = false });
        small = Content.Load<BitmapFont>("font/nunito20");
        smallB = Content.Load<BitmapFont>("font/nunito20b");
        large = Content.Load<BitmapFont>("font/nunito32b");
        SizeF backSize = smallB.MeasureString("Back");
        backPos = new(64 - backSize.Width / 2, 24 - backSize.Height / 2);

        _keyboardListener.KeyPressed += KeyPressed;
        _keyboardListener.KeyReleased += KeyReleased;

        ScrollView = new ScrollViewer
        {
            X = 88,
            Y = 266,
            Width = 552,
            Height = 410,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
        };
        var scrollviewvisual = (ScrollViewerVisual)ScrollView.Visual;
        scrollviewvisual.BackgroundColor = Color.Transparent;
        ScrollBarVisual scrollBar = scrollviewvisual.VerticalScrollBarInstance;
        scrollBar.UpButtonInstance.BackgroundColor = Styling.ActiveStyle.Colors.InputBackground;
        scrollBar.UpButtonIcon.Color = Styling.ActiveStyle.Colors.TextPrimary;
        scrollBar.DownButtonInstance.BackgroundColor = Styling.ActiveStyle.Colors.InputBackground;
        scrollBar.DownButtonIcon.Color = Styling.ActiveStyle.Colors.TextPrimary;
        scrollBar.ThumbInstance.BackgroundColor = Styling.ActiveStyle.Colors.InputBackground;

        ScrollView.AddToRoot();

        var panel = new StackPanel();
        ScrollView.AddChild(panel);
        panel.Height = 100;
        panel.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
        panel.Width = 100;
        panel.WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent;
        var panelVisual = panel.Visual;
        panelVisual.ChildrenLayout = Gum.Managers.ChildrenLayout.AutoGridHorizontal;
        panelVisual.AutoGridHorizontalCells = 2;
        panelVisual.AutoGridVerticalCells = 1;
        panelVisual.StackSpacing = 20;


        var fullscreenLbl = new Label
        {
            Text = "Fullscreen Button",
        };
        var fullscreenLblVisual = (LabelVisual)fullscreenLbl.Visual;
        fullscreenLblVisual.CustomFontFile = "font/ui20.fnt";
        fullscreenLblVisual.UseCustomFont = true;
        FullscreenBtn = new Button
        {
            Text = game.settings.KeyBinds[BindKey.Fullscreen].ToString(),
            WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent,
            Width = 100,
        };
        FullscreenBtn.Click += FullscreenBtn_Click;
        var fullscreenBtnVisual = (ButtonVisual)FullscreenBtn.Visual;
        fullscreenBtnVisual.BackgroundColor = Styling.ActiveStyle.Colors.InputBackground;
        fullscreenBtnVisual.ForegroundColor = Styling.ActiveStyle.Colors.TextPrimary;
        fullscreenBtnVisual.TextInstance.CustomFontFile = "font/ui16.fnt";
        fullscreenBtnVisual.TextInstance.UseCustomFont = true;
        panel.AddChild(fullscreenLbl);
        panel.AddChild(FullscreenBtn);

        AddSpacing(panel);

        var screenshotLbl = new Label
        {
            Text = "Screenshot Button",
        };
        var screenshotLblVisual = (LabelVisual)screenshotLbl.Visual;
        screenshotLblVisual.CustomFontFile = "font/ui20.fnt";
        screenshotLblVisual.UseCustomFont = true;
        ScreenshotBtn = new Button
        {
            Text = game.settings.KeyBinds[BindKey.Screenshot].ToString(),
            WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent,
            Width = 100,
        };
        ScreenshotBtn.Click += ScreenshotBtn_Click;
        var screenshotBtnVisual = (ButtonVisual)ScreenshotBtn.Visual;
        screenshotBtnVisual.BackgroundColor = Styling.ActiveStyle.Colors.InputBackground;
        screenshotBtnVisual.ForegroundColor = Styling.ActiveStyle.Colors.TextPrimary;
        screenshotBtnVisual.TextInstance.CustomFontFile = "font/ui16.fnt";
        screenshotBtnVisual.TextInstance.UseCustomFont = true;
        panel.AddChild(screenshotLbl);
        panel.AddChild(ScreenshotBtn);


        AddSpacing(panel);

        VolumeLabel = new Label
        {
            Text = $"Volume ({game.Audio.Volume * 100}%)",
        };
        var volumeLblVisual = (LabelVisual)VolumeLabel.Visual;
        volumeLblVisual.CustomFontFile = "font/ui20.fnt";
        volumeLblVisual.UseCustomFont = true;
        VolumeSlider = new Slider
        {
            Value = game.Audio.Volume * 100,
            Minimum = 0,
            Maximum = 100,
            TicksFrequency = 1,
            IsSnapToTickEnabled = true,
            WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent,
            Width = 100,
        };
        VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
        var volumeSliderVisual = (SliderVisual)VolumeSlider.Visual;
        volumeSliderVisual.TrackBackgroundColor = Color.Lerp(Color.Red, Color.Green, game.Audio.Volume);
        volumeSliderVisual.ThumbInstance.BackgroundColor = Styling.ActiveStyle.Colors.InputBackground;
        panel.AddChild(VolumeLabel);
        panel.AddChild(VolumeSlider);

        AddSpacing(panel);

        var serverLbl = new Label
        {
            Text = "Server",
        };
        var serverLblVisual = (LabelVisual)serverLbl.Visual;
        serverLblVisual.CustomFontFile = "font/ui20.fnt";
        serverLblVisual.UseCustomFont = true;

        panel.AddChild(serverLbl);

        ServerSelect = new ComboBox
        {
            WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent,
            Width = 100,
            Items = {
                "Main",
                "Test",
                "Custom",
            },
            SelectedIndex = game.ServerIndex,
        };
        ServerSelect.SelectionChanged += ServerSelect_SelectionChanged;
        panel.AddChild(ServerSelect);
        AddSpacing(panel, false);
        CustomServer = new TextBox
        {
            Placeholder = "IP:port",
            WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent,
            Width = 100,
            IsVisible = game.ServerIndex == 2,
            Text = game.CustomAddress,
        };
        CustomServer.TextChanged += CustomServer_TextChanged;
        panel.AddChild(CustomServer);


        base.LoadContent();
    }

    private void VolumeSlider_ValueChanged(object sender, EventArgs e)
    {
        VolumeLabel.Text = $"Volume ({VolumeSlider.Value}%)";
        game.Audio.Volume = (float)(VolumeSlider.Value / 100);
        game.settings.Volume = game.Audio.Volume;
        var volumeSliderVisual = (SliderVisual)VolumeSlider.Visual;
        volumeSliderVisual.TrackBackgroundColor = Color.Lerp(Color.Red, Color.Green, game.Audio.Volume);
    }

    private void CustomServer_TextChanged(object sender, EventArgs e)
    {
        if (game.ServerIndex != 2)
            return;
        game.CustomAddress = CustomServer.Text;
    }

    private void ServerSelect_SelectionChanged(object arg1, Gum.Wireframe.SelectionChangedEventArgs arg2)
    {
        Debug.WriteLine($"Server: #{ServerSelect.SelectedIndex}");
        game.ServerIndex = ServerSelect.SelectedIndex;
        CustomServer.IsVisible = game.ServerIndex == 2;
        CustomServer.Text = game.CustomAddress;
    }

    private readonly Keys[] _ignoreKey = [
        Keys.LeftShift,
        Keys.RightShift,
        Keys.LeftControl,
        Keys.RightControl,
        Keys.LeftAlt,
        Keys.RightAlt,
        Keys.LeftWindows,
        Keys.RightWindows,
    ];

    private readonly Keys[] _ignoreChar = [
        Keys.Space,
        Keys.Tab,
        Keys.Enter,
        Keys.Back,
    ];

    private void RefreshText(KeyBind newbind)
    {
        Button btn = null;
        switch (update)
        {
            case BindKey.Fullscreen:
                btn = FullscreenBtn;
                break;
            case BindKey.Screenshot:
                btn = ScreenshotBtn;
                break;
                // todo: add game chat
        }
        if(btn != null)
        {
            btn.Text = string.IsNullOrEmpty(newbind.ToString()) ? "Press any key..." : newbind.ToString();
        }
    }

    private void KeyReleased(object sender, KeyboardEventArgs e)
    {
        if (!game.UpdateKeyBind)
            return;
        var keyboard = KeyboardExtended.GetState();
        var newbind = new KeyBind(keyboard.IsControlDown(), keyboard.IsShiftDown());
        RefreshText(newbind);
    }

    private void KeyPressed(object sender, KeyboardEventArgs e)
    {
        if (!game.UpdateKeyBind)
        {
            if(e.Key == Keys.Escape)
            {
                ScreenManager.CloseScreen();
            }
            return;
        }
        if(e.Key == Keys.Escape)
        {
            game.UpdateKeyBind = false;
            RefreshText(game.settings.KeyBinds[update.Value]);
            update = null;
            return;
        }
        var keyboard = KeyboardExtended.GetState();
        Keys key = Keys.None;
        char? Character = e.Character;
        if (!_ignoreKey.Contains(e.Key))
        {
            key = e.Key;
        }
        if (_ignoreChar.Contains(e.Key))
        {
            Character = null;
        }
        var newbind = new KeyBind(keyboard.IsControlDown(), keyboard.IsShiftDown(), key, Character);
        if (game.settings.KeyBinds.Any(x => x.Value.Equals(newbind) && x.Key != update))
            return;
        RefreshText(newbind);
        if(newbind.Char != null || newbind.Key != Keys.None)
        {
            game.UpdateKeyBind = false;
            game.settings.KeyBinds[update.Value] = newbind;
            // todo: save this value somewhere
            update = null;
        }
    }

    private void FullscreenBtn_Click(object sender, EventArgs e)
    {
        FullscreenBtn.IsFocused = false;
        StartUpdate(BindKey.Fullscreen);
    }

    private void ScreenshotBtn_Click(object sender, EventArgs e)
    {
        ScreenshotBtn.IsFocused = false;
        StartUpdate(BindKey.Screenshot);
    }

    private void StartUpdate(BindKey bindKey)
    {
        if (game.UpdateKeyBind)
            return;
        game.UpdateKeyBind = true;
        update = bindKey;
        RefreshText(new());
    }

    private void AddSpacing(StackPanel panel, bool doubleSpace = true)
    {
        panel.AddChild(new Label
        {
            Text = "",
            Height = 1,
            HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute,
        });
        if(doubleSpace)
            panel.AddChild(new Label
            {
                Text = "",
                Height = 1,
                HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute,
            });
    }

    public override void UnloadContent()
    {
        Mouse.SetCursor(MouseCursor.Arrow);
        ScrollView.RemoveFromRoot();
        _keyboardListener.KeyPressed -= KeyPressed;
        FullscreenBtn.Click -= FullscreenBtn_Click;
        ScreenshotBtn.Click -= ScreenshotBtn_Click;
        VolumeSlider.ValueChanged -= VolumeSlider_ValueChanged;
        CustomServer.TextChanged -= CustomServer_TextChanged;
        ServerSelect.SelectionChanged -= ServerSelect_SelectionChanged;
        base.UnloadContent();
    }
}
