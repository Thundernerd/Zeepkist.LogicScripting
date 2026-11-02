using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the Script editor is at least three rows tall, and it and the error panel's header lie inside the
/// center, inside the window.
/// </summary>
internal sealed class ExpectEditorInViewStep : ScenarioStep
{
    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        // The window takes its minimum height a frame after what it counts changed, so the center settles after that
        yield return null;
        yield return null;
        yield return null;

        var layout = context.Window.EditorLayout;
        var window = context.Window.WindowRect;
        if (!RectCheck.Contains(window, layout.Center))
            context.Fail("the center reaches outside the window", RectCheck.Describe(window), RectCheck.Describe(layout.Center));
        else if (layout.Field.H < layout.MinFieldHeight - RectCheck.Tolerance)
            context.Fail("the editor is less than three rows tall", $"{RectCheck.Pixels(layout.MinFieldHeight)} px or more", $"{RectCheck.Pixels(layout.Field.H)} px");
        else if (!RectCheck.Contains(layout.Center, layout.Field))
            context.Fail("the editor reaches outside the center", RectCheck.Describe(layout.Center), RectCheck.Describe(layout.Field));
        else if (!RectCheck.Contains(layout.Center, layout.ErrorsHeader))
            context.Fail("the error panel's header reaches outside the center", RectCheck.Describe(layout.Center), RectCheck.Describe(layout.ErrorsHeader));
        else
            context.Pass($"the editor is {RectCheck.Pixels(layout.Field.H)} px tall, with the error panel's header under it", null, null);
    }
}
