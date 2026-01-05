using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using ReFMGame.Network;
using System;
using System.Diagnostics;

namespace ReFMGame.Scenes;
public class Select(FMGame game) : GameScreen(game)
{
    BitmapFont nunito;
    SizeF size;
    readonly Rectangle[] charPos =
    {
        new(1038, 64, 200, 200),
        new(42, 64, 200, 200),
        new(291, 64, 200, 200),
        new(540, 64, 200, 200),
        new(789, 64, 200, 200),
    };
    readonly Rectangle readyPos = new(1020, 600, 186, 56);
    readonly Rectangle backButton = new(0, 0, 128, 48);
    readonly bool[] isCrossed =
    {
        false,
        false,
        true,
        true,
        true
    };
    readonly bool[] isReady =
    {
        false,
        false,
        false,
        false,
        false
    };
    readonly Guid[] selected =
    [
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
    ];
    Texture2D[] charIcons;
    Texture2D check;
    Texture2D cross;
    Texture2D ready;
    SoundEffect error;
    readonly Color selectColor = new(255, 255, 255, 127);

    Character character = Character.None;

    private MouseListener _mouseListener;
    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.RenderTarget);
        GraphicsDevice.Clear(Color.Black);
        game.SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.NonPremultiplied);
        game.SpriteBatch.DrawString(nunito, "Back", new(64-size.Width/2,24-size.Height/2), Color.White);

        for(int i = 0; i < charIcons.Length; i++)
        {
            game.SpriteBatch.Draw(charIcons[i], charPos[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            if(isCrossed[i])
                game.SpriteBatch.Draw(cross, charPos[i].Location.ToVector2(), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
            else if (selected[i] != Guid.Empty)
                game.SpriteBatch.Draw(check, charPos[i].Location.ToVector2(), null, !isReady[i] ? selectColor : Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, .5f);
        }

        game.SpriteBatch.Draw(ready, readyPos, Color.White);

        if (game.DebugMode)
        {
            game.SpriteBatch.DrawRectangle(backButton, new(163, 87, 171));
        }
        game.SpriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        _mouseListener.Update(gameTime);
    }

    public override void LoadContent()
    {
        nunito = Content.Load<BitmapFont>("font/nunito20b");
        size = nunito.MeasureString("Back");
        charIcons = [
            Content.Load<Texture2D>("select/guard"),
            Content.Load<Texture2D>("select/freddy"),
            Content.Load<Texture2D>("select/bonnie"),
            Content.Load<Texture2D>("select/chica"),
            Content.Load<Texture2D>("select/foxy"),
        ];
        check = Content.Load<Texture2D>("select/checkmark");
        cross = Content.Load<Texture2D>("select/crossmark");
        ready = Content.Load<Texture2D>("select/ready");
        error = Content.Load<SoundEffect>("error");

        var settings = new MouseListenerSettings();
        settings.DoubleClickMilliseconds = int.MinValue;
        settings.DragThreshold = int.MaxValue;
        _mouseListener = new MouseListener(settings);
        _mouseListener.MouseClicked += MouseClicked;

        base.LoadContent();
    }

    private void MouseClicked(object sender, MouseEventArgs e)
    {
        Vector2 position = game.MouseState.Position;
        if (e.Button != MouseButton.Left)
        {
            return;
        }
        if (backButton.Contains(position))
        {
            ScreenManager.ReplaceScreen(new Menu(game));
            return;
        }
        if (readyPos.Contains(position) && character != Character.None)
        {
            isReady[(int)character] = !isReady[(int)character];
            return;
        }
        for (int i = 0; i < charPos.Length; i++)
        {
            if (charPos[i].Contains(position))
            {
                if (isCrossed[i])
                {
                    error.Play();
                }
                else
                {
                    if (character != Character.None && i != (int)character)
                        break;
                    if (selected[i] == Guid.Empty)
                    {
                        selected[i] = Guid.NewGuid();
                        character = (Character)i;
                        //selected[i] = game.Client.Self.Id;
                    }
                    else if (!isReady[i])
                    {
                        character = Character.None;
                        selected[i] = Guid.Empty;
                    }
                }
                break;
            }
        }
    }

    public override void UnloadContent()
    {

        base.UnloadContent();
    }
}
