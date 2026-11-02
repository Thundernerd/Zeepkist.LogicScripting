using Imui.Core;

namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// The window's minimum height, Imui's resize handle and the screen it's on, the last frame it drew them.
/// </summary>
internal readonly struct WindowLayout
{
    public WindowLayout(int frame, float minHeight, ImRect resizeHandle, bool isResizing, ImRect screen)
    {
        Frame = frame;
        MinHeight = minHeight;
        ResizeHandle = resizeHandle;
        IsResizing = isResizing;
        Screen = screen;
    }

    /// <summary>
    /// Unity's frame count when it was drawn.
    /// </summary>
    public int Frame { get; }

    public float MinHeight { get; }

    /// <summary>
    /// The corner the player drags to resize the window.
    /// </summary>
    public ImRect ResizeHandle { get; }

    /// <summary>
    /// Whether the resize handle holds the mouse, from a press on it until the mouse is let go.
    /// </summary>
    public bool IsResizing { get; }

    /// <summary>
    /// The part of the screen the window keeps to, Imui's safe screen rect.
    /// </summary>
    public ImRect Screen { get; }
}
