
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class WestHall : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public WestHall(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/2a/empty"),
            content.Load<Texture2D>("camera/view/2a/off"),
            content.Load<Texture2D>("camera/view/2a/bonnie"),
            content.Load<Texture2D>("camera/view/2a/off"),
            null,//foxy running, special render
            null,
            null,
            null,
        ];
    }
}
