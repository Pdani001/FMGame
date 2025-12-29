
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class Kitchen : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public Kitchen(ContentManager content)
    {
        Loop = false;
        // no cheating on my watch lmao
        var black = content.Load<Texture2D>("camera/view/6/black");
        Frames =
        [
            black,
            black,
            black,
            black,
            black,
            black,
            black,
            black,
        ];
    }
}
