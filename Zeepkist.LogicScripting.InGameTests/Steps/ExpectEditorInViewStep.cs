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
        var check = EditorLayoutCheck.Run(layout, context.Window.WindowRect);
        if (check.Passed)
            context.Pass($"the editor is {RectCheck.Pixels(layout.Field.H)} px tall, with the error panel's header under it", null, null);
        else
            context.Fail(check.Failure, check.Expected, check.Actual);
    }
}
