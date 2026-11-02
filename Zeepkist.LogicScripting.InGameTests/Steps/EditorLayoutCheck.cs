using Imui.Core;
using TNRD.Zeepkist.LogicScripting.UI;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// The checks of <see cref="ExpectEditorInViewStep"/>, for one frame's <see cref="EditorLayout"/>.
/// </summary>
internal readonly struct EditorLayoutCheck
{
    private EditorLayoutCheck(string failure, string expected, string actual)
    {
        Failure = failure;
        Expected = expected;
        Actual = actual;
    }

    /// <summary>
    /// What's wrong, or null when the layout passed.
    /// </summary>
    public string Failure { get; }

    public string Expected { get; }
    public string Actual { get; }

    public static EditorLayoutCheck Run(EditorLayout layout, ImRect window)
    {
        if (!RectCheck.Contains(window, layout.Center))
            return new EditorLayoutCheck("the center reaches outside the window", RectCheck.Describe(window), RectCheck.Describe(layout.Center));

        if (layout.Field.H < layout.MinFieldHeight - RectCheck.Tolerance)
            return new EditorLayoutCheck("the editor is less than three rows tall", $"{RectCheck.Pixels(layout.MinFieldHeight)} px or more", $"{RectCheck.Pixels(layout.Field.H)} px");

        if (!RectCheck.Contains(layout.Center, layout.Field))
            return new EditorLayoutCheck("the editor reaches outside the center", RectCheck.Describe(layout.Center), RectCheck.Describe(layout.Field));

        if (!RectCheck.Contains(layout.Center, layout.ErrorsHeader))
            return new EditorLayoutCheck("the error panel's header reaches outside the center", RectCheck.Describe(layout.Center), RectCheck.Describe(layout.ErrorsHeader));

        return default;
    }
}
