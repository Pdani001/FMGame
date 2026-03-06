using Microsoft.Xna.Framework;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class CamMap : FrameAnimation
{
	public override int Threshold => 835;
	protected override Rectangle[] Frames { get; }

	public CamMap()
	{
		Frames =
        [
            new(0,0,400,400),
			new(400,0,400,400),
		];
	}

}
