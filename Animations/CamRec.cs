using Microsoft.Xna.Framework;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
class CamRec : FrameAnimation
{
	public override int Threshold => 835;
	protected override Rectangle[] Frames { get; }

	public CamRec()
	{
		Frames =
        [
            new(0,0,50,50),
			new(50,0,50,50),
		];
	}

}
