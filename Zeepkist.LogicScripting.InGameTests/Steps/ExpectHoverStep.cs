using System;
using System.Collections;
using System.Collections.Generic;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the hover box shows and its text, as it's drawn, contains the text, matching case. A row's name and
/// value are one line, a space between, and the lines are separated by <c>|</c>.
/// </summary>
internal sealed class ExpectHoverStep : ScenarioStep
{
    /// <summary>
    /// What a hover step reports as found when the hover box doesn't show.
    /// </summary>
    public const string NoHover = "(no hover box)";

    private const float WaitSeconds = 1;

    public ExpectHoverStep(string text)
    {
        Text = text;
    }

    public string Text { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        yield return context.WaitUntil(() => Contains(context.Window.HoverLines), WaitSeconds);

        var lines = context.Window.HoverLines;
        var actual = Describe(lines);
        if (Contains(lines))
            context.Pass(actual, Text, actual);
        else
            context.Fail(lines == null ? "the hover box doesn't show" : "the hover box doesn't show it", Text, actual);
    }

    public static string Describe(IReadOnlyList<string> lines)
    {
        return lines == null ? NoHover : string.Join(" | ", lines);
    }

    private bool Contains(IReadOnlyList<string> lines)
    {
        return lines != null && Describe(lines).IndexOf(Text, StringComparison.Ordinal) >= 0;
    }
}
