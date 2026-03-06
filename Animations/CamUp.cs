using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class CamUp : TextureAnimation
{
	public override int Threshold => 23;
	protected override Texture2D[] Frames { get; }

	public CamUp(ContentManager content)
	{
        Loop = false;
		Frames =
        [
            content.Load<Texture2D>("camera/anim/up_01"),
            content.Load<Texture2D>("camera/anim/up_02"),
            content.Load<Texture2D>("camera/anim/up_03"),
            content.Load<Texture2D>("camera/anim/up_04"),
            content.Load<Texture2D>("camera/anim/up_05"),
            content.Load<Texture2D>("camera/anim/up_06"),
            content.Load<Texture2D>("camera/anim/up_07"),
            content.Load<Texture2D>("camera/anim/up_08"),
            content.Load<Texture2D>("camera/anim/up_09"),
            content.Load<Texture2D>("camera/anim/up_10"),
            content.Load<Texture2D>("camera/anim/up_11"),
        ];
	}

}
