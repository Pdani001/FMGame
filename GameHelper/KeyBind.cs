using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace ReFMGame.GameHelper;
public class KeyBind(bool ctrl = false, bool alt = false, bool shift = false, Keys key = Keys.None)
{
    public bool Ctrl { get; set; } = ctrl;
    public bool Alt { get; set; } = alt;
    public bool Shift { get; set; } = shift;
    public Keys Key { get; set; } = key;

    public bool IsValid()
    {
        var keyboard = KeyboardExtended.GetState();
        if (
            (Ctrl && keyboard.IsControlDown() || !Ctrl && !keyboard.IsControlDown()) &&
            (Alt && keyboard.IsAltDown() || !Alt && !keyboard.IsAltDown()) &&
            (Shift && keyboard.IsShiftDown() || !Shift && !keyboard.IsShiftDown())
        )
            if (keyboard.WasKeyPressed(Key))
            {
                return true;
            }
        return false;
    }
}
