using Microsoft.Xna.Framework;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
class FanLoop : FrameAnimation
{
	public override int Threshold => 33;
	protected override Rectangle[] Frames { get; }

	public FanLoop()
	{
		Frames =
        [
            new(0,0,138,196),
			new(138,0,138,196),
			new(276,0,138,196),
		];
	}

}
