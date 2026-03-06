using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations;
public class CamLocationText : TextureAnimation
{
	public override int Threshold => -1;
	protected override Texture2D[] Frames { get; }

	public CamLocationText(ContentManager content)
	{
        Loop = false;
		Frames =
        [
            content.Load<Texture2D>("camera/location/stage"),
            content.Load<Texture2D>("camera/location/dining"),
            content.Load<Texture2D>("camera/location/cove"),
            content.Load<Texture2D>("camera/location/west_hall"),
            content.Load<Texture2D>("camera/location/west_hall_corner"),
            content.Load<Texture2D>("camera/location/closet"),
            content.Load<Texture2D>("camera/location/east_hall"),
            content.Load<Texture2D>("camera/location/east_hall_corner"),
            content.Load<Texture2D>("camera/location/backstage"),
            content.Load<Texture2D>("camera/location/kitchen"),
            content.Load<Texture2D>("camera/location/restrooms"),
        ];
	}

}
