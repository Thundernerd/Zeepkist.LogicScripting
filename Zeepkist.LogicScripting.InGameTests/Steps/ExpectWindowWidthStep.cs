using System.Collections;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the window is drawn this wide, or as wide as the screen, to within <see cref="RectCheck.Tolerance"/>.
/// </summary>
internal sealed class ExpectWindowWidthStep : ScenarioStep
{
    public ExpectWindowWidthStep(int? width)
    {
        Width = width;
    }

    /// <summary>
    /// Null for the screen's width.
    /// </summary>
    public int? Width { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        var width = context.Window.WindowRect.W;
        var screenWidth = context.Window.WindowLayout.Screen.W;
        var expectedWidth = Width ?? screenWidth;
        var expected = Width == null ? $"the screen's {RectCheck.Pixels(screenWidth)}" : RectCheck.Pixels(expectedWidth);
        var actual = RectCheck.Pixels(width);
        var detail = $"{actual} px wide, on a screen {RectCheck.Pixels(screenWidth)} px wide";
        if (Mathf.Abs(width - expectedWidth) <= RectCheck.Tolerance)
            context.Pass(detail, expected, actual);
        else
            context.Fail(detail, expected, actual);
    }
}
