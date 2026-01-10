using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using ReFMGame.Animations;
using ReFMGame.Animations.Jumpscare;
using ReFMGame.GameHelper;
using System.Threading;
using System.Threading.Tasks;

namespace ReFMGame.Scenes;
public class FreddyScene(FMGame game) : GameScreen(game)
{
	private TextureAnimation freddy;
    private TextureAnimation blip;
    private TextureAnimation staticAnim;
    private SoundEffect scream;
    private SoundEffect staticSound;

    public override void Draw(GameTime gameTime)
	{
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin();
        if (freddy.Running)
        {
            game.SpriteBatch.Draw(freddy[freddy.Index], Vector2.Zero, null, Color.White);
        }
        else
        {
            if (blip.Running)
            {
                game.SpriteBatch.Draw(blip[blip.Index], Vector2.Zero, null, Color.White);
            }
            game.SpriteBatch.Draw(staticAnim[staticAnim.Index], Vector2.Zero, null, Color.White);
        }
        game.SpriteBatch.End();
    }

	public override void Update(GameTime gameTime)
	{
		freddy.Animate(gameTime);
		blip.Animate(gameTime);
		staticAnim.Animate(gameTime);
    }

    CancellationTokenSource source = new();

    public void PreLoad()
    {
        freddy = new JumpFreddyNopower(Content);
        if (!game.Audio.NoAudio)
        {
            scream = Content.Load<SoundEffect>("jumpscare/xscream");
            staticSound = Content.Load<SoundEffect>("static/static");
        }
        freddy.AnimationFinished += delegate
        {
            game.Audio.StopAll();
            game.Audio.Play(staticSound);
        };
        blip = new CamBlip(Content);
        staticAnim = new StaticAnim(Content);
    }

    public override void LoadContent()
	{
        base.LoadContent();
        PreLoad();
        freddy.Reset();
        game.Audio.Play(scream);
        Task.Delay(12000).WaitAsync(source.Token).ContinueWith(t => {
            if (source.IsCancellationRequested)
            {
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
