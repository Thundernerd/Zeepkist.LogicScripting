using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the Scripts side shows this tab.
/// </summary>
internal sealed class ExpectTabStep : ScenarioStep
{
    public ExpectTabStep(bool libraries)
    {
        Libraries = libraries;
    }

    public bool Libraries { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (context.RequireOpenWindow())
        {
            var expected = Describe(Libraries);
            var actual = Describe(context.Window.ShowsLibraries);
            if (context.Window.ShowsLibraries == Libraries)
                context.Pass($"the {actual} tab shows", expected, actual);
            else
                context.Fail($"the {actual} tab shows", expected, actual);
        }

        yield return null;
    }

    private static string Describe(bool libraries)
    {
        return libraries ? "libraries" : "scripts";
    }
}
