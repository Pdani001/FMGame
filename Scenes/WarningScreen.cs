using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGameGum;
using ReFMGame.GameHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReFMGame.Scenes;
public class WarningScreen(GameExtended game, LobbyMenu menu) : GameScreen(game)
{
    BitmapFont large;
    BitmapFont small;
    BitmapFont smallB;
    public StackPanel panel { get; private set; }
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        game.SpriteBatch.DrawString(large, "Caution", new(64, 249), Color.Red, .5f);
        game.SpriteBatch.DrawString(small, "During online play, you may be exposed to unmoderated chat messages\nor other types of user-generated content that may not be suitable for everyone.", new(64, 313), Color.White, .5f);
        game.SpriteBatch.FillRectangle(new RectangleF(60, 247, 832, 130), Color.Black, .4f);
        game.SpriteBatch.DrawRectangle(new RectangleF(58, 245, 836, 134), Color.Red, 2);
        if(panel != null)
            panel.IsVisible = true;
        if(menu.panel != null)
            menu.panel.IsVisible = false;
        game.GumUI.Draw();
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        
    }

    public override void Initialize()
    {
        panel = new StackPanel();
        panel.Visual.ChildrenLayout = Gum.Managers.ChildrenLayout.Regular;
        panel.AddToRoot();
        base.Initialize();
    }

    private Button proceed;
    private Button back;
    private CheckBox dontshow;
    public override void LoadContent()
    {
        large = Content.Load<BitmapFont>("font/nunito32b");
        small = Content.Load<BitmapFont>("font/nunito20");
        smallB = Content.Load<BitmapFont>("font/nunito20b");
        
        proceed = new Button()
        {
            Text = "Proceed",
            Width = 128,
            Height = 24,
            X = 64,
            Y = 418,
        };
        var prvisual = (ButtonVisual)proceed.Visual;
        prvisual.BackgroundColor = Color.Red;
        panel.AddChild(proceed);

        back = new Button()
        {
            Text = "Back",
            Width = 128,
            Height = 24,
            X = 760,
            Y = 418,
        };
        var bkvisual = (ButtonVisual)back.Visual;
        bkvisual.BackgroundColor = Color.Gray;
        panel.AddChild(back);

        dontshow = new CheckBox()
        {
            Text = "Don't show this again",
            X = 64,
            Y = 386,
            Width = 200,
        };
        var cbvisual = (CheckBoxVisual)dontshow.Visual;
        cbvisual.BackgroundColor = Color.Red;
        panel.AddChild(dontshow);

        back.Click += (s, e) =>
        {
            ScreenManager.CloseScreen();
            menu.PressBack();
        };

        proceed.Click += (s, e) =>
        {
            panel.IsVisible = false;
            game.WarningShown = true;
            game.settings.WarningDismissed = dontshow.IsChecked ?? false;
            ScreenManager.CloseScreen();
            menu.WarningDismissed();
        };

        base.LoadContent();
    }

    public override void UnloadContent()
    {
        panel.RemoveFromRoot();
        base.UnloadContent();
    }
}
