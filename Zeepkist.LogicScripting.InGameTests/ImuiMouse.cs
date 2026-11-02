using Imui.IO.Events;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests;

/// <summary>
/// Hands Imui a mouse the way its backend's pointer handlers hand it Unity's: a position, which Imui reads each frame
/// to find the hovered control, and the left button's events, one a frame. Unity's own <c>Input</c>, and so the level
/// editor, never sees them.
/// </summary>
internal static class ImuiMouse
{
    /// <summary>
    /// Where Imui finds the mouse, in its own pixels, or null while it reads the player's.
    /// </summary>
    public static Vector2? Position { get; private set; }

    public static void MoveTo(Vector2 position)
    {
        Position = position;
    }

    /// <summary>
    /// Imui reads the player's mouse again from the next frame.
    /// </summary>
    public static void GiveBack()
    {
        Position = null;
    }

    /// <summary>
    /// Returns false when there's no backend to queue it on.
    /// </summary>
    public static bool Press()
    {
        return Queue(new ImMouseEvent(ImMouseEventType.Down, 0, EventModifiers.None, Vector2.zero, ImMouseDevice.Mouse));
    }

    /// <summary>
    /// Moves the mouse by <paramref name="delta"/> with the button held. Returns false when there's no backend.
    /// </summary>
    public static bool Drag(Vector2 delta)
    {
        Position += delta;
        return Queue(new ImMouseEvent(ImMouseEventType.Drag, 0, EventModifiers.None, delta, ImMouseDevice.Mouse));
    }

    public static bool Release()
    {
        return Queue(new ImMouseEvent(ImMouseEventType.Up, 0, EventModifiers.None, Vector2.zero, ImMouseDevice.Mouse));
    }

    private static bool Queue(ImMouseEvent mouseEvent)
    {
        var backend = ImuiBackend.Find();
        if (backend == null)
            return false;

        backend.mouseEventsQueue.PushFront(mouseEvent);
        return true;
    }
}
