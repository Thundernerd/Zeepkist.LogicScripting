using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the hover box doesn't show.
/// </summary>
internal sealed class ExpectNoHoverStep : ScenarioStep
{
    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        // Drawn this frame, so a box it didn't draw is one it has nothing for and not one it missed
        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        var lines = context.Window.HoverLines;
        if (lines == null)
            context.Pass("no hover box", ExpectHoverStep.NoHover, ExpectHoverStep.NoHover);
        else
            context.Fail("the hover box shows", ExpectHoverStep.NoHover, ExpectHoverStep.Describe(lines));
    }
}
