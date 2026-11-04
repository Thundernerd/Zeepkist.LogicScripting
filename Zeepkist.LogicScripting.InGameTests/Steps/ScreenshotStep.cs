using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Fails on a blank capture, as a Scenario takes a screenshot to show what the game drew.
/// </summary>
internal sealed class ScreenshotStep : ScenarioStep
{
    public ScreenshotStep(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override IEnumerator Run(StepContext context)
    {
        yield return context.Screenshot(Name);
        if (context.LastBlankColor is { } color)
            context.Fail($"the screenshot is blank: every pixel is {color}", "more than one color", $"every pixel {color}");
    }
}
