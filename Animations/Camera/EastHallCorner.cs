
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class EastHallCorner : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public EastHallCorner(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/4b/none"),
            content.Load<Texture2D>("camera/view/4b/freddy"),
            null,
            null,
            content.Load<Texture2D>("camera/view/4b/chica"),
            null,
            null,
            null,
        ];
    }
}
