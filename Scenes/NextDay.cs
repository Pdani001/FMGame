using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System;
using ReFMGame.GameHelper;

namespace ReFMGame.Scenes;
public class NextDay(FMGame game, Texture2D screenshot) : GameScreen(game)
{
    private Texture2D five;
    private Texture2D six;
    private Texture2D black;
    private Texture2D am;
    private SoundEffect chime;
    private SoundEffect crowd;
    private float elapsed = 0;
    private float pos = 0;
    private bool yay = false;
    private bool fade = false;
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        if (elapsed <= 1.01f)
        {
            Color bg = Color.White;
            bg.A = (byte)((double)elapsed).Map(0, 1.01f, 255, 0);
            game.SpriteBatch.Draw(screenshot, new(0, 0), null, bg, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }
        game.SpriteBatch.Draw(black, new(498, 169), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .1f);
        game.SpriteBatch.Draw(five, new(549, 298 - pos), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        game.SpriteBatch.Draw(six, new(553, 408 - pos), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        game.SpriteBatch.Draw(am, new(645, 296), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        game.SpriteBatch.Draw(black, new(499, 385), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .1f);
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        pos = (float)(elapsed - 1.01d).Map(0, 5.48f, 0, 112);
        if(!yay && pos == 112)
        {
            yay = true;
            game.Audio.Play(crowd);
        }
        if(yay && !fade && ((elapsed - 6.59f) >= 3.33f))
        {
            fade = true;
            FadeTransition fadeTransition = new FadeTransition(GraphicsDevice, Color.Black, 0.9f);
            ScreenManager.CloseScreen(fadeTransition);
            fadeTransition.Completed += delegate
            {
                ScreenManager.ShowScreen(new MainMenu(game));
            };
        }
    }

    public override void LoadContent()
    {
        game.Audio.StopAll();
        five = Content.Load<Texture2D>("nextday/5");
        six = Content.Load<Texture2D>("nextday/6");
        am = Content.Load<Texture2D>("nextday/am");
        black = Content.Load<Texture2D>("nextday/black");
        if (!game.Audio.NoAudio)
        {
            chime = Content.Load<SoundEffect>("nextday/chimes");
            crowd = Content.Load<SoundEffect>("nextday/children");
        }
        game.Audio.Play(chime);
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
