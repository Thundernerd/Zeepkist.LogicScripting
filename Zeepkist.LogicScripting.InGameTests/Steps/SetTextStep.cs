using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Writes <c>{uid:&lt;name&gt;}</c> as the UID of the block <c>place-block ... as &lt;name&gt;</c> placed, so a Script
/// can refer to it inline.
/// </summary>
internal sealed class SetTextStep : ScenarioStep
{
    public SetTextStep(string text)
    {
        Text = text;
    }

    public string Text { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        var text = context.PlacedBlocks.Expand(Text, out var unknown);
        if (text == null)
        {
            context.Fail($"no block placed or named as \"{unknown}\": add place-block <function> as {unknown} or name-group-block first", unknown, context.PlacedBlocks.Listed);
            yield break;
        }

        context.Window.SetText(text);
        yield return null;
    }
}
