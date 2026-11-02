using System;
using System.Collections;
using TNRD.Zeepkist.LogicScripting.UI;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// For the steps that check whether the window drew a warning, in the frame it was last drawn.
/// </summary>
internal static class WarningCheck
{
    /// <summary>
    /// Reads whether the window drew the warning from <paramref name="layout"/>, failing with
    /// <paramref name="notDrawn"/> when the part of the window it goes in wasn't drawn that frame.
    /// </summary>
    public static IEnumerator Run(StepContext context, bool shown, string warning, Func<ScriptWindow.Driver, WarningLayout> layout, string notDrawn)
    {
        if (!context.RequireOpenWindow())
            yield break;

        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        var drawn = layout(context.Window);
        if (drawn.Frame != Time.frameCount)
        {
            context.Fail(notDrawn, $"Unity frame {Time.frameCount}", $"Unity frame {drawn.Frame}");
            yield break;
        }

        var expected = Describe(shown);
        var actual = Describe(drawn.Shown);
        if (drawn.Shown == shown)
            context.Pass($"{warning}: {Shown(shown)}", expected, actual);
        else
            context.Fail($"{warning}: {Shown(drawn.Shown)}", expected, actual);
    }

    private static string Describe(bool shown)
    {
        return shown ? "on" : "off";
    }

    private static string Shown(bool shown)
    {
        return shown ? "shown" : "not shown";
    }
}
