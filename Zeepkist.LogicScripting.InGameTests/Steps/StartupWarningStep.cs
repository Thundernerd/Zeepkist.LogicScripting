using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

internal sealed class StartupWarningStep : ScenarioStep
{
    public StartupWarningStep(bool show)
    {
        Show = show;
    }

    public bool Show { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        context.Window.ShowStartupWarning(Show);

        // A frame to draw the center with or without the warnings, and one for the window's height to follow
        yield return null;
        yield return null;
    }
}
