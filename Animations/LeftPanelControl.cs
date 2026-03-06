using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class LeftPanelControl : TextureAnimation
{
	public override int Threshold => -1;
	protected override Texture2D[] Frames { get; }

	public LeftPanelControl(ContentManager content)
	{
		Frames =
        [
            content.Load<Texture2D>("office/doors/left/panel/open"),
            content.Load<Texture2D>("office/doors/left/panel/openlight"),
            content.Load<Texture2D>("office/doors/left/panel/closed"),
            content.Load<Texture2D>("office/doors/left/panel/closedlight"),
        ];
	}

}
