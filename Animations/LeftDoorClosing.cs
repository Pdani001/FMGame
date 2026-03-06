using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class LeftDoorClosing : TextureAnimation
{
	public override int Threshold => 33;
	protected override Texture2D[] Frames { get; }

	public LeftDoorClosing(ContentManager content)
	{
        Loop = false;
		Frames =
        [
            content.Load<Texture2D>("office/doors/left/closing/_01"),
            content.Load<Texture2D>("office/doors/left/closing/_02"),
            content.Load<Texture2D>("office/doors/left/closing/_03"),
            content.Load<Texture2D>("office/doors/left/closing/_04"),
            content.Load<Texture2D>("office/doors/left/closing/_05"),
            content.Load<Texture2D>("office/doors/left/closing/_06"),
            content.Load<Texture2D>("office/doors/left/closing/_07"),
            content.Load<Texture2D>("office/doors/left/closing/_08"),
            content.Load<Texture2D>("office/doors/left/closing/_09"),
            content.Load<Texture2D>("office/doors/left/closing/_10"),
            content.Load<Texture2D>("office/doors/left/closing/_11"),
            content.Load<Texture2D>("office/doors/left/closing/_12"),
            content.Load<Texture2D>("office/doors/left/closing/_13"),
            content.Load<Texture2D>("office/doors/left/closing/_14"),
            content.Load<Texture2D>("office/doors/left/closing/_15"),
            content.Load<Texture2D>("office/doors/left/closing/_16"),
        ];
	}

}
