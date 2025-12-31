using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
class OfficeControl : TextureAnimation
{
	public override int Threshold => -1;
	protected override Texture2D[] Frames { get; }

	public OfficeControl(ContentManager content)
	{
		Frames =
        [
            content.Load<Texture2D>("office/default"),
            content.Load<Texture2D>("office/left-none"),
            content.Load<Texture2D>("office/left-bonnie"),
            content.Load<Texture2D>("office/right-none"),
            content.Load<Texture2D>("office/right-chica"),
            content.Load<Texture2D>("office/dark"),
            content.Load<Texture2D>("office/dark-freddy"),
            content.Load<Texture2D>("office/black"),
        ];
	}

}
