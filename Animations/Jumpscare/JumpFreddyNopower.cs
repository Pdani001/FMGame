using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Jumpscare
{
    public class JumpFreddyNopower : TextureAnimation
    {
        public override int Threshold => 28;
        protected override Texture2D[] Frames { get; }

        public JumpFreddyNopower(ContentManager content)
        {
            Loop = false;
            Frames =
            [
                content.Load<Texture2D>("jumpscare/freddy_nopower_01"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_02"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_03"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_04"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_05"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_06"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_07"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_08"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_09"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_10"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_11"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_12"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_13"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_14"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_15"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_16"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_17"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_18"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_19"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_20"),
                content.Load<Texture2D>("jumpscare/freddy_nopower_21"),
            ];
        }
    }
}
