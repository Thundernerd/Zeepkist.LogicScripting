using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

internal sealed class CenterWindowStep : ScenarioStep
{
    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        context.Window.CenterWindow();
        yield return context.WaitForWindowRect();
        if (context.Failed)
            yield break;

        context.Pass(RectCheck.Describe(context.Window.WindowRect), null, null);
    }
}
