using Microsoft.Xna.Framework;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
class UsageMeter : FrameAnimation
{
	public override int Threshold => -1;
	protected override Rectangle[] Frames { get; }

	public UsageMeter()
	{
		Frames =
        [
            new(0,0,103,32),
			new(0,32,103,32),
			new(0,64,103,32),
			new(0,96,103,32),
			new(0,128,103,32),
		];
	}

}
