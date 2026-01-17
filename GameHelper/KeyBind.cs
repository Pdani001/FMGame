using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;

namespace ReFMGame.GameHelper;
public class KeyBind(bool Ctrl = false, bool Shift = false, Keys Key = Keys.None, char? Char = null)
{
    public bool Ctrl { get; set; } = Ctrl;
    public bool Shift { get; set; } = Shift;
    public Keys Key { get; set; } = Key;
    public char? Char { get; set; } = Char;

    public override bool Equals(object obj)
    {
        return obj is KeyBind bind &&
               Ctrl == bind.Ctrl &&
               Shift == bind.Shift &&
               Key == bind.Key &&
               Char == bind.Char;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Ctrl, Shift, Key, Char);
    }

    public bool IsValid(char? kchar = null)
    {
        var keyboard = KeyboardExtended.GetState();
        if (
            (Ctrl && keyboard.IsControlDown() || !Ctrl && !keyboard.IsControlDown()) &&
            (Shift && keyboard.IsShiftDown() || !Shift && !keyboard.IsShiftDown())
        )
            if ((Char == null && keyboard.WasKeyPressed(Key)) || (Char != null && Char == kchar))
            {
                return true;
            }
        return false;
    }

    public override string ToString()
    {
        List<string> modifiers = [];
        if (Ctrl)
            modifiers.Add("CTRL");
        if (Shift)
            modifiers.Add("SHIFT");
        return string.Join(" + ", modifiers) + (modifiers.Count > 0 ? " + " : "") + (Char != null ? Char.ToString().ToUpper() : Key != Keys.None ? Key.ToString() : "");
    }
}

public enum BindKey
{
    Fullscreen,
    Chat,
    Screenshot,
    Debug = 99,
}
