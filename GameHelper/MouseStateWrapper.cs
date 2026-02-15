using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace ReFMGame.GameHelper;

public class MouseStateWrapper
{
    Point gameSize;
    MouseState mouseState;
    Vector2 scaledMousePosition;
    Rectangle renderTargetDestination;
    float screenScale;

    public bool Scaled { get; set; } = true;

    public Vector2 Position => Scaled ? scaledMousePosition : mouseState.Position.ToVector2();

    public MouseState MouseState => mouseState;

    public MouseStateWrapper(bool scaled = true) => Scaled = scaled;

    internal void SetMouseState(MouseState mouseState)
    {
        this.mouseState = mouseState;
        scaledMousePosition = mouseState.Position.ToVector2();
        if(screenScale == 1.0f)
        {
            float xScale = (float)gameSize.X / renderTargetDestination.Width;
            scaledMousePosition.X *= xScale;
            scaledMousePosition.X = (int)Math.Round(scaledMousePosition.X, 0, MidpointRounding.AwayFromZero);
            float yScale = (float)gameSize.Y / renderTargetDestination.Height;
            scaledMousePosition.Y *= yScale;
            scaledMousePosition.Y = (int)Math.Round(scaledMousePosition.Y, 0, MidpointRounding.AwayFromZero);
        }
        else
        {
            scaledMousePosition.X -= renderTargetDestination.X;
            scaledMousePosition.Y -= renderTargetDestination.Y;
            scaledMousePosition /= screenScale;
        }
    }

    internal void SetWindowSize(Point gameSize)
    {
        this.gameSize = gameSize;
    }

    internal void SetRenderTargetDestination(Rectangle renderTargetDestination)
    {
        gameSize = new(renderTargetDestination.Width, renderTargetDestination.Height);
        this.renderTargetDestination = renderTargetDestination;
    }

    internal void SetScreenScale(float scale)
    {
        screenScale = scale;
    }

    internal void SetMouseLocation(Point location) => SetMouseState(new MouseState(location.X, location.Y, mouseState.ScrollWheelValue,
        mouseState.LeftButton, mouseState.MiddleButton, mouseState.RightButton, mouseState.XButton1, mouseState.XButton2));

    internal void SetMouseLocation(int x, int y) => SetMouseState(new MouseState(x, y, mouseState.ScrollWheelValue,
        mouseState.LeftButton, mouseState.MiddleButton, mouseState.RightButton, mouseState.XButton1, mouseState.XButton2));

    public Point ScalePositionUp(Vector2 position)
    {
        Vector2 unscaledMousePosition = position;
        if (screenScale == 1.0f)
        {
            float xScale = (float)gameSize.X / renderTargetDestination.Width;
            unscaledMousePosition.X /= xScale;
            float yScale = (float)gameSize.Y / renderTargetDestination.Height;
            unscaledMousePosition.Y /= yScale;
        }
        else
        {
            unscaledMousePosition *= screenScale;
            unscaledMousePosition.X += renderTargetDestination.X;
            unscaledMousePosition.Y += renderTargetDestination.Y;
        }
        return unscaledMousePosition.ToPoint();
    }
}
