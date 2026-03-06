using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class StaticAnim : TextureAnimation
{
	public override int Threshold => 16;
	protected override Texture2D[] Frames { get; }

	public StaticAnim(ContentManager content)
	{
		Loop = true;
		Frames =
        [
            content.Load<Texture2D>("static/static_1"),
            content.Load<Texture2D>("static/static_2"),
            content.Load<Texture2D>("static/static_3"),
            content.Load<Texture2D>("static/static_4"),
            content.Load<Texture2D>("static/static_5"),
            content.Load<Texture2D>("static/static_6"),
            content.Load<Texture2D>("static/static_7"),
            content.Load<Texture2D>("static/static_8"),
		];
	}

}
