using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Clicks the Script in the list, which selects it while the window draws the list, as a player's click does.
/// </summary>
internal sealed class SelectScriptStep : ScenarioStep
{
    private const float DrawSeconds = 2;

    public SelectScriptStep(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        if (!context.Window.SelectScript(Name))
        {
            context.FailNoScript(Name);
            yield break;
        }

        var list = context.Window.ShownList;
        yield return context.WaitUntil(() => !list.IsClickPending, DrawSeconds);
        if (list.IsClickPending)
            context.Fail("the list wasn't drawn to take the click");
    }
}
