using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

internal sealed class ShowBlocksTabStep : ScenarioStep
{
    public ShowBlocksTabStep(bool reference)
    {
        Reference = reference;
    }

    public bool Reference { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (context.RequireOpenWindow())
            context.Window.ShowBlocksTab(Reference);

        yield return null;
    }
}
