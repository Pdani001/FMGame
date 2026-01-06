using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using ReFMGame.Animations;
using ReFMGame.GameHelper;
using ReFMGame.Network;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ReFMGame.Scenes;
public class Loading(FMGame game, Character character = Character.Guard) : GameScreen(game)
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

    CancellationTokenSource source = new();
    readonly Office office = new Office(game, character);


    public override void LoadContent()
	{
        base.LoadContent();
        spinner = Content.Load<Texture2D>("fm-spinner");
		animation = new SpinnerAnim();
        game.Client.GameStart += Client_GameStart;
        Task.Delay(10).ContinueWith(t =>
        {
            office.PreLoad(delegate
            {
                Debug.WriteLine("Office preload complete");
                if(!source.IsCancellationRequested)
                    game.Client.SetReady(true);
            });
        });
	}

    private void Client_GameStart(CharacterPosition[] obj)
    {
        office.SetPositions(obj);
        ScreenManager.ReplaceScreen(office);
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        game.Client.GameStart -= Client_GameStart;
        source.Cancel();
    }
}
