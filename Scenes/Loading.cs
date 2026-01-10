using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using ReFMGame.Animations;
using ReFMGame.GameHelper;
using ReFMGame.Network;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ReFMGame.Scenes;
public class Loading(FMGame game, Character character = Character.Guard) : GameScreen(game)
{
	private Texture2D spinner;
	private FrameAnimation animation;
    private bool ready_wait = false;
    private BitmapFont font;
    private readonly string text = "Loading complete, waiting for other players...";
    private Vector2 textpos;
	public override void Draw(GameTime gameTime)
	{
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin();
        if (!ready_wait)
            game.SpriteBatch.Draw(spinner, new Vector2(1184, 32), animation[animation.Index], Color.White);
        else
            game.SpriteBatch.DrawString(font, text, textpos, Color.White);
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
        game.Client.GameAbort += Client_GameAbort;
        Task.Delay(10).ContinueWith(t =>
        {
            office.PreLoad(delegate
            {
                font = Content.Load<BitmapFont>("font/b_volter24");
                var size = font.MeasureString(text);
                textpos = new((game.WindowSize.X / 2) - (size.Width / 2), game.WindowSize.Y - size.Height - 5);
                Debug.WriteLine("Office preload complete");
                if (!source.IsCancellationRequested)
                    game.Client.SetReady(true);
                ready_wait = true;
            });
        });
	}

    private void Client_GameStart(CharacterPosition[] obj)
    {
        office.SetPositions(obj);
        ScreenManager.ReplaceScreen(office);
    }

    private void Client_GameAbort()
    {
        ScreenManager.ReplaceScreen(new Select(game, false));
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        game.Client.GameStart -= Client_GameStart;
        game.Client.GameAbort -= Client_GameAbort;
        source.Cancel();
    }
}
