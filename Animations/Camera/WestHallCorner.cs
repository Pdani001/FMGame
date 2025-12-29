
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class WestHallCorner : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public WestHallCorner(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/2b/none"),
            null,
            content.Load<Texture2D>("camera/view/2b/bonnie"),
            null,
            null,
            null,
            null,
            null,
        ];
    }
}
