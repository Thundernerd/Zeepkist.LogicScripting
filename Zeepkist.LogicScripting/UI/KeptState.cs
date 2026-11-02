using Imui.Core;

namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// What the window keeps from one in-game test Scenario to the next that a Scenario can change, for the run to put back.
/// </summary>
internal readonly struct KeptState
{
    public KeptState(ImRect? windowRect, bool showLibraries, bool showLibraryReadFailed, bool showStartupWarning)
    {
        WindowRect = windowRect;
        ShowLibraries = showLibraries;
        ShowLibraryReadFailed = showLibraryReadFailed;
        ShowStartupWarning = showStartupWarning;
    }

    /// <summary>
    /// Null for a window that was never drawn, which opens at its first size and position.
    /// </summary>
    public ImRect? WindowRect { get; }

    /// <summary>
    /// Whether the Scripts side shows its Libraries tab rather than its Scripts tab.
    /// </summary>
    public bool ShowLibraries { get; }

    public bool ShowLibraryReadFailed { get; }
    public bool ShowStartupWarning { get; }

    public KeptState WithWindowRect(ImRect windowRect)
    {
        return new KeptState(windowRect, ShowLibraries, ShowLibraryReadFailed, ShowStartupWarning);
    }
}
