using Microsoft.Xna.Framework;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class SpinnerAnim : FrameAnimation
{
	public override int Threshold => 33;
	protected override Rectangle[] Frames { get; }

	public SpinnerAnim()
	{
		Frames =
        [
            new(0,0,64,64),
			new(64,0,64,64),
			new(0,64,64,64),
			new(64,64,64,64),
			new(0,128,64,64),
			new(64,128,64,64),
			new(0,192,64,64),
			new(64,192,64,64),
			new(0,256,64,64),
			new(64,256,64,64),
			new(0,320,64,64),
			new(64,320,64,64),
			new(0,384,64,64),
			new(64,384,64,64),
			new(0,448,64,64),
		];
	}

}
