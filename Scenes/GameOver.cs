using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System;
using ReFMGame.GameHelper;

namespace ReFMGame.Scenes;
public class GameOver(FMGame game, Texture2D screenshot) : GameScreen(game)
{
    private Texture2D bg;
    private Texture2D text;
    private float elapsed = 0;
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        if (elapsed <= 1.01f)
        {
            Color bg = Color.White;
            bg.A = (byte)((double)elapsed).Map(0, 1.01f, 255, 0);
            game.SpriteBatch.Draw(screenshot, Vector2.Zero, null, bg, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }
        game.SpriteBatch.Draw(bg, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
        game.SpriteBatch.Draw(text, new(1046, 660), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .1f);
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        float timer = (float)(elapsed - 1.01d);
        if(timer > 10f)
        {
            ScreenManager.ShowScreen(new MainMenu(game));
        }
    }

    public override void LoadContent()
    {
        game.Audio.StopAll();
        bg = Content.Load<Texture2D>("gameover/bg");
        text = Content.Load<Texture2D>("gameover/text");
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
