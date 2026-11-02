namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// What the window keeps from one in-game test Scenario to the next that a Scenario can change, for the run to put back.
/// </summary>
internal readonly struct KeptState
{
    public KeptState(float height, bool showLibraries, bool showLibraryReadFailed, bool showStartupWarning)
    {
        Height = height;
        ShowLibraries = showLibraries;
        ShowLibraryReadFailed = showLibraryReadFailed;
        ShowStartupWarning = showStartupWarning;
    }

    public float Height { get; }

    /// <summary>
    /// Whether the Scripts side shows its Libraries tab rather than its Scripts tab.
    /// </summary>
    public bool ShowLibraries { get; }

    public bool ShowLibraryReadFailed { get; }
    public bool ShowStartupWarning { get; }
}
