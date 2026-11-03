using System;
using System.Collections;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Moves the mouse, as <see cref="ImuiMouse"/> hands it to Imui, onto the first character of the first time the text
/// is in the selected Script's or Library's editor, and waits until it has rested there long enough for the hover box.
/// The mouse stays there for the steps after it, until the Scenario ends.
/// </summary>
internal sealed class HoverStep : ScenarioStep
{
    // The hover box needs half a second's rest, so this leaves it plenty
    private const float RestSeconds = 3;

    public HoverStep(string text)
    {
        Text = text;
    }

    public string Text { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        var index = context.Window.Text.IndexOf(Text, StringComparison.Ordinal);
        if (index < 0)
        {
            context.Fail("the editor's text doesn't have it", Text, context.Window.Text);
            yield break;
        }

        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        if (context.Window.FindDrawnCharacter(index) is not { } drawn || drawn.Frame != Time.frameCount)
        {
            context.Fail("the window didn't draw the editor");
            yield break;
        }

        // A quarter of the way across, so the character is the one under the mouse and not the one before it
        var point = new Vector2(drawn.Character.X + (drawn.Character.W / 4), drawn.Character.Y + (drawn.Character.H / 2));
        var where = $"the mouse at x {RectCheck.Pixels(point.x)}, y {RectCheck.Pixels(point.y)}";
        if (!drawn.Visible.Contains(point))
        {
            context.Fail("it isn't in view in the editor", RectCheck.Describe(drawn.Visible), where);
            yield break;
        }

        ImuiMouse.MoveTo(point);

        // The rest starts the frame Imui first finds the mouse there, so one from before can't count
        var movedAt = Time.frameCount;
        yield return context.WaitUntil(() => context.Window.HoverRestingFrame > movedAt, RestSeconds);
        if (context.Window.HoverRestingFrame <= movedAt)
        {
            context.Fail("the mouse didn't rest on the editor, as when something covers it", "the editor hovered", where);
            yield break;
        }

        context.Pass(where, null, null);
    }
}
