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

namespace ReFMGame.Scenes;
public class Settings(FMGame game, Menu menu) : GameScreen(game)
{
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

        game.SpriteBatch.DrawString(smallB, "Back", backPos, Color.White);
        game.SpriteBatch.DrawString(large, "Settings", new(64, 224), Color.White);

        game.SpriteBatch.Draw(menu.logo, new(68, 50), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
        if(menu.RareBGM)
            game.SpriteBatch.DrawString(menu.bmfont, "57", new(79, 190), Color.Yellow);
        if (game.DebugMode)
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
        menu.static_animation.Animate(gameTime);
        if (backButton.Contains(game.MouseState.Position))
        {
            if (MouseExtended.GetState().WasButtonPressed(MouseButton.Left))
                ScreenManager.CloseScreen();
        }
    }

    ScrollViewer ScrollView;
    Button FullscreenBtn;
    public override void LoadContent()
    {
        small = Content.Load<BitmapFont>("font/nunito20");
        smallB = Content.Load<BitmapFont>("font/nunito20b");
        large = Content.Load<BitmapFont>("font/nunito32b");
        SizeF backSize = smallB.MeasureString("Back");
        backPos = new(64 - backSize.Width / 2, 24 - backSize.Height / 2);

        ScrollView = new ScrollViewer
        {
            X = 88,
            Y = 266,
            Width = 1104,
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
            Text = game.FullScreenBind.ToString(),
            WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent,
            Width = 50,
        };
        var fullscreenBtnVisual = (ButtonVisual)FullscreenBtn.Visual;
        fullscreenBtnVisual.BackgroundColor = Styling.ActiveStyle.Colors.InputBackground;
        fullscreenBtnVisual.ForegroundColor = Styling.ActiveStyle.Colors.TextPrimary;
        fullscreenBtnVisual.TextInstance.CustomFontFile = "font/ui16.fnt";
        fullscreenBtnVisual.TextInstance.UseCustomFont = true;
        panel.AddChild(fullscreenLbl);
        panel.AddChild(FullscreenBtn);

        panel.AddChild(new Label
        {
            Text = "",
            Height = 1,
            HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute,
        });
        panel.AddChild(new Label
        {
            Text = "",
            Height = 1,
            HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute,
        });

        panel.AddChild(new Label
        {
            Text = "Server",
        });
        panel.AddChild(new ComboBox
        {
            WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent,
            Width = 50,
            Items = {
                "Main",
                "Test",
                //"Custom",
            },
            SelectedIndex = 0,
        });


        base.LoadContent();
    }

    public override void UnloadContent()
    {
        Mouse.SetCursor(MouseCursor.Arrow);
        ScrollView.RemoveFromRoot();
        base.UnloadContent();
    }
}
