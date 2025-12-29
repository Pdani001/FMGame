
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class DiningArea : TextureAnimation
{
    public override int Threshold => -1;

    protected override Texture2D[] Frames { get; }

    public DiningArea(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/1b/none"),
            content.Load<Texture2D>("camera/view/1b/fr"),
            content.Load<Texture2D>("camera/view/1b/bo"),
            content.Load<Texture2D>("camera/view/1b/bofr"),
            content.Load<Texture2D>("camera/view/1b/ch"),
            content.Load<Texture2D>("camera/view/1b/chfr"),
            content.Load<Texture2D>("camera/view/1b/boch"),
            content.Load<Texture2D>("camera/view/1b/all"),
        ];
    }
}
