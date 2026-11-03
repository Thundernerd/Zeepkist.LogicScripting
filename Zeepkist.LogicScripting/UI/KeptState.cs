using Imui.Core;

namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// What the window keeps from one in-game test Scenario to the next that a Scenario can change, for the run to put back.
/// </summary>
internal readonly struct KeptState
{
    public KeptState(
        ImRect? windowRect,
        bool showLibraries,
        bool showReference,
        string referenceFilter,
        bool showLibraryReadFailed,
        bool showStartupWarning)
    {
        WindowRect = windowRect;
        ShowLibraries = showLibraries;
        ShowReference = showReference;
        ReferenceFilter = referenceFilter;
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

    /// <summary>
    /// Whether the Blocks side shows its Reference tab rather than its Placed blocks tab.
    /// </summary>
    public bool ShowReference { get; }

    /// <summary>
    /// The text in the Reference tab's filter.
    /// </summary>
    public string ReferenceFilter { get; }

    public bool ShowLibraryReadFailed { get; }
    public bool ShowStartupWarning { get; }

    public KeptState WithWindowRect(ImRect windowRect)
    {
        return new KeptState(windowRect, ShowLibraries, ShowReference, ReferenceFilter, ShowLibraryReadFailed, ShowStartupWarning);
    }
}
