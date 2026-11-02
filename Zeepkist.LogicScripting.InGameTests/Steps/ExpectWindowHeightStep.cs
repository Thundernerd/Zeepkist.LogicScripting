using System.Collections;
using System.Globalization;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the window is drawn this tall, to within <see cref="RectCheck.Tolerance"/>.
/// </summary>
internal sealed class ExpectWindowHeightStep : ScenarioStep
{
    public ExpectWindowHeightStep(int height)
    {
        Height = height;
    }

    public int Height { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        var height = context.Window.WindowRect.H;
        var expected = Height.ToString(CultureInfo.InvariantCulture);
        var actual = height.ToString("0.#", CultureInfo.InvariantCulture);
        if (Mathf.Abs(height - Height) <= RectCheck.Tolerance)
            context.Pass($"{actual} px tall", expected, actual);
        else
            context.Fail($"the window is {actual} px tall", expected, actual);
    }
}
