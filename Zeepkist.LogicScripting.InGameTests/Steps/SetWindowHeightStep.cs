using System.Collections;
using System.Globalization;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

internal sealed class SetWindowHeightStep : ScenarioStep
{
    public SetWindowHeightStep(int height)
    {
        Height = height;
    }

    public int Height { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        context.Window.SetWindowHeight(Height);
        yield return context.WaitForWindowRect();
        if (context.Failed)
            yield break;

        // A height below the window's minimum gives the smallest window
        var actual = context.Window.WindowRect.H.ToString("0", CultureInfo.InvariantCulture);
        context.Pass($"{actual} px tall", Height.ToString(CultureInfo.InvariantCulture), actual);
    }
}
