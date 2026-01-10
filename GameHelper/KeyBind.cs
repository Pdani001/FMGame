using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace ReFMGame.GameHelper;
public class KeyBind(bool ctrl = false, bool alt = false, bool shift = false, Keys key = Keys.None, char? kchar = null)
{
    public bool Ctrl { get; set; } = ctrl;
    public bool Alt { get; set; } = alt;
    public bool Shift { get; set; } = shift;
    public Keys Key { get; set; } = key;
    public char? Char { get; set; } = kchar;

    public bool IsValid(char? kchar = null)
    {
        var keyboard = KeyboardExtended.GetState();
        if (
            (Ctrl && keyboard.IsControlDown() || !Ctrl && !keyboard.IsControlDown()) &&
            (Alt && keyboard.IsAltDown() || !Alt && !keyboard.IsAltDown()) &&
            (Shift && keyboard.IsShiftDown() || !Shift && !keyboard.IsShiftDown())
        )
            if (keyboard.WasKeyPressed(Key) || (Char != null && Char == kchar))
            {
                return true;
            }
        return false;
    }
}
