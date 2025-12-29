using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
class LeftDoorOpening : TextureAnimation
{
	public override int Threshold => 33;
	protected override Texture2D[] Frames { get; }

	public LeftDoorOpening(ContentManager content)
	{
        Loop = false;
		Frames =
        [
            content.Load<Texture2D>("office/doors/left/opening/_01"),
            content.Load<Texture2D>("office/doors/left/opening/_02"),
            content.Load<Texture2D>("office/doors/left/opening/_03"),
            content.Load<Texture2D>("office/doors/left/opening/_04"),
            content.Load<Texture2D>("office/doors/left/opening/_05"),
            content.Load<Texture2D>("office/doors/left/opening/_06"),
            content.Load<Texture2D>("office/doors/left/opening/_07"),
            content.Load<Texture2D>("office/doors/left/opening/_08"),
            content.Load<Texture2D>("office/doors/left/opening/_09"),
            content.Load<Texture2D>("office/doors/left/opening/_10"),
            content.Load<Texture2D>("office/doors/left/opening/_11"),
            content.Load<Texture2D>("office/doors/left/opening/_12"),
            content.Load<Texture2D>("office/doors/left/opening/_13"),
            content.Load<Texture2D>("office/doors/left/opening/_14"),
            content.Load<Texture2D>("office/doors/left/opening/_15"),
            content.Load<Texture2D>("office/doors/left/opening/_16"),
        ];
	}

}
