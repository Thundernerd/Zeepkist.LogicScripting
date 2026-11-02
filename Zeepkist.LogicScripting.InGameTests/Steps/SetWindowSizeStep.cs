using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

internal sealed class SetWindowSizeStep : ScenarioStep
{
    public SetWindowSizeStep((int Width, int Height) size)
    {
        (Width, Height) = size;
    }

    public int Width { get; }
    public int Height { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        context.Window.SetWindowSize(Width, Height);
        yield return context.WaitForWindowRect();
        if (context.Failed)
            yield break;

        // A size past the window's minimum or the screen gives the nearest the window can be
        var rect = context.Window.WindowRect;
        context.Pass(RectCheck.Describe(rect), RectCheck.Size(Width, Height), RectCheck.Size(rect.W, rect.H));
    }
}
