using System.Collections;
using System.Linq;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the completion list is open and has no row with the label. A closed list fails, as it does for
/// <see cref="ExpectCompletionStep"/>, so a list that didn't open can't pass for one that left the row out.
/// </summary>
internal sealed class ExpectCompletionMissingStep : ScenarioStep
{
    private const float WaitSeconds = 1;

    public ExpectCompletionMissingStep(string label)
    {
        Label = label;
    }

    public string Label { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        yield return context.WaitUntil(() => context.Window.CompletionItems.Count > 0, WaitSeconds);

        var expected = $"no {Label}";
        var items = context.Window.CompletionItems;
        if (items.Count == 0)
        {
            context.Fail("the completion list is closed", expected, "(the list is closed)");
            yield break;
        }

        var actual = string.Join(", ", items.Select(i => ExpectCompletionStep.Describe(i.Label, i.Detail)));
        if (items.Any(i => i.Label == Label))
            context.Fail("the completion list has the row", expected, actual);
        else
            context.Pass(actual, expected, actual);
    }
}
