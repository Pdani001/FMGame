using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using ReFMGame.Animations;
using ReFMGame.GameHelper;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReFMGame.Scenes;
public class Loading(FMGame game) : GameScreen(game)
{
	private Texture2D spinner;
	private FrameAnimation animation;
	public override void Draw(GameTime gameTime)
	{
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin();
        game.SpriteBatch.Draw(spinner, new Vector2(1184,32), animation[animation.Index], Color.White);
        game.SpriteBatch.End();
    }

	public override void Update(GameTime gameTime)
	{
		animation.Animate(gameTime);
	}

	public override void LoadContent()
	{
        base.LoadContent();
        spinner = Content.Load<Texture2D>("fm-spinner");
		animation = new SpinnerAnim();
        Task.Delay(10).ContinueWith(t =>
        {
            var office = new Office(game);
            office.PreLoad(delegate
            {
                Debug.WriteLine("Office preload complete");
                Task.Delay(1000).ContinueWith(t =>
                {
                    ScreenManager.ReplaceScreen(office);
                });
            });
        });
	}
}
