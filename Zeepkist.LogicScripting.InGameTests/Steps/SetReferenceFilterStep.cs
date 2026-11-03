using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Replaces the text in the Reference tab's filter, as typing it does. Empty text clears it.
/// </summary>
internal sealed class SetReferenceFilterStep : ScenarioStep
{
    public SetReferenceFilterStep(string text)
    {
        Text = text;
    }

    public string Text { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (context.RequireOpenWindow() && !context.Window.SetReferenceFilter(Text))
            context.Fail("the Reference tab isn't shown: add show-blocks-tab reference first", "reference", "placed");

        yield return null;
    }
}
