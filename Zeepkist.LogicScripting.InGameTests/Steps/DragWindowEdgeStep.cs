using System;
using System.Collections;
using Imui.Core;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Drags the window's resize handle as a player's mouse does: the mouse moves onto it, presses, moves by an even part
/// of the drag each frame, down or up, or right or left when <see cref="Sideways"/>, and lets go. Every frame from the
/// press runs the checks of <see cref="ExpectEditorInViewStep"/> and checks the window is no smaller than its minimum,
/// failing with the first frame that's wrong.
/// </summary>
internal sealed class DragWindowEdgeStep : ScenarioStep
{
    public DragWindowEdgeStep(int pixels, int frames, bool sideways)
    {
        Pixels = pixels;
        Frames = frames;
        Sideways = sideways;
    }

    /// <summary>
    /// How far the mouse moves down, or up when below 0, or right, or left, when <see cref="Sideways"/>.
    /// </summary>
    public int Pixels { get; }

    public int Frames { get; }

    public bool Sideways { get; }

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        var window = context.Window;
        var startSize = GetSize(window.WindowRect);
        var smallest = startSize;
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

                smallest = Mathf.Min(smallest, GetSize(window.WindowRect));
                if (frame == Frames)
                    break;

                if (!window.WindowLayout.IsResizing)
                {
                    context.Fail($"frame {frame}: the resize handle let go of the mouse");
                    yield break;
                }

                var move = GetMove(frame + 1);

                // Imui counts pixels up the screen, as Unity does, so a move down is below 0
                if (!ImuiMouse.Drag(Sideways ? new Vector2(move, 0) : new Vector2(0, -move)))
                {
                    context.Fail($"frame {frame}: Imui has no backend to send the mouse to");
                    yield break;
                }

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

        // A frame for Imui to take the release, drawn so the window says whether the handle let go
        yield return null;
        yield return context.WaitForWindowDrawn();
        if (context.Failed)
            yield break;

        if (window.WindowLayout.IsResizing)
        {
            context.Fail("the resize handle kept the mouse after letting go");
            yield break;
        }

        var (size, least) = Sideways ? ("wide", "narrowest") : ("tall", "shortest");
        var minSize = Sideways ? window.WindowLayout.MinWidth : window.WindowLayout.MinHeight;
        context.Pass(
            $"{Frames + 1} frames in view, from {RectCheck.Pixels(startSize)} px {size} to {RectCheck.Pixels(GetSize(window.WindowRect))} px, {RectCheck.Pixels(smallest)} px at the {least}, its minimum {RectCheck.Pixels(minSize)} px",
            null,
            null);
    }

    /// <summary>
    /// The window's width when <see cref="Sideways"/>, otherwise its height.
    /// </summary>
    private float GetSize(ImRect rect)
    {
        return Sideways ? rect.W : rect.H;
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
        var minWidth = window.WindowLayout.MinWidth;
        if (rect.W < minWidth - RectCheck.Tolerance)
        {
            context.Fail($"frame {frame}: the window is narrower than its minimum", $"{RectCheck.Pixels(minWidth)} px or more", $"{RectCheck.Pixels(rect.W)} px");
            return false;
        }

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
