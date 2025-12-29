using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
class RightDoorOpening : TextureAnimation
{
	public override int Threshold => 33;
	protected override Texture2D[] Frames { get; }

	public RightDoorOpening(ContentManager content)
	{
        Loop = false;
		Frames =
        [
            content.Load<Texture2D>("office/doors/right/opening/_01"),
            content.Load<Texture2D>("office/doors/right/opening/_02"),
            content.Load<Texture2D>("office/doors/right/opening/_03"),
            content.Load<Texture2D>("office/doors/right/opening/_04"),
            content.Load<Texture2D>("office/doors/right/opening/_05"),
            content.Load<Texture2D>("office/doors/right/opening/_06"),
            content.Load<Texture2D>("office/doors/right/opening/_07"),
            content.Load<Texture2D>("office/doors/right/opening/_08"),
            content.Load<Texture2D>("office/doors/right/opening/_09"),
            content.Load<Texture2D>("office/doors/right/opening/_10"),
            content.Load<Texture2D>("office/doors/right/opening/_11"),
            content.Load<Texture2D>("office/doors/right/opening/_12"),
            content.Load<Texture2D>("office/doors/right/opening/_13"),
            content.Load<Texture2D>("office/doors/right/opening/_14"),
            content.Load<Texture2D>("office/doors/right/opening/_15"),
            content.Load<Texture2D>("office/doors/right/opening/_16"),
        ];
	}

}
