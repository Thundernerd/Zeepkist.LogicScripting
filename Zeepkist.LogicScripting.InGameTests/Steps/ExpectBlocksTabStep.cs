using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the Blocks side shows this tab.
/// </summary>
internal sealed class ExpectBlocksTabStep : ScenarioStep
{
    public ExpectBlocksTabStep(bool reference)
    {
        Reference = reference;
    }

    public bool Reference { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (context.RequireOpenWindow())
        {
            var expected = Describe(Reference);
            var actual = Describe(context.Window.ShowsReference);
            if (context.Window.ShowsReference == Reference)
                context.Pass($"the {actual} tab shows", expected, actual);
            else
                context.Fail($"the {actual} tab shows instead", expected, actual);
        }

        yield return null;
    }

    private static string Describe(bool reference)
    {
        return reference ? "reference" : "placed";
    }
}
