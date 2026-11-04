using System.Collections;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Covers the frame in one color and takes the screenshot, to check that the run finds a blank one.
/// </summary>
internal sealed class ExpectBlankScreenshotStep : ScenarioStep
{
    public ExpectBlankScreenshotStep(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override IEnumerator Run(StepContext context)
    {
        yield return context.Screenshot(Name, Color.magenta);
        if (context.LastBlankColor is { } color)
            context.Pass($"every pixel is {color}", "blank", "blank");
        else
            context.Fail("the screen was covered in one color, but the screenshot isn't blank", "blank", "more than one color");
    }
}
