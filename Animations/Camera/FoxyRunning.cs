
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Camera;
public class FoxyRunning : TextureAnimation
{
    public override int Threshold => 26;

    protected override Texture2D[] Frames { get; }

    public FoxyRunning(ContentManager content)
    {
        Loop = false;
        Frames =
        [
            content.Load<Texture2D>("camera/view/2a/foxy_01"),
            content.Load<Texture2D>("camera/view/2a/foxy_02"),
            content.Load<Texture2D>("camera/view/2a/foxy_03"),
            content.Load<Texture2D>("camera/view/2a/foxy_04"),
            content.Load<Texture2D>("camera/view/2a/foxy_05"),
            content.Load<Texture2D>("camera/view/2a/foxy_06"),
            content.Load<Texture2D>("camera/view/2a/foxy_07"),
            content.Load<Texture2D>("camera/view/2a/foxy_08"),
            content.Load<Texture2D>("camera/view/2a/foxy_09"),
            content.Load<Texture2D>("camera/view/2a/foxy_10"),
            content.Load<Texture2D>("camera/view/2a/foxy_11"),
            content.Load<Texture2D>("camera/view/2a/foxy_12"),
            content.Load<Texture2D>("camera/view/2a/foxy_13"),
            content.Load<Texture2D>("camera/view/2a/foxy_14"),
            content.Load<Texture2D>("camera/view/2a/foxy_15"),
            content.Load<Texture2D>("camera/view/2a/foxy_16"),
            content.Load<Texture2D>("camera/view/2a/foxy_17"),
            content.Load<Texture2D>("camera/view/2a/foxy_18"),
            content.Load<Texture2D>("camera/view/2a/foxy_19"),
            content.Load<Texture2D>("camera/view/2a/foxy_20"),
            content.Load<Texture2D>("camera/view/2a/foxy_21"),
            content.Load<Texture2D>("camera/view/2a/foxy_22"),
            content.Load<Texture2D>("camera/view/2a/foxy_23"),
            content.Load<Texture2D>("camera/view/2a/foxy_24"),
            content.Load<Texture2D>("camera/view/2a/foxy_25"),
            content.Load<Texture2D>("camera/view/2a/foxy_26"),
            content.Load<Texture2D>("camera/view/2a/foxy_27"),
            content.Load<Texture2D>("camera/view/2a/foxy_28"),
            content.Load<Texture2D>("camera/view/2a/foxy_29"),
            content.Load<Texture2D>("camera/view/2a/foxy_30"),
            content.Load<Texture2D>("camera/view/2a/foxy_31"),
            content.Load<Texture2D>("camera/view/2a/foxy_32"),
            content.Load<Texture2D>("camera/view/2a/foxy_33"),
        ];
    }
}
