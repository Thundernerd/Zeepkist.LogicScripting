using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Runs the checks of <see cref="ExpectEditorInViewStep"/> without waiting for the window to settle: on the frame the
/// step before ended on, frame 0, and on each of the next frames the window draws, so it catches a single bad frame.
/// </summary>
internal sealed class ExpectEditorInViewEachFrameStep : ScenarioStep
{
    private const float DrawSeconds = 2;

    public ExpectEditorInViewEachFrameStep(int frames)
    {
        Frames = frames;
    }

    public int Frames { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        // Steps run after the window draws in the same frame, and the next step starts in the frame the last one ended
        var layout = context.Window.EditorLayout;
        if (layout.Frame != Time.frameCount)
        {
            context.Fail("frame 0: the window didn't draw the editor", $"Unity frame {Time.frameCount}", $"Unity frame {layout.Frame}");
            yield break;
        }

        var warned = new List<int>();
        for (var frame = 0; ; frame++)
        {
            var check = EditorLayoutCheck.Run(layout, context.Window.WindowRect);
            var warning = layout.HasStorageWarning ? ", with the storage warning" : string.Empty;
            if (check.Failure != null)
            {
                context.Fail($"frame {frame}{warning}: {check.Failure}", check.Expected, check.Actual);
                yield break;
            }

            if (layout.HasStorageWarning)
                warned.Add(frame);

            if (frame == Frames)
                break;

            var last = layout.Frame;
            yield return context.WaitUntil(() => context.Window.EditorLayout.Frame != last, DrawSeconds);
            layout = context.Window.EditorLayout;
            if (layout.Frame != last + 1)
            {
                context.Fail($"frame {frame + 1}: the window didn't draw the editor", $"Unity frame {last + 1}", layout.Frame == last ? "none" : $"Unity frame {layout.Frame}");
                yield break;
            }
        }

        context.Pass($"{Frames + 1} frames in view, {DescribeWarned(warned)}", null, null);
    }

    private static string DescribeWarned(List<int> warned)
    {
        if (warned.Count == 0)
            return "none with the storage warning";

        if (warned.Count == 1)
            return $"frame {warned[0]} with the storage warning";

        var contiguous = warned.Last() - warned.First() == warned.Count - 1;
        return contiguous
            ? $"frames {warned.First()} to {warned.Last()} with the storage warning"
            : $"frames {string.Join(", ", warned)} with the storage warning";
    }
}
