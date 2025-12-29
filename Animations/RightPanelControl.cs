using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
class RightPanelControl : TextureAnimation
{
	public override int Threshold => -1;
	protected override Texture2D[] Frames { get; }

	public RightPanelControl(ContentManager content)
	{
		Frames =
        [
            content.Load<Texture2D>("office/doors/right/panel/open"),
            content.Load<Texture2D>("office/doors/right/panel/openlight"),
            content.Load<Texture2D>("office/doors/right/panel/closed"),
            content.Load<Texture2D>("office/doors/right/panel/closedlight"),
        ];
	}

}
