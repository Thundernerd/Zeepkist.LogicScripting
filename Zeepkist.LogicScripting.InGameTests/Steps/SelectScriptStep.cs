using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Clicks the Script in the list, which selects it while the window draws the list, as a player's click does.
/// </summary>
internal sealed class SelectScriptStep : ScenarioStep
{
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

        yield return context.WaitForShownList("the list wasn't drawn to take the click: is the Scripts side collapsed?");
    }
}
