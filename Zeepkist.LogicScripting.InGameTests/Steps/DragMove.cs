namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// One move of the mouse in a <see cref="DragWindowEdgeStep"/>: <see cref="Pixels"/> one way, below 0 for the other,
/// in even steps over <see cref="Frames"/>.
/// </summary>
internal readonly struct DragMove
{
    public DragMove(int pixels, int frames)
    {
        Pixels = pixels;
        Frames = frames;
    }

    public int Pixels { get; }
    public int Frames { get; }
}
