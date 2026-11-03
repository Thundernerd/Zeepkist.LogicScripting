using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TNRD.Zeepkist.LogicScripting.Compiler;
using TNRD.Zeepkist.LogicScripting.InGameTests.Steps;
using TNRD.Zeepkist.LogicScripting.UI;

namespace TNRD.Zeepkist.LogicScripting.InGameTests;

/// <summary>
/// Every step a scenario can use, by the name it's written with. A new step is a class in Steps/ and a line here.
/// </summary>
internal static class StepList
{
    public const string SetText = "set-text";
    public const string End = "end";

    /// <summary>
    /// What <c>expect-reference-headings</c> takes for a Reference tab that draws no headings.
    /// </summary>
    public const string NoHeadings = "none";

    /// <summary>
    /// Whether the tab is the Libraries tab.
    /// </summary>
    private static readonly Dictionary<string, bool> _tabs = new()
    {
        ["scripts"] = false,
        ["libraries"] = true,
    };

    /// <summary>
    /// Whether the Blocks side's tab is the Reference tab.
    /// </summary>
    private static readonly Dictionary<string, bool> _blocksTabs = new()
    {
        ["placed"] = false,
        ["reference"] = true,
    };

    private static readonly Dictionary<string, bool> _onOff = new()
    {
        ["on"] = true,
        ["off"] = false,
    };

    private static readonly Dictionary<string, Func<string, string, ScenarioStep>> _steps = new()
    {
        ["open-window"] = (step, argument) => None(step, argument, new OpenWindowStep()),
        ["close-window"] = (step, argument) => None(step, argument, new CloseWindowStep()),
        ["new-script"] = (step, argument) => new NewScriptStep(StepArgument.Text(step, argument, "a name")),
        ["select-script"] = (step, argument) => new SelectScriptStep(StepArgument.Text(step, argument, "a name")),
        ["new-library"] = (step, argument) => new NewLibraryStep(StepArgument.Text(step, argument, "a name")),
        ["select-library"] = (step, argument) => new SelectLibraryStep(StepArgument.Text(step, argument, "a name")),
        ["open-delete"] = (step, argument) => new OpenDeleteStep(StepArgument.Text(step, argument, "a name")),
        ["show-tab"] = (step, argument) => new ShowTabStep(StepArgument.Choice(step, argument, _tabs)),
        ["expect-tab"] = (step, argument) => new ExpectTabStep(StepArgument.Choice(step, argument, _tabs)),
        ["show-blocks-tab"] = (step, argument) => new ShowBlocksTabStep(StepArgument.Choice(step, argument, _blocksTabs)),
        ["expect-blocks-tab"] = (step, argument) => new ExpectBlocksTabStep(StepArgument.Choice(step, argument, _blocksTabs)),
        ["set-reference-filter"] = (_, argument) => new SetReferenceFilterStep(argument),
        ["expect-reference-headings"] = ExpectReferenceHeadings,
        ["open-new"] = (step, argument) => None(step, argument, new OpenNewStep()),
        ["open-rename"] = (step, argument) => new OpenRenameStep(StepArgument.Text(step, argument, "a name")),
        ["type-name"] = (step, argument) => new TypeNameStep(StepArgument.Text(step, argument, "the name to type")),
        ["press-prompt"] = (step, argument) => new PressPromptStep(StepArgument.Choice(step, argument, PromptNames.Buttons)),
        ["select-group"] = (step, argument) => new SelectGroupStep(StepArgument.Text(step, argument, "a name")),
        ["expect-list-result"] = (step, argument) => new ExpectListResultStep(StepArgument.Text(step, argument, "the text to look for")),
        ["expect-prompt"] = (step, argument) => new ExpectPromptStep(StepArgument.Choice(step, argument, PromptNames.Prompts)),
        ["expect-footer-in-view"] = (step, argument) => None(step, argument, new ExpectFooterInViewStep()),
        ["set-window-height"] = (step, argument) => new SetWindowHeightStep(StepArgument.Pixels(step, argument)),
        ["expect-window-height"] = (step, argument) => new ExpectWindowHeightStep(StepArgument.Pixels(step, argument)),
        ["set-window-size"] = (step, argument) => new SetWindowSizeStep(StepArgument.Size(step, argument)),
        ["center-window"] = (step, argument) => None(step, argument, new CenterWindowStep()),
        ["expect-window-size"] = (step, argument) => new ExpectWindowSizeStep(StepArgument.Size(step, argument)),
        ["expect-window-width"] = (step, argument) => new ExpectWindowWidthStep(StepArgument.PixelsOrScreen(step, argument)),
        ["expect-window-centered"] = (step, argument) => None(step, argument, new ExpectWindowCenteredStep()),
        ["drag-window-edge"] = (step, argument) => DragWindowEdge(step, argument, false),
        ["drag-window-edge-sideways"] = (step, argument) => DragWindowEdge(step, argument, true),
        ["remember-window-rect"] = (step, argument) => None(step, argument, new RememberWindowRectStep()),
        ["expect-window-rect"] = (step, argument) => None(step, argument, new ExpectWindowRectStep()),
        ["expect-window-start-rect"] = (step, argument) => None(step, argument, new ExpectWindowStartRectStep()),
        ["library-read-failed"] = (step, argument) => new LibraryReadFailedStep(StepArgument.Choice(step, argument, _onOff)),
        ["startup-warning"] = (step, argument) => new StartupWarningStep(StepArgument.Choice(step, argument, _onOff)),
        ["expect-library-read-failed"] = (step, argument) => new ExpectLibraryReadFailedStep(StepArgument.Choice(step, argument, _onOff)),
        ["expect-startup-warning"] = (step, argument) => new ExpectStartupWarningStep(StepArgument.Choice(step, argument, _onOff)),
        ["expect-editor-in-view"] = (step, argument) => None(step, argument, new ExpectEditorInViewStep()),
        ["expect-editor-in-view-each-frame"] = (step, argument) => new ExpectEditorInViewEachFrameStep(StepArgument.Count(step, argument)),
        ["compile"] = (step, argument) => None(step, argument, new CompileStep()),
        ["expect-text"] = (step, argument) => new ExpectTextStep(StepArgument.Text(step, argument, "the text to look for")),
        ["expect-result"] = (step, argument) => new ExpectResultStep(StepArgument.Text(step, argument, "the text to look for")),
        ["expect-no-errors"] = (step, argument) => None(step, argument, new ExpectNoErrorsStep()),
        ["expect-errors"] = (step, argument) => new ExpectErrorsStep(StepArgument.Count(step, argument)),
        ["expect-error"] = ExpectError,
        ["expect-blocks"] = (step, argument) => new ExpectBlocksStep(StepArgument.Count(step, argument)),
        ["caret-at-start"] = (step, argument) => None(step, argument, new PlaceCaretStep(atEnd: false)),
        ["caret-at-end"] = (step, argument) => None(step, argument, new PlaceCaretStep(atEnd: true)),
        ["complete"] = (step, argument) => None(step, argument, new CompleteStep()),
        ["expect-completion"] = (step, argument) => ExpectCompletion(StepArgument.Text(step, argument, "a row's label, and optionally its detail")),
        ["expect-completion-no-detail"] = (step, argument) => new ExpectCompletionStep(ReadLabel(step, argument), string.Empty),
        ["expect-completion-missing"] = (step, argument) => new ExpectCompletionMissingStep(ReadLabel(step, argument)),
        ["expect-call-help"] = (step, argument) => new ExpectCallHelpStep(StepArgument.Text(step, argument, "the text to look for")),
        ["expect-call-help-active"] = (step, argument) => new ExpectCallHelpActiveStep(StepArgument.Text(step, argument, "the highlighted input's or Setting's label")),
        ["expect-call-help-no-active"] = (step, argument) => None(step, argument, new ExpectCallHelpActiveStep(null)),
        ["expect-no-completion"] = (step, argument) => None(step, argument, new ExpectNoCompletionStep()),
        ["expect-no-call-help"] = (step, argument) => None(step, argument, new ExpectNoCallHelpStep()),
        ["hover"] = (step, argument) => new HoverStep(StepArgument.Text(step, argument, "the text to move the mouse onto")),
        ["expect-hover"] = (step, argument) => new ExpectHoverStep(StepArgument.Text(step, argument, "the text to look for")),
        ["expect-no-hover"] = (step, argument) => None(step, argument, new ExpectNoHoverStep()),
        ["press"] = (step, argument) => new PressStep(StepArgument.Keys(step, argument, KeysStep.MaxKeysPerFrame)),
        ["type"] = (step, argument) => new TypeStep(StepArgument.Text(step, argument, "the text to type")),
        ["place-block"] = PlaceBlock,
        ["set-block-setting"] = SetBlockSetting,
        ["name-group-block"] = NameGroupBlock,
        ["turn-off-style"] = TurnOffStyle,
        ["insert-selected"] = (step, argument) => None(step, argument, new InsertSelectedStep()),
        ["undo"] = (step, argument) => None(step, argument, new UndoStep()),
        ["redo"] = (step, argument) => None(step, argument, new RedoStep()),
        ["place-start"] = (step, argument) => None(step, argument, new PlaceStartStep()),
        ["test-mode"] = (step, argument) => None(step, argument, new TestModeStep()),
        ["back-to-editor"] = (step, argument) => None(step, argument, new BackToEditorStep()),
        ["wait"] = (step, argument) => new WaitStep(StepArgument.Seconds(step, argument)),
        ["screenshot"] = (step, argument) => new ScreenshotStep(StepArgument.FileName(step, argument)),
        ["expect-no-exceptions"] = (step, argument) => None(step, argument, new ExpectNoExceptionsStep()),
        ["log"] = (step, argument) => new LogStep(StepArgument.Text(step, argument, "the text to log")),
    };

    /// <summary>
    /// Includes <see cref="SetText"/>, which <see cref="ScenarioParser"/> reads itself as it spans several lines.
    /// </summary>
    public static IEnumerable<string> Names => [.. _steps.Keys, SetText];

    /// <summary>
    /// Returns null for an unknown step.
    /// </summary>
    public static ScenarioStep Create(string step, string argument)
    {
        return _steps.TryGetValue(step, out var create) ? create(step, argument) : null;
    }

    /// <summary>
    /// A label has no spaces, so the rest of the line is the detail.
    /// </summary>
    private static ScenarioStep ExpectCompletion(string argument)
    {
        var space = argument.IndexOf(' ');
        return space < 0
            ? new ExpectCompletionStep(argument, null)
            : new ExpectCompletionStep(argument.Substring(0, space), argument.Substring(space + 1).Trim());
    }

    /// <summary>
    /// A completion row's label, which has no spaces. <c>expect-completion-no-detail</c> takes only the label, as the
    /// detail can't be left empty on <c>expect-completion</c>'s line: the line's trailing spaces are taken off.
    /// </summary>
    private static string ReadLabel(string step, string argument)
    {
        var what = "a row's label";
        var label = StepArgument.Text(step, argument, what);
        return label.IndexOf(' ') < 0 ? label : throw new ScenarioFormatException($"{step} takes {what}, with no spaces, not '{argument}'");
    }

    /// <summary>
    /// Headings separated by spaces, each a name with <see cref="ReferenceHeading.ClosedSuffix"/> after it for a closed
    /// one, or <see cref="NoHeadings"/> alone. A heading's name has no spaces, nor a colon.
    /// </summary>
    private static ScenarioStep ExpectReferenceHeadings(string step, string argument)
    {
        var what = $"the headings, such as Inputs Libraries{ReferenceHeading.ClosedSuffix}, or {NoHeadings}";
        var words = StepArgument.Text(step, argument, what).Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 1 && words[0] == NoHeadings)
            return new ExpectReferenceHeadingsStep([]);

        var headings = new List<ReferenceHeading>();
        foreach (var word in words)
        {
            var open = !word.EndsWith(ReferenceHeading.ClosedSuffix, StringComparison.Ordinal);
            var name = open ? word : word.Substring(0, word.Length - ReferenceHeading.ClosedSuffix.Length);
            if (name.Length == 0 || name == NoHeadings || name.IndexOf(':') >= 0)
                throw new ScenarioFormatException($"{step} takes {what}, not '{word}'");

            headings.Add(new ReferenceHeading(name, open));
        }

        return new ExpectReferenceHeadingsStep(headings);
    }

    /// <summary>
    /// <c>&lt;line&gt;:&lt;column&gt; &lt;text&gt;</c>, where the text is the rest of the line.
    /// </summary>
    private static ScenarioStep ExpectError(string step, string argument)
    {
        var what = $"{step} takes a line and column, such as 2:5, then the text to look for";
        var space = argument.IndexOf(' ');
        var text = space < 0 ? string.Empty : argument.Substring(space + 1).Trim();
        if (text.Length == 0)
            throw new ScenarioFormatException(what);

        var written = argument.Substring(0, space);
        var position = written.Split(':');
        if (position.Length != 2 || !StepArgument.TryPositive(position[0], out var line) || !StepArgument.TryPositive(position[1], out var column))
            throw new ScenarioFormatException($"{what}, not '{written}'");

        return new ExpectErrorStep(new Position(line, column), text);
    }

    /// <summary>
    /// <c>&lt;function&gt;</c>, or <c>&lt;function&gt; as &lt;name&gt;</c> to remember the block by the name.
    /// </summary>
    private static ScenarioStep PlaceBlock(string step, string argument)
    {
        var what = "a Block Function's name, and optionally 'as <name>'";
        var words = StepArgument.Text(step, argument, what).Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 1)
            return new PlaceBlockStep(words[0]);

        if (words[1] != "as")
            throw new ScenarioFormatException($"{step} takes {what}, not '{argument}'");

        var name = StepArgument.FileName($"{step} ... as", string.Join(" ", words.Skip(2)));
        return new PlaceBlockStep(words[0], name);
    }

    /// <summary>
    /// <c>&lt;name&gt; &lt;setting&gt; &lt;value&gt;</c>. Whether the block has the Setting and it takes the value is
    /// only known once the block is placed, so the step checks that when it runs.
    /// </summary>
    private static ScenarioStep SetBlockSetting(string step, string argument)
    {
        var what = "a placed block's name, a Setting's name and its value, such as t mode global_velocity";
        var words = StepArgument.Text(step, argument, what).Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length != 3)
            throw new ScenarioFormatException($"{step} takes {what}, not '{argument}'");

        return new SetBlockSettingStep(StepArgument.FileName(step, words[0]), words[1], words[2]);
    }

    /// <summary>
    /// <c>&lt;script&gt; &lt;function&gt; as &lt;name&gt;</c>. A Script's name can have spaces, so the last three words
    /// are the rest.
    /// </summary>
    private static ScenarioStep NameGroupBlock(string step, string argument)
    {
        var what = "a Script's name, a Block Function's name and 'as <name>', such as Lights logic as l";
        var words = StepArgument.Text(step, argument, what).Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length < 4 || words[words.Length - 2] != "as")
            throw new ScenarioFormatException($"{step} takes {what}, not '{argument}'");

        var name = StepArgument.FileName($"{step} ... as", words[words.Length - 1]);
        return new NameGroupBlockStep(string.Join(" ", words.Take(words.Length - 3)), words[words.Length - 3], name);
    }

    /// <summary>
    /// <c>&lt;function&gt;</c>, or <c>&lt;function&gt; &lt;style&gt;</c>. Whether the type has the style is only
    /// checked when the step runs, as for <c>place-block</c>'s Block Function.
    /// </summary>
    private static ScenarioStep TurnOffStyle(string step, string argument)
    {
        var what = "a Block Function's name, and optionally one of its styles";
        var words = StepArgument.Text(step, argument, what).Split([' '], StringSplitOptions.RemoveEmptyEntries);
        return words.Length switch
        {
            1 => new TurnOffStyleStep(words[0]),
            2 => new TurnOffStyleStep(words[0], words[1]),
            _ => throw new ScenarioFormatException($"{step} takes {what}, not '{argument}'"),
        };
    }

    /// <summary>
    /// <c>&lt;pixels&gt;</c>, below 0 to drag up, or left when <paramref name="sideways"/>, and optionally <c>over &lt;frames&gt;</c>, 1 when left out,
    /// with <c>then</c> before each move after the first, all in one press.
    /// </summary>
    private static ScenarioStep DragWindowEdge(string step, string argument, bool sideways)
    {
        var direction = sideways ? "right, below 0 for left" : "down, below 0 for up";
        var what = $"the pixels to drag the window's edge {direction}, and optionally 'over <frames>', with 'then' before each move after the first";
        var words = StepArgument.Text(step, argument, what).Split([' '], StringSplitOptions.RemoveEmptyEntries);
        var moves = new List<DragMove>();
        var start = 0;
        // Each move ends at the next 'then', or at the end of the words
        for (var end = 0; end <= words.Length; end++)
        {
            if (end < words.Length && words[end] != "then")
                continue;

            if (!TryReadDragMove(words, start, end - start, out var move))
                throw new ScenarioFormatException($"{step} takes {what}, not '{argument}'");

            moves.Add(move);
            start = end + 1;
        }

        return new DragWindowEdgeStep(moves, sideways);
    }

    /// <summary>
    /// Reads <c>&lt;pixels&gt; [over &lt;frames&gt;]</c> from the <paramref name="count"/> words at <paramref name="start"/>.
    /// </summary>
    private static bool TryReadDragMove(string[] words, int start, int count, out DragMove move)
    {
        move = default;
        var frames = 1;
        if (count == 0 || !int.TryParse(words[start], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var pixels) || pixels == 0
            || !(count == 1 || (count == 3 && words[start + 1] == "over" && StepArgument.TryPositive(words[start + 2], out frames))))
            return false;

        move = new DragMove(pixels, frames);
        return true;
    }

    private static ScenarioStep None(string step, string argument, ScenarioStep created)
    {
        StepArgument.None(step, argument);
        return created;
    }
}
