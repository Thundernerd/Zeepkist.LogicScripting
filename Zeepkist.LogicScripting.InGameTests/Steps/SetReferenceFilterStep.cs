using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Replaces the text in the Reference tab's filter, as typing it does. Empty text clears it. The tab has to be on
/// screen, as typing can't reach it otherwise.
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
        if (!context.RequireOpenWindow())
            yield break;

        yield return context.WaitForReferenceTabDrawn();
        if (context.Failed)
            yield break;

        context.Window.SetReferenceFilter(Text);
        yield return null;
    }
}
