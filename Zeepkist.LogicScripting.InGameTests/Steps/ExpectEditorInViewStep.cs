using System.Collections;
using System.Globalization;
using Imui.Core;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the Script editor is at least three rows tall, and it and the error panel's header lie inside the
/// center, inside the window.
/// </summary>
internal sealed class ExpectEditorInViewStep : ScenarioStep
{
    // Rects are laid out in fractions of a pixel
    private const float Tolerance = 0.5f;

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        // The window takes its minimum height a frame after what it counts changed, so the center settles after that
        yield return null;
        yield return null;
        yield return null;

        var layout = context.Window.EditorLayout;
        var window = context.Window.WindowRect;
        if (!Contains(window, layout.Center))
            context.Fail("the center reaches outside the window", Describe(window), Describe(layout.Center));
        else if (layout.Field.H < layout.MinFieldHeight - Tolerance)
            context.Fail("the editor is less than three rows tall", $"{Pixels(layout.MinFieldHeight)} px or more", $"{Pixels(layout.Field.H)} px");
        else if (!Contains(layout.Center, layout.Field))
            context.Fail("the editor reaches outside the center", Describe(layout.Center), Describe(layout.Field));
        else if (!Contains(layout.Center, layout.ErrorsHeader))
            context.Fail("the error panel's header reaches outside the center", Describe(layout.Center), Describe(layout.ErrorsHeader));
        else
            context.Pass($"the editor is {Pixels(layout.Field.H)} px tall, with the error panel's header under it", null, null);
    }

    private static bool Contains(ImRect outer, ImRect inner)
    {
        return inner.H >= 0
            && inner.Left >= outer.Left - Tolerance
            && inner.Right <= outer.Right + Tolerance
            && inner.Bottom >= outer.Bottom - Tolerance
            && inner.Top <= outer.Top + Tolerance;
    }

    private static string Describe(ImRect rect)
    {
        return $"x {Pixels(rect.X)}, y {Pixels(rect.Y)}, {Pixels(rect.W)} by {Pixels(rect.H)} px";
    }

    private static string Pixels(float pixels)
    {
        return pixels.ToString("0", CultureInfo.InvariantCulture);
    }
}
