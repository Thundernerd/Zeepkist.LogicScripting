using System.Collections;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the window is drawn this big, to within <see cref="RectCheck.Tolerance"/>.
/// </summary>
internal sealed class ExpectWindowSizeStep : ScenarioStep
{
    public ExpectWindowSizeStep((int Width, int Height) size)
    {
        (Width, Height) = size;
    }

    public int Width { get; }
    public int Height { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        var rect = context.Window.WindowRect;
        var expected = RectCheck.Size(Width, Height);
        var actual = RectCheck.Size(rect.W, rect.H);
        if (Mathf.Abs(rect.W - Width) <= RectCheck.Tolerance && Mathf.Abs(rect.H - Height) <= RectCheck.Tolerance)
            context.Pass(RectCheck.Describe(rect), expected, actual);
        else
            context.Fail(RectCheck.Describe(rect), expected, actual);
    }
}
