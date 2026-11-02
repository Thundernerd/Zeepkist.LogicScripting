using System.Collections;
using System.Linq;
using TNRD.Zeepkist.LogicScripting.Compiler.Editing;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the open completion list has a row with the label, and with the detail too when one is given: empty
/// for a row with none.
/// </summary>
internal sealed class ExpectCompletionStep : ScenarioStep
{
    private const float WaitSeconds = 1;

    public ExpectCompletionStep(string label, string detail)
    {
        Label = label;
        Detail = detail;
    }

    public string Label { get; }

    /// <summary>
    /// Null to accept any detail, empty for none.
    /// </summary>
    public string Detail { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        yield return context.WaitUntil(() => context.Window.CompletionItems.Any(Matches), WaitSeconds);

        var expected = Detail == null ? Label : Detail.Length == 0 ? $"{Label} (no detail)" : CompletionRows.Describe(Label, Detail);
        var items = context.Window.CompletionItems;
        var actual = CompletionRows.Describe(items);
        if (items.Any(Matches))
            context.Pass(actual, expected, actual);
        else
            context.Fail("the completion list has no such row", expected, actual);
    }

    private bool Matches(CompletionItem item)
    {
        return item.Label == Label && (Detail == null || item.Detail == Detail);
    }
}
