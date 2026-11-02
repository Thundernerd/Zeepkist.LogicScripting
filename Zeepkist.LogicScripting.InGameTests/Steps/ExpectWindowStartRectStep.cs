using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the window is drawn at the size and position it was first drawn at in the run, which the run puts back
/// before each Scenario after the first.
/// </summary>
internal sealed class ExpectWindowStartRectStep : ScenarioStep
{
    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        if (context.StartWindowRect is not { } start)
        {
            context.Fail("the run has no start rect: the window wasn't drawn yet");
            yield break;
        }

        yield return context.ExpectWindowRect(start, "the window is where the run started it", "the window isn't where the run started it");
    }
}
