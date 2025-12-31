using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ReFMGame.GameHelper;

namespace ReFMGame.Animations.Jumpscare
{
    public class JumpBonnie : TextureAnimation
    {
        public override int Threshold => 22;
        protected override Texture2D[] Frames { get; }

        public JumpBonnie(ContentManager content)
        {
            Loop = true;
            Frames =
            [
                content.Load<Texture2D>("jumpscare/bonnie_01"),
                content.Load<Texture2D>("jumpscare/bonnie_02"),
                content.Load<Texture2D>("jumpscare/bonnie_03"),
                content.Load<Texture2D>("jumpscare/bonnie_04"),
                content.Load<Texture2D>("jumpscare/bonnie_05"),
                content.Load<Texture2D>("jumpscare/bonnie_06"),
                content.Load<Texture2D>("jumpscare/bonnie_07"),
                content.Load<Texture2D>("jumpscare/bonnie_08"),
                content.Load<Texture2D>("jumpscare/bonnie_09"),
                content.Load<Texture2D>("jumpscare/bonnie_10"),
                content.Load<Texture2D>("jumpscare/bonnie_11"),
            ];
        }
    }
}
