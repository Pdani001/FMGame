using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Jumpscare
{
    public class JumpFoxy : TextureAnimation
    {
        public override int Threshold => 33;
        protected override Texture2D[] Frames { get; }

        public JumpFoxy(ContentManager content)
        {
            Loop = false;
            Frames =
            [
                content.Load<Texture2D>("jumpscare/foxy_01"),
                content.Load<Texture2D>("jumpscare/foxy_02"),
                content.Load<Texture2D>("jumpscare/foxy_03"),
                content.Load<Texture2D>("jumpscare/foxy_04"),
                content.Load<Texture2D>("jumpscare/foxy_05"),
                content.Load<Texture2D>("jumpscare/foxy_06"),
                content.Load<Texture2D>("jumpscare/foxy_07"),
                content.Load<Texture2D>("jumpscare/foxy_08"),
                content.Load<Texture2D>("jumpscare/foxy_09"),
                content.Load<Texture2D>("jumpscare/foxy_10"),
                content.Load<Texture2D>("jumpscare/foxy_11"),
                content.Load<Texture2D>("jumpscare/foxy_12"),
                content.Load<Texture2D>("jumpscare/foxy_13"),
                content.Load<Texture2D>("jumpscare/foxy_14"),
                content.Load<Texture2D>("jumpscare/foxy_15"),
                content.Load<Texture2D>("jumpscare/foxy_16"),
                content.Load<Texture2D>("jumpscare/foxy_17"),
                content.Load<Texture2D>("jumpscare/foxy_18"),
                content.Load<Texture2D>("jumpscare/foxy_19"),
                content.Load<Texture2D>("jumpscare/foxy_20"),
                content.Load<Texture2D>("jumpscare/foxy_21"),
                content.Load<Texture2D>("jumpscare/foxy_22"),
                content.Load<Texture2D>("jumpscare/foxy_23"),
                content.Load<Texture2D>("jumpscare/foxy_24"),
                content.Load<Texture2D>("jumpscare/foxy_25"),
            ];
        }
    }
}
