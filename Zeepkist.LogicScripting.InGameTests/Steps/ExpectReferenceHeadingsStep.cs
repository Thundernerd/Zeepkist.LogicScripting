using System.Collections;
using System.Collections.Generic;
using TNRD.Zeepkist.LogicScripting.UI;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Passes when the Reference tab draws exactly these headings, in this order, each open or closed as given.
/// </summary>
internal sealed class ExpectReferenceHeadingsStep : ScenarioStep
{
    public ExpectReferenceHeadingsStep(IReadOnlyList<ReferenceHeading> headings)
    {
        Headings = headings;
    }

    /// <summary>
    /// Empty for none, as when nothing matches the filter.
    /// </summary>
    public IReadOnlyList<ReferenceHeading> Headings { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireOpenWindow())
            yield break;

        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        var drawn = context.Window.ReferenceDrawnFrame;
        if (drawn != Time.frameCount)
        {
            context.Fail(
                "the Reference tab wasn't drawn: add show-blocks-tab reference first, with the Blocks side shown",
                $"Unity frame {Time.frameCount}",
                $"Unity frame {drawn}");
            yield break;
        }

        var expected = Describe(Headings);
        var actual = Describe(context.Window.ReferenceHeadings);
        if (expected == actual)
            context.Pass($"the Reference tab draws {actual}", expected, actual);
        else
            context.Fail($"the Reference tab draws {actual} instead", expected, actual);
    }

    private static string Describe(IReadOnlyList<ReferenceHeading> headings)
    {
        return headings.Count == 0 ? StepList.NoHeadings : ReferenceHeading.Describe(headings);
    }
}
