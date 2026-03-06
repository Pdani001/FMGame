using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class MenuBgAnim : TextureAnimation
{
	public override int Threshold => -1;
	protected override Texture2D[] Frames { get; }

	public MenuBgAnim(ContentManager content)
	{
		Frames =
        [
            content.Load<Texture2D>("menu/bg/0"),
            content.Load<Texture2D>("menu/bg/1"),
            content.Load<Texture2D>("menu/bg/2"),
            content.Load<Texture2D>("menu/bg/3"),
        ];
	}

}
