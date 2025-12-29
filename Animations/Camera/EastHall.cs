
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class EastHall : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public EastHall(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/4a/none"),
            content.Load<Texture2D>("camera/view/4a/freddy"),
            null,
            null,
            content.Load<Texture2D>("camera/view/4a/chica"),
            null,
            null,
            null,
        ];
    }
}
