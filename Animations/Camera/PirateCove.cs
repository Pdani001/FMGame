
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class PirateCove : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public PirateCove(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/1c/0"),
            content.Load<Texture2D>("camera/view/1c/1"),
            content.Load<Texture2D>("camera/view/1c/2"),
            content.Load<Texture2D>("camera/view/1c/3"),
        ];
    }
}
