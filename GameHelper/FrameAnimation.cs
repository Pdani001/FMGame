using Microsoft.Xna.Framework;
using System;

namespace ReFMGame.GameHelper;
public abstract class FrameAnimation
{
    public float Timer { get; private set; }
    public abstract int Threshold { get; }
    protected abstract Rectangle[] Frames { get; }
    public byte Index { get; private set; }

    public void Animate(GameTime gameTime)
    {
        if(Threshold < 0){
            return;
        }
        Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        if (Timer >= Threshold)
        {
            Index++;
            if (Index >= Frames.Length)
                Index = 0;
            Timer -= Threshold;
        }
    }

    public void Reset(byte targetIndex = 0)
    {
        Timer = 0;
        Index = targetIndex;
    }

    public Rectangle this[int index]
    {
        get => Frames[Math.Clamp(index, 0, Frames.Length - 1)];
    }
}