using System;
using System.Collections;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Drags the window's resize handle as a player's mouse does: the mouse moves onto it, presses, moves by an even part
/// of the drag each frame and lets go. Every frame from the press runs the checks of <see cref="ExpectEditorInViewStep"/>
/// and checks the window is no shorter than its minimum, failing with the first frame that's wrong.
/// </summary>
internal sealed class DragWindowEdgeStep : ScenarioStep
{
    public DragWindowEdgeStep(int pixels, int frames)
    {
        Pixels = pixels;
        Frames = frames;
    }

    /// <summary>
    /// How far the mouse moves down, or up when below 0.
    /// </summary>
    public int Pixels { get; }

    public int Frames { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        var window = context.Window;
        var startHeight = window.WindowRect.H;
        var shortest = startHeight;
        var pressed = false;
        try
        {
            ImuiMouse.MoveTo(window.WindowLayout.ResizeHandle.Center);

            // Imui finds the hovered control as it draws, for the frame after
            yield return null;
            if (!ImuiMouse.Press())
            {
                context.Fail("Imui has no backend to send the mouse to");
                yield break;
            }

            pressed = true;
            yield return null;
            if (!window.WindowLayout.IsResizing)
            {
                context.Fail("frame 0: the resize handle didn't take the press", RectCheck.Describe(window.WindowLayout.ResizeHandle), $"the mouse at {Describe(ImuiMouse.Position.Value)}");
                yield break;
            }

            for (var frame = 0; ; frame++)
            {
                if (!Check(context, frame))
                    yield break;

                shortest = Mathf.Min(shortest, window.WindowRect.H);
                if (frame == Frames)
                    break;

                if (!window.WindowLayout.IsResizing)
                {
                    context.Fail($"frame {frame}: the resize handle let go of the mouse");
                    yield break;
                }

                // Imui counts pixels up the screen, as Unity does
                ImuiMouse.Drag(new Vector2(0, -GetMove(frame + 1)));
                yield return null;
            }
        }
        finally
        {
            // Also when the step fails or throws, so the next step finds the mouse let go and the player's again
            if (pressed)
                ImuiMouse.Release();

            ImuiMouse.GiveBack();
        }

        yield return null;
        if (window.WindowLayout.IsResizing)
        {
            context.Fail("the resize handle kept the mouse after letting go");
            yield break;
        }

        var height = RectCheck.Pixels(window.WindowRect.H);
        context.Pass(
            $"{Frames + 1} frames in view, from {RectCheck.Pixels(startHeight)} px tall to {height} px, {RectCheck.Pixels(shortest)} px at the shortest, its minimum {RectCheck.Pixels(window.WindowLayout.MinHeight)} px",
            null,
            null);
    }

    /// <summary>
    /// The part of the drag the mouse moves in <paramref name="frame"/>, from 1, in whole pixels that add up to all of it.
    /// </summary>
    private float GetMove(int frame)
    {
        return Mathf.Round((float)Pixels * frame / Frames) - Mathf.Round((float)Pixels * (frame - 1) / Frames);
    }

    /// <summary>
    /// Frame 0 is the press, and each one after it a move of the mouse.
    /// </summary>
    private static bool Check(StepContext context, int frame)
    {
        var window = context.Window;
        var layout = window.EditorLayout;
        if (layout.Frame != Time.frameCount || window.WindowLayout.Frame != Time.frameCount)
        {
            context.Fail($"frame {frame}: the window didn't draw the editor", $"Unity frame {Time.frameCount}", $"Unity frame {Math.Min(layout.Frame, window.WindowLayout.Frame)}");
            return false;
        }

        var rect = window.WindowRect;
        var minHeight = window.WindowLayout.MinHeight;
        if (rect.H < minHeight - RectCheck.Tolerance)
        {
            context.Fail($"frame {frame}: the window is shorter than its minimum", $"{RectCheck.Pixels(minHeight)} px or more", $"{RectCheck.Pixels(rect.H)} px");
            return false;
        }

        var check = EditorLayoutCheck.Run(layout, rect);
        if (!check.Passed)
        {
            context.Fail($"frame {frame}: {check.Failure}", check.Expected, check.Actual);
            return false;
        }

        return true;
    }

    private static string Describe(Vector2 position)
    {
        return $"x {RectCheck.Pixels(position.x)}, y {RectCheck.Pixels(position.y)}";
    }
}
