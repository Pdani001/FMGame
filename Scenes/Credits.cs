using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using ReFMGame.GameHelper;
using System;
using System.Collections.Generic;

namespace ReFMGame.Scenes;
public class Credits(FMGame game, Menu menu) : GameScreen(game)
{
    readonly Rectangle backButton = new(0, 0, 128, 48);
    Vector2 backPos;
    SizeF lineSize;
    List<(Rectangle, Action)> credits = [];
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
        game.SpriteBatch.DrawString(large, "Credits", new(64, 224), Color.White);
        game.SpriteBatch.DrawString(small,
@"Five Nights at Freddy's by:
    Scott Cawthon

Fan-game created by:
    Dániel ""Pdani"" Pécsi

Menu music created by:
    @ZombieLord343

Huge thanks to:
    Bonnie20402
    for reigniting my interest in the project
    and helping me test it
", new(64, 260), Color.White, new(64, 260, 448, 414));
        game.SpriteBatch.Draw(menu.logo, new(68, 50), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
        if(menu.RareBGM)
            game.SpriteBatch.DrawString(menu.bmfont, "57", new(79, 190), Color.Yellow);
        if (game.DebugMode)
        {
            game.SpriteBatch.DrawRectangle(backButton, new(163, 87, 171));
            foreach (var credit in credits)
            {
                game.SpriteBatch.DrawRectangle(credit.Item1, Color.Red);
            }
        }
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
        bool hit = false;
        if (game.IsActive)
        {
            foreach (var credit in credits)
            {
                if (credit.Item1.Contains(game.MouseState.Position))
                {
                    hit = true;
                    Mouse.SetCursor(MouseCursor.Hand);
                    if (MouseExtended.GetState().WasButtonPressed(MouseButton.Left))
                        credit.Item2();
                    break;
                }
            }
        }
        if (!hit)
        {
            Mouse.SetCursor(MouseCursor.Arrow);
        }
    }

    public override void LoadContent()
    {
        small = Content.Load<BitmapFont>("font/nunito20");
        smallB = Content.Load<BitmapFont>("font/nunito20b");
        large = Content.Load<BitmapFont>("font/nunito32b");
        SizeF backSize = smallB.MeasureString("Back");
        backPos = new(64 - backSize.Width / 2, 24 - backSize.Height / 2);
        lineSize = small.MeasureString("text");
        Rectangle homepage = new(64, 260 + ((int)lineSize.Height * 3) + ((int)lineSize.Height / 4), 448, (int)lineSize.Height * 2);
        credits.Add((homepage, () =>
        {
            MethodHelper.OpenUrl("https://github.com/Pdani001/FMGame");
        }
        ));
        base.LoadContent();
    }

    public override void UnloadContent()
    {
        Mouse.SetCursor(MouseCursor.Arrow);
        base.UnloadContent();
    }
}
