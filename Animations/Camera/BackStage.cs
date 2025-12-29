
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class BackStage : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public BackStage(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/5/none"),
            null,
            content.Load<Texture2D>("camera/view/5/bonnie"),
            null,
            null,
            null,
            null,
            null,
        ];
    }
}
