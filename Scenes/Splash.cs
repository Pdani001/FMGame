using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using ReFMGame.GameHelper;
using System;
using System.Collections.Generic;

namespace ReFMGame.Scenes;
public class Splash(GameExtended game) : GameScreen(game)
{
    private readonly List<Texture2D> textures = [];
    private readonly List<Rectangle> positions = [];
    private readonly Rectangle GameView = new(Point.Zero, game.WindowSize);
    private const float MaxWidth = 1200f;
    private int Index = 0;
    private bool Forward = false;
    private int Alpha = 255;
    private double update = 0;
    public override void Draw(GameTime gameTime)
    {
        if (textures.Count == 0)
            return;
        if (textures.Count <= Index)
            return;
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
        Color color = Color.White;
        color.A = (byte)Alpha;
        game.SpriteBatch.Draw(textures[Index], positions[Index], color);
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        if (textures.Count <= Index || (Mouse.GetState().LeftButton == ButtonState.Pressed && game.IsActive && GameView.Contains(game.MouseState.Position)) || (Mouse.GetState().RightButton == ButtonState.Pressed && game.IsActive && GameView.Contains(game.MouseState.Position))) {
            ScreenManager.ReplaceScreen(new MainMenu(game, rare: rare));
            return;
        }
#if DEBUG
        if (KeyboardExtended.GetState().WasKeyPressed(Keys.R))
        {
            rare = true;
        }
#endif
        double time = gameTime.TotalGameTime.TotalMilliseconds;
        if (update == 0)
            update = time + 2500;
        if (time >= update)
        {
            if (!Forward)
            {
                if (Alpha > 0)
                {
                    Alpha -= 3;
                    update = time + 5;
                }
                else
                {
                    Index++;
                    Forward = true;
                }
            }
            if (Forward)
            {
                if (Alpha < 255)
                {
                    Alpha += 3;
                    update = time + 5;
                }
                else
                {
                    Forward = false;
                    update = time + 2500;
                }
            }
            if (Alpha < 0)
                Alpha = 0;
            if (Alpha > 255)
                Alpha = 255;
        }
    }
    private readonly Random rng = new(Guid.NewGuid().GetHashCode());
    private bool rare = false;
    public override void LoadContent()
    {
        rare = rng.Next(6) == 1;
        base.LoadContent();
        Alpha = 255;
        update = 0;
        Forward = false;
        Index = 0;
        Texture2D fmload = Content.Load<Texture2D>("fm-loading-small");
        Texture2D mglogo = Content.Load<Texture2D>("MonoGameLogo-small");
        textures.Add(fmload);
        textures.Add(mglogo);

        textures.ForEach(texture =>
        {
            float scale = MaxWidth / texture.Width;
            int w = texture.Width;
            int h = texture.Height;
            if (w > MaxWidth)
            {
                h = (int)(MaxWidth / w * h);
                w = (int)MaxWidth;
            }
            int x = (game.WindowSize.X - w) / 2;
            int y = (game.WindowSize.Y - h) / 2;
            positions.Add(new Rectangle(x, y, w, h));
        });
    }
}