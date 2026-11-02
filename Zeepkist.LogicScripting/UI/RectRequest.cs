using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// The size and position the in-game tests asked the window for, which it takes as it's next drawn, as Imui keeps the
/// window's rect itself. A position is either a top left corner or the middle of the screen.
/// </summary>
internal sealed class RectRequest
{
    public float? Width { get; private set; }
    public float? Height { get; private set; }

    /// <summary>
    /// Null when the window keeps its top left corner, or moves to the middle of the screen.
    /// </summary>
    public Vector2? TopLeft { get; private set; }

    public bool Center { get; private set; }

    public bool IsPending => Width != null || Height != null || TopLeft != null || Center;

    public void SetWidth(float width)
    {
        Width = width;
    }

    public void SetHeight(float height)
    {
        Height = height;
    }

    public void MoveTo(Vector2 topLeft)
    {
        TopLeft = topLeft;
        Center = false;
    }

    public void MoveToCenter()
    {
        TopLeft = null;
        Center = true;
    }

    public void Clear()
    {
        Width = null;
        Height = null;
        TopLeft = null;
        Center = false;
    }
}
