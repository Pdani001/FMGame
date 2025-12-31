using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using ReFMGame.Animations;
using ReFMGame.Animations.Jumpscare;
using ReFMGame.GameHelper;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ReFMGame.Scenes;
public class StaticScene(FMGame game) : GameScreen(game)
{
    private TextureAnimation blip;
    private TextureAnimation staticAnim;
    private SoundEffect staticSound;

    public override void Draw(GameTime gameTime)
	{
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin();
        if (blip.Running)
        {
            game.SpriteBatch.Draw(blip[blip.Index], Vector2.Zero, null, Color.White);
        }
        game.SpriteBatch.Draw(staticAnim[staticAnim.Index], Vector2.Zero, null, Color.White);
        game.SpriteBatch.End();
    }

	public override void Update(GameTime gameTime)
	{
		blip.Animate(gameTime);
		staticAnim.Animate(gameTime);
    }

    CancellationTokenSource source = new();

    public override void LoadContent()
	{
        base.LoadContent();
        staticSound = Content.Load<SoundEffect>("static/static");
        blip = new CamBlip(Content);
        staticAnim = new StaticAnim(Content);
        game.Audio.StopAll();
        game.Audio.Play(staticSound);
        Task.Delay(10000).WaitAsync(source.Token).ContinueWith(t => {
            if (source.IsCancellationRequested)
            {
                Debug.WriteLine("GameOver screen task cancelled!");
                return;
            }
            ScreenManager.ReplaceScreen(new GameOver(game, game.GetScreenshot()));
        });
	}

    public override void UnloadContent()
    {
        base.UnloadContent();
        source.Cancel();
    }
}
