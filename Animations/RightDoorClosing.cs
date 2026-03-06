using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class RightDoorClosing : TextureAnimation
{
	public override int Threshold => 33;
	protected override Texture2D[] Frames { get; }

	public RightDoorClosing(ContentManager content)
	{
        Loop = false;
		Frames =
        [
            content.Load<Texture2D>("office/doors/right/closing/_01"),
            content.Load<Texture2D>("office/doors/right/closing/_02"),
            content.Load<Texture2D>("office/doors/right/closing/_03"),
            content.Load<Texture2D>("office/doors/right/closing/_04"),
            content.Load<Texture2D>("office/doors/right/closing/_05"),
            content.Load<Texture2D>("office/doors/right/closing/_06"),
            content.Load<Texture2D>("office/doors/right/closing/_07"),
            content.Load<Texture2D>("office/doors/right/closing/_08"),
            content.Load<Texture2D>("office/doors/right/closing/_09"),
            content.Load<Texture2D>("office/doors/right/closing/_10"),
            content.Load<Texture2D>("office/doors/right/closing/_11"),
            content.Load<Texture2D>("office/doors/right/closing/_12"),
            content.Load<Texture2D>("office/doors/right/closing/_13"),
            content.Load<Texture2D>("office/doors/right/closing/_14"),
            content.Load<Texture2D>("office/doors/right/closing/_15"),
            content.Load<Texture2D>("office/doors/right/closing/_16"),
        ];
	}

}
