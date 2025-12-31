using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Jumpscare
{
    public class JumpFreddy : TextureAnimation
    {
        public override int Threshold => 33;
        protected override Texture2D[] Frames { get; }

        public JumpFreddy(ContentManager content)
        {
            Loop = false;
            Frames =
            [
                content.Load<Texture2D>("jumpscare/freddy_01"),
                content.Load<Texture2D>("jumpscare/freddy_02"),
                content.Load<Texture2D>("jumpscare/freddy_03"),
                content.Load<Texture2D>("jumpscare/freddy_04"),
                content.Load<Texture2D>("jumpscare/freddy_05"),
                content.Load<Texture2D>("jumpscare/freddy_06"),
                content.Load<Texture2D>("jumpscare/freddy_07"),
                content.Load<Texture2D>("jumpscare/freddy_08"),
                content.Load<Texture2D>("jumpscare/freddy_09"),
                content.Load<Texture2D>("jumpscare/freddy_10"),
                content.Load<Texture2D>("jumpscare/freddy_11"),
                content.Load<Texture2D>("jumpscare/freddy_12"),
                content.Load<Texture2D>("jumpscare/freddy_13"),
                content.Load<Texture2D>("jumpscare/freddy_14"),
                content.Load<Texture2D>("jumpscare/freddy_15"),
                content.Load<Texture2D>("jumpscare/freddy_16"),
                content.Load<Texture2D>("jumpscare/freddy_17"),
                content.Load<Texture2D>("jumpscare/freddy_18"),
                content.Load<Texture2D>("jumpscare/freddy_19"),
                content.Load<Texture2D>("jumpscare/freddy_20"),
                content.Load<Texture2D>("jumpscare/freddy_21"),
                content.Load<Texture2D>("jumpscare/freddy_22"),
                content.Load<Texture2D>("jumpscare/freddy_23"),
                content.Load<Texture2D>("jumpscare/freddy_24"),
                content.Load<Texture2D>("jumpscare/freddy_25"),
                content.Load<Texture2D>("jumpscare/freddy_26"),
                content.Load<Texture2D>("jumpscare/freddy_27"),
                content.Load<Texture2D>("jumpscare/freddy_28"),
                content.Load<Texture2D>("jumpscare/freddy_29"),
                content.Load<Texture2D>("jumpscare/freddy_30"),
                content.Load<Texture2D>("jumpscare/freddy_31"),
            ];
        }
    }
}
