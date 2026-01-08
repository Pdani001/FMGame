using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using System.Collections.Generic;

namespace ReFMGame.Scenes;
public class Splash(FMGame game) : GameScreen(game)
{
    private readonly List<Texture2D> textures = [];
    private readonly Rectangle GameView = new(new(0, 0), game.WindowSize);
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
        Texture2D text = textures[Index];
        int w = text.Width;
        int h = text.Height;
        if(w > MaxWidth)
        {
            h = (int)(MaxWidth / w * h);
            w = (int)MaxWidth;
        }
        int x = (int)((game.WindowSize.X/2f) - (w/2f));
        int y = (int)((game.WindowSize.Y/2f) - (h/2f));
        Color color = Color.White;
        color.A = (byte)Alpha;
        game.SpriteBatch.Draw(text, new Rectangle(x,y,w,h), color);
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        if (textures.Count <= Index || (Mouse.GetState().LeftButton == ButtonState.Pressed && game.IsActive && GameView.Contains(game.MouseState.Position)) || (Mouse.GetState().RightButton == ButtonState.Pressed && game.IsActive && GameView.Contains(game.MouseState.Position))) {
            ScreenManager.ReplaceScreen(new Menu(game));
            return;
        }
        double time = gameTime.TotalGameTime.TotalMilliseconds;
        if (update == 0)
            update = time + 2000;
        if (time >= update)
        {
            if (!Forward && Alpha > 0)
            {
                Alpha -= 3;
                update = time + 5;
            }
            if (!Forward && Alpha == 0)
            {
                Index++;
                Forward = true;
            }
            if (Forward && Alpha < 255)
            {
                Alpha += 3;
                update = time + 5;
            }
            if (Forward && Alpha == 255)
            {
                Forward = false;
                update = time + 2000;
            }
            if (Alpha < 0)
                Alpha = 0;
            if (Alpha > 255)
                Alpha = 255;
        }
    }

    public override void LoadContent()
    {
        base.LoadContent();
        Alpha = 255;
        update = 0;
        Forward = false;
        Index = 0;
        Texture2D fmload = Content.Load<Texture2D>("fm-loading-small");
        Texture2D mglogo = Content.Load<Texture2D>("MonoGameLogo-small");
        textures.Add(fmload);
        textures.Add(mglogo);
    }
}