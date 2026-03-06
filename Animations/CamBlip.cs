using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class CamBlip : TextureAnimation
{
	public override int Threshold => 23;
	protected override Texture2D[] Frames { get; }

	public CamBlip(ContentManager content)
	{
        Loop = false;
		Frames =
        [
            content.Load<Texture2D>("camera/anim/blip_1"),
            content.Load<Texture2D>("camera/anim/blip_2"),
            content.Load<Texture2D>("camera/anim/blip_3"),
            content.Load<Texture2D>("camera/anim/blip_4"),
            content.Load<Texture2D>("camera/anim/blip_5"),
            content.Load<Texture2D>("camera/anim/blip_6"),
            content.Load<Texture2D>("camera/anim/blip_7"),
            content.Load<Texture2D>("camera/anim/blip_8"),
            content.Load<Texture2D>("camera/anim/blip_9"),
        ];
	}

}
