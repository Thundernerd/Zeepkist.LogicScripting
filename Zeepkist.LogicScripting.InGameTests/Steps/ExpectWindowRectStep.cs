using System.Collections;
using Imui.Core;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the window is drawn at the size and position <see cref="RememberWindowRectStep"/> remembered.
/// </summary>
internal sealed class ExpectWindowRectStep : ScenarioStep
{
    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        if (context.RememberedWindowRect is not { } remembered)
        {
            context.Fail("no rect remembered: add remember-window-rect first");
            yield break;
        }

        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        var rect = context.Window.WindowRect;
        var expected = RectCheck.Describe(remembered);
        var actual = RectCheck.Describe(rect);
        if (IsSame(remembered, rect))
            context.Pass("the window is where it was", expected, actual);
        else
            context.Fail("the window's size or position changed", expected, actual);
    }

    private static bool IsSame(ImRect a, ImRect b)
    {
        return Mathf.Abs(a.X - b.X) <= RectCheck.Tolerance
            && Mathf.Abs(a.Y - b.Y) <= RectCheck.Tolerance
            && Mathf.Abs(a.W - b.W) <= RectCheck.Tolerance
            && Mathf.Abs(a.H - b.H) <= RectCheck.Tolerance;
    }
}
