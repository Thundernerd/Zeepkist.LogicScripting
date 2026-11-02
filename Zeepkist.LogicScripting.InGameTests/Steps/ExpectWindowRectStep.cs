using System.Collections;

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

        yield return context.ExpectWindowRect(remembered, "the window is where it was", "the window's size or position changed");
    }
}
