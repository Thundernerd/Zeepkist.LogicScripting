namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// Whether the window drew a warning, the last frame it drew the part of the window the warning goes in.
/// </summary>
internal readonly struct WarningLayout
{
    public WarningLayout(int frame, bool shown)
    {
        Frame = frame;
        Shown = shown;
    }

    /// <summary>
    /// Unity's frame count when it was drawn.
    /// </summary>
    public int Frame { get; }

    public bool Shown { get; }
}
