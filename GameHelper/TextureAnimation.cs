using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ReFMGame.GameHelper;
public abstract class TextureAnimation
{
    public float Timer { get; private set; }
    public abstract int Threshold { get; }
    protected abstract Texture2D[] Frames { get; }
    public bool Loop { get; protected set; } = false;
    public bool Running { get; private set; } = false;
    public byte Index { get; private set; }
    /**
     * <summary>Only raised if this animation does not Loop</summary>
     */
    public event EventHandler AnimationFinished;
    protected virtual void OnAnimationFinished()
    {
        Running = false;
        Index--;
        AnimationFinished?.Invoke(this, EventArgs.Empty);
    }

    /**
     * <summary>Raised for every frame of this animation</summary>
     */
    public event EventHandler AnimationRunning;
    protected virtual void OnAnimationRunning()
    {
        AnimationRunning?.Invoke(this, EventArgs.Empty);
    }

    public void Animate(GameTime gameTime)
    {
        if(Threshold < 0 || Index >= Frames.Length || (!Running && !Loop))
        {
            return;
        }
        Running = true;
        Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        if (Timer >= Threshold)
        {
            Index++;
            if (Index >= Frames.Length)
            {
                if (!Loop)
                {
                    OnAnimationFinished();
                    return;
                }
                Index = 0;
            }
            OnAnimationRunning();
            Timer -= Threshold;
        }
    }

    public void Reset()
    {
        Timer = 0;
        Index = 0;
        Running = true;
    }

    public void Stop()
    {
        Running = false;
    }

    public Texture2D this[int index]
    {
        get => Frames[Math.Clamp(index, 0, Frames.Length-1)];
    }
}