
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class ShowStage : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public ShowStage(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/1a/none"),
            content.Load<Texture2D>("camera/view/1a/fr"),
            null,
            content.Load<Texture2D>("camera/view/1a/bofr"),
            null,
            content.Load<Texture2D>("camera/view/1a/chfr"),
            null,
            content.Load<Texture2D>("camera/view/1a/all"),
        ];
    }
}
