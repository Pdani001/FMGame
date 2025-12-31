using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Jumpscare
{
    public class JumpChica : TextureAnimation
    {
        public override int Threshold => 17;
        protected override Texture2D[] Frames { get; }

        public JumpChica(ContentManager content)
        {
            Loop = true;
            Frames =
            [
                content.Load<Texture2D>("jumpscare/chica_01"),
                content.Load<Texture2D>("jumpscare/chica_02"),
                content.Load<Texture2D>("jumpscare/chica_03"),
                content.Load<Texture2D>("jumpscare/chica_04"),
                content.Load<Texture2D>("jumpscare/chica_05"),
                content.Load<Texture2D>("jumpscare/chica_06"),
                content.Load<Texture2D>("jumpscare/chica_07"),
                content.Load<Texture2D>("jumpscare/chica_08"),
                content.Load<Texture2D>("jumpscare/chica_09"),
                content.Load<Texture2D>("jumpscare/chica_10"),
                content.Load<Texture2D>("jumpscare/chica_11"),
                content.Load<Texture2D>("jumpscare/chica_12"),
                content.Load<Texture2D>("jumpscare/chica_13"),
                content.Load<Texture2D>("jumpscare/chica_14"),
                content.Load<Texture2D>("jumpscare/chica_15"),
                content.Load<Texture2D>("jumpscare/chica_16"),
            ];
        }
    }
}
