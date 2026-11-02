using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the center draws the startup check's warnings, or doesn't, as expected.
/// </summary>
internal sealed class ExpectStartupWarningStep : ScenarioStep
{
    public ExpectStartupWarningStep(bool shown)
    {
        Shown = shown;
    }

    public bool Shown { get; }

    public override IEnumerator Run(StepContext context)
    {
        return WarningCheck.Run(context, Shown, "the startup check's warnings", w => w.StartupWarningLayout, "the center wasn't drawn");
    }
}
