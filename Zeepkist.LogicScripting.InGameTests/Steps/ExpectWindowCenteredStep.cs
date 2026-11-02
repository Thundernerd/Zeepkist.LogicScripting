using System.Collections;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the window's middle is the screen's, to within <see cref="RectCheck.Tolerance"/>, as center-window and a
/// first open leave it.
/// </summary>
internal sealed class ExpectWindowCenteredStep : ScenarioStep
{
    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        var rect = context.Window.WindowRect;
        var screen = context.Window.WindowLayout.Screen;
        var expected = RectCheck.Point(screen.Center);
        var actual = RectCheck.Point(rect.Center);
        var detail = $"{RectCheck.Describe(rect)}, on a screen {RectCheck.Size(screen.W, screen.H)} px";
        if (Mathf.Abs(rect.Center.x - screen.Center.x) <= RectCheck.Tolerance && Mathf.Abs(rect.Center.y - screen.Center.y) <= RectCheck.Tolerance)
            context.Pass(detail, expected, actual);
        else
            context.Fail(detail, expected, actual);
    }
}
