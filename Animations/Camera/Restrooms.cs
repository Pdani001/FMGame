
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class Restrooms : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public Restrooms(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/7/none"),
            content.Load<Texture2D>("camera/view/7/fr"),
            null,
            null,
            content.Load<Texture2D>("camera/view/7/ch"),
            content.Load<Texture2D>("camera/view/7/chfr"),
            null,
            null,
        ];
    }
}
