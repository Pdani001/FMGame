using Microsoft.Xna.Framework;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
class CamButton : FrameAnimation
{
	public override int Threshold => 555;
	protected override Rectangle[] Frames { get; }

	public CamButton()
	{
		Frames =
        [
            new(0,0,60,40),
			new(60,0,60,40),
		];
	}

}
