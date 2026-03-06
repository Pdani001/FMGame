using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class CamDown : TextureAnimation
{
	public override int Threshold => 23;
	protected override Texture2D[] Frames { get; }

	public CamDown(ContentManager content)
	{
        Loop = false;
		Frames =
        [
            content.Load<Texture2D>("camera/anim/down_01"),
            content.Load<Texture2D>("camera/anim/down_02"),
            content.Load<Texture2D>("camera/anim/down_03"),
            content.Load<Texture2D>("camera/anim/down_04"),
            content.Load<Texture2D>("camera/anim/down_05"),
            content.Load<Texture2D>("camera/anim/down_06"),
            content.Load<Texture2D>("camera/anim/down_07"),
            content.Load<Texture2D>("camera/anim/down_08"),
            content.Load<Texture2D>("camera/anim/down_09"),
            content.Load<Texture2D>("camera/anim/down_10"),
            content.Load<Texture2D>("camera/anim/down_11"),
        ];
	}

}
