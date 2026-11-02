using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Remembers the window's rect, as it's drawn, for <see cref="ExpectWindowRectStep"/>.
/// </summary>
internal sealed class RememberWindowRectStep : ScenarioStep
{
    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        context.RememberedWindowRect = context.Window.WindowRect;
        context.Pass(RectCheck.Describe(context.Window.WindowRect), null, null);
    }
}
