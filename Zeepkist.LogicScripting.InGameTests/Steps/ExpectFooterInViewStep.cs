using System.Collections;
using TNRD.Zeepkist.LogicScripting.UI;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the shown tab's footer, the open prompt, the result line and New, lies inside the tab, inside the
/// window, with the list above it at least two rows tall. The prompts may scroll when there's no room for all of them,
/// but get at least a row.
/// </summary>
internal sealed class ExpectFooterInViewStep : ScenarioStep
{
    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        // The footer sizes itself from the height its prompts took the frame before, so a prompt just opened settles
        // a frame after it's first drawn
        yield return null;
        yield return null;
        yield return null;

        var list = context.Window.ShownList;
        var layout = list.FooterLayout;
        var window = context.Window.WindowRect;
        var hasPrompts = list.OpenPrompt != ListPrompt.None || list.Message.Length > 0;
        if (!RectCheck.Contains(window, layout.Tab))
            context.Fail("the tab reaches outside the window", RectCheck.Describe(window), RectCheck.Describe(layout.Tab));
        else if (!RectCheck.Contains(layout.Tab, layout.NewRow))
            context.Fail("New reaches outside the tab", RectCheck.Describe(layout.Tab), RectCheck.Describe(layout.NewRow));
        else if (layout.List.H < layout.MinListHeight - RectCheck.Tolerance)
            context.Fail("the list is less than two rows tall", $"{RectCheck.Pixels(layout.MinListHeight)} px or more", $"{RectCheck.Pixels(layout.List.H)} px");
        else if (hasPrompts && layout.PromptsHeight <= 0)
            context.Fail("the prompt or result line wasn't drawn");
        else if (hasPrompts && !RectCheck.Contains(layout.Tab, layout.Prompts))
            context.Fail("the prompt or result line reaches outside the tab", RectCheck.Describe(layout.Tab), RectCheck.Describe(layout.Prompts));
        else if (hasPrompts && layout.Prompts.H < layout.RowHeight - RectCheck.Tolerance)
            context.Fail("the prompt and result line get less than a row", $"{RectCheck.Pixels(layout.RowHeight)} px or more", $"{RectCheck.Pixels(layout.Prompts.H)} px");
        else
            context.Pass(Summarize(layout, hasPrompts), null, null);
    }

    private static string Summarize(ListFooterLayout layout, bool hasPrompts)
    {
        var listed = $"the list is {RectCheck.Pixels(layout.List.H)} px tall";
        if (!hasPrompts)
            return $"{listed}, with no prompt or result line";

        return layout.Prompts.H >= layout.PromptsHeight - RectCheck.Tolerance
            ? $"{listed}, and the prompt and result line show all {RectCheck.Pixels(layout.PromptsHeight)} px"
            : $"{listed}, and the prompt and result line show {RectCheck.Pixels(layout.Prompts.H)} of {RectCheck.Pixels(layout.PromptsHeight)} px, the rest scrolls";
    }
}
