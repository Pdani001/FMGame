using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ReFMGame.GameHelper;
public class NumberRenderer
{
    private readonly Dictionary<char, Texture2D> textures = new()
    {
        { '0', null },
        { '1', null },
        { '2', null },
        { '3', null },
        { '4', null },
        { '5', null },
        { '6', null },
        { '7', null },
        { '8', null },
        { '9', null },
        { '.', null },
        { ',', null },
        { '-', null },
        { '+', null },
        { 'e', null },
    };
    public NumberRenderer(Game game, string fontName)
    {
        foreach (char c in textures.Keys.ToArray())
        {
            string target = c switch
            {
                '.' => "dot",
                ',' => "dot",
                '-' => "minus",
                '+' => "plus",
                _ => c.ToString()
            };
            try
            {
                textures[c] = game.Content.Load<Texture2D>("numbers/" + fontName + "/" + target);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{GetType().Name}] Failed to load texture for character '{c}' in number font '{fontName}': {ex.Message}");
                Debug.WriteLine($"[{GetType().Name}] Failed to load texture for character '{c}' in number font '{fontName}': {ex.Message}");
            }
        }
    }
    public void DrawNumber(SpriteBatch spriteBatch, string number, Vector2 position, float layer = 1f)
    {
        float x = position.X;
        int lastWidth = 0;
        foreach (char c in number.Reverse())
        {
            if (textures.TryGetValue(c, out Texture2D texture) && texture != null)
            {
                lastWidth = texture.Width;
                x -= texture.Width;
                spriteBatch.Draw(texture, new Vector2(x, position.Y), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, layer);
            }
            else
            {
                x -= lastWidth;
            }
        }
    }
}
