using System.Collections;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Replaces the text in the Reference tab's filter, as typing it does. Empty text clears it. The tab has to be on
/// screen, as typing can't reach it otherwise.
/// </summary>
internal sealed class SetReferenceFilterStep : ScenarioStep
{
    public SetReferenceFilterStep(string text)
    {
        Text = text;
    }

    public string Text { get; }

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

        context.Window.SetReferenceFilter(Text);
        yield return null;
    }
}
