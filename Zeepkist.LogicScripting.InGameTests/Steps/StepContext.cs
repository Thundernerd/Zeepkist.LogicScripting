using System;
using System.Collections;
using System.Linq;
using BepInEx.Logging;
using Imui.Core;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;
using TNRD.Zeepkist.LogicScripting.UI;
using UnityEngine;
using ZeepSDK.Utilities;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// What a step works with, and where it leaves its outcome. One per scenario, reset before each step.
/// </summary>
internal sealed class StepContext
{
    private const float ListDrawSeconds = 2;
    private const float WindowDrawSeconds = 2;

    private readonly Func<string, IEnumerator> _screenshot;
    private readonly Func<ImRect?> _startWindowRect;

    public StepContext(
        string scenario,
        ScriptWindow.Driver window,
        GameEvents game,
        ExceptionLog exceptions,
        ManualLogSource logger,
        Func<string, IEnumerator> screenshot,
        Func<ImRect?> startWindowRect)
    {
        Scenario = scenario;
        Window = window;
        Game = game;
        Exceptions = exceptions;
        Logger = logger;
        _screenshot = screenshot;
        _startWindowRect = startWindowRect;
    }

    public string Scenario { get; }
    public ScriptWindow.Driver Window { get; }
    public GameEvents Game { get; }
    public ExceptionLog Exceptions { get; }
    public ManualLogSource Logger { get; }

    /// <summary>
    /// They last the scenario.
    /// </summary>
    public PlacedBlockNames PlacedBlocks { get; } = new();

    /// <summary>
    /// What remember-window-rect remembered, for the rest of the scenario, or null.
    /// </summary>
    public ImRect? RememberedWindowRect { get; set; }

    /// <summary>
    /// The window's rect the first time a Scenario of the run drew it, which the run puts back before each Scenario after
    /// the first, or null while none has.
    /// </summary>
    public ImRect? StartWindowRect => _startWindowRect();

    public bool Failed { get; private set; }

    /// <summary>
    /// What the step found, such as a result line, shown whether it passed or not.
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    public string Expected { get; private set; }
    public string Actual { get; private set; }

    public void Reset()
    {
        Failed = false;
        Detail = string.Empty;
        Expected = null;
        Actual = null;
    }

    /// <summary>
    /// For a check that passed, so the results show what it compared.
    /// </summary>
    public void Pass(string detail, string expected, string actual)
    {
        Detail = detail;
        Expected = expected;
        Actual = actual;
    }

    public void Fail(string detail, string expected = null, string actual = null)
    {
        Failed = true;
        Detail = detail;
        Expected = expected;
        Actual = actual;
    }

    public void FailNoScript(string name)
    {
        Fail($"no Script named \"{name}\"", name, string.Join(", ", Window.ScriptNames));
    }

    /// <summary>
    /// For a step on an entry of the shown tab's list that has none by the name.
    /// </summary>
    public void FailNoEntry(string name)
    {
        var list = Window.ShownList;
        Fail($"no {list.Noun} named \"{name}\" on the shown tab", name, string.Join(", ", list.Names));
    }

    /// <summary>
    /// For a step that remembers a block by a name another step already placed or named a block as.
    /// </summary>
    public void FailNameTaken(string name)
    {
        Fail($"a block was already placed or named as \"{name}\"", "a new name", PlacedBlocks.Listed);
    }

    /// <summary>
    /// The Logic Block type with the Block Function's name, or null after failing the step when there's none.
    /// </summary>
    public BlockType RequireBlockType(string function)
    {
        var type = BlockManifest.All.FirstOrDefault(t => t.Function == function);
        if (type == null)
            Fail($"no Logic Block type is called '{function}'");

        return type;
    }

    /// <summary>
    /// Saved as <c>&lt;scenario&gt;-&lt;name&gt;.png</c> in the run's results.
    /// </summary>
    public IEnumerator Screenshot(string name)
    {
        return _screenshot($"{Scenario}-{name}");
    }

    /// <summary>
    /// Waits until <paramref name="condition"/> holds or <paramref name="seconds"/> pass, whichever is first, so the
    /// step checks again afterwards.
    /// </summary>
    public IEnumerator WaitUntil(Func<bool> condition, float seconds)
    {
        var giveUpAt = Time.realtimeSinceStartup + seconds;
        while (!condition() && Time.realtimeSinceStartup < giveUpAt)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Waits for the shown tab's list to take the button or click handed to it, the next time the window draws it. When
    /// it isn't drawn in time, such as with the Scripts side collapsed, fails the step and drops the input, so it can't
    /// land in a later step.
    /// </summary>
    public IEnumerator WaitForShownList(string failure)
    {
        var list = Window.ShownList;
        yield return WaitUntil(() => !list.IsInputPending, ListDrawSeconds);
        if (list.IsInputPending)
        {
            list.CancelInput();
            Fail(failure);
        }
    }

    /// <summary>
    /// Waits for the window to be drawn, unless it was this frame, so its rect is the one Imui keeps now and not the one
    /// it had before it closed. Fails the step when it isn't drawn in time.
    /// </summary>
    public IEnumerator WaitForWindowDrawn()
    {
        yield return WaitUntil(() => Window.WindowLayout.Frame == Time.frameCount, WindowDrawSeconds);
        if (Window.WindowLayout.Frame != Time.frameCount)
            Fail("the window wasn't drawn", $"Unity frame {Time.frameCount}", $"Unity frame {Window.WindowLayout.Frame}");
    }

    /// <summary>
    /// Waits for the window to be drawn, as <see cref="WaitForWindowDrawn"/> does, and fails the step when that frame
    /// didn't draw the Reference tab: with the Placed blocks tab shown or the Blocks side collapsed.
    /// </summary>
    public IEnumerator WaitForReferenceTabDrawn()
    {
        yield return WaitForWindowDrawn();
        if (Failed)
            yield break;

        var drawn = Window.ReferenceDrawnFrame;
        if (drawn != Time.frameCount)
        {
            Fail(
                "the Reference tab wasn't drawn: add show-blocks-tab reference first, with the Blocks side shown",
                $"Unity frame {Time.frameCount}",
                $"Unity frame {drawn}");
        }
    }

    /// <summary>
    /// For a step that checks the window is drawn at <paramref name="expected"/>, so its result has both rects.
    /// </summary>
    public IEnumerator ExpectWindowRect(ImRect expected, string passDetail, string failDetail)
    {
        yield return WaitForWindowDrawn();
        if (Failed)
            yield break;

        var rect = Window.WindowRect;
        if (RectCheck.Matches(expected, rect))
            Pass(passDetail, RectCheck.Describe(expected), RectCheck.Describe(rect));
        else
            Fail(failDetail, RectCheck.Describe(expected), RectCheck.Describe(rect));
    }

    /// <summary>
    /// Waits for the window to take the size or position a step asked for, which it does as it's next drawn, and to lay
    /// itself out in it. Fails the step when it isn't drawn in time.
    /// </summary>
    public IEnumerator WaitForWindowRect()
    {
        yield return WaitUntil(() => !Window.IsRectPending, WindowDrawSeconds);
        if (Window.IsRectPending)
        {
            Fail("the window wasn't drawn to take the size or position");
            yield break;
        }

        // A frame to lay the window out in its new rect, and one for the footer, which sizes itself from the last
        yield return null;
        yield return null;
    }

    /// <summary>
    /// The level editor, or null after failing the step when the game isn't in it.
    /// </summary>
    public LEV_LevelEditorCentral RequireEditor()
    {
        var central = ComponentCache.Get<LEV_LevelEditorCentral>();
        if (central == null)
            Fail("not in the level editor");

        return central;
    }

    /// <summary>
    /// Fails the step when the window isn't open, as a player can't use its buttons then. Returns whether it is.
    /// </summary>
    public bool RequireOpenWindow()
    {
        if (!Window.IsOpen)
            Fail("the window isn't open: add open-window first");

        return Window.IsOpen;
    }

    /// <summary>
    /// Fails the step when no field in the window has focus, as keys typed then go to the level editor. Returns whether
    /// one has.
    /// </summary>
    public bool RequireFocusedField()
    {
        if (!RequireOpenWindow())
            return false;

        if (!Window.IsFieldFocused)
            Fail("no field is focused: add caret-at-start or caret-at-end first");

        return Window.IsFieldFocused;
    }

    /// <summary>
    /// Like <see cref="RequireOpenWindow"/>, with a Script selected too.
    /// </summary>
    public bool RequireScript()
    {
        if (!RequireOpenWindow())
            return false;

        if (!Window.HasScript)
            Fail("no Script selected: add new-script or select-script first");

        return Window.HasScript;
    }

    /// <summary>
    /// Like <see cref="RequireOpenWindow"/>, with a Script or a Library selected too, for a step on the editor's text.
    /// </summary>
    public bool RequireText()
    {
        if (!RequireOpenWindow())
            return false;

        if (!Window.HasText)
            Fail("no Script or Library selected: add new-script, select-script, new-library or select-library first");

        return Window.HasText;
    }
}
