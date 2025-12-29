using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
class CamButtonText : TextureAnimation
{
	public override int Threshold => -1;
	protected override Texture2D[] Frames { get; }

	public CamButtonText(ContentManager content)
	{
        Loop = false;
		Frames =
        [
            content.Load<Texture2D>("camera/button/1a"),
            content.Load<Texture2D>("camera/button/1b"),
            content.Load<Texture2D>("camera/button/1c"),
            content.Load<Texture2D>("camera/button/2a"),
            content.Load<Texture2D>("camera/button/2b"),
            content.Load<Texture2D>("camera/button/3"),
            content.Load<Texture2D>("camera/button/4a"),
            content.Load<Texture2D>("camera/button/4b"),
            content.Load<Texture2D>("camera/button/5"),
            content.Load<Texture2D>("camera/button/6"),
            content.Load<Texture2D>("camera/button/7"),
        ];
	}

}
