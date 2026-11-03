using System;
using System.Linq;
using System.Text.RegularExpressions;
using TNRD.Zeepkist.LogicScripting.Compiler;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;
using TNRD.Zeepkist.LogicScripting.InGameTests;
using TNRD.Zeepkist.LogicScripting.InGameTests.Steps;
using TNRD.Zeepkist.LogicScripting.UI;
using Xunit;

namespace TNRD.Zeepkist.LogicScripting.Tests;

public class StepListTests
{
    [Theory]
    [InlineData("open-window", typeof(OpenWindowStep))]
    [InlineData("close-window", typeof(CloseWindowStep))]
    [InlineData("new-script Lights", typeof(NewScriptStep))]
    [InlineData("select-script Lights", typeof(SelectScriptStep))]
    [InlineData("new-library timing", typeof(NewLibraryStep))]
    [InlineData("select-library timing", typeof(SelectLibraryStep))]
    [InlineData("open-delete Lights", typeof(OpenDeleteStep))]
    [InlineData("show-tab libraries", typeof(ShowTabStep))]
    [InlineData("expect-tab scripts", typeof(ExpectTabStep))]
    [InlineData("open-new", typeof(OpenNewStep))]
    [InlineData("open-rename Lights", typeof(OpenRenameStep))]
    [InlineData("type-name Lights", typeof(TypeNameStep))]
    [InlineData("press-prompt keep-blocks", typeof(PressPromptStep))]
    [InlineData("select-group Lights", typeof(SelectGroupStep))]
    [InlineData("expect-list-result Deleted", typeof(ExpectListResultStep))]
    [InlineData("expect-prompt none", typeof(ExpectPromptStep))]
    [InlineData("expect-footer-in-view", typeof(ExpectFooterInViewStep))]
    [InlineData("set-window-height 300", typeof(SetWindowHeightStep))]
    [InlineData("expect-window-height 300", typeof(ExpectWindowHeightStep))]
    [InlineData("set-window-size 1600 900", typeof(SetWindowSizeStep))]
    [InlineData("center-window", typeof(CenterWindowStep))]
    [InlineData("expect-window-size 1600 900", typeof(ExpectWindowSizeStep))]
    [InlineData("expect-window-width 64", typeof(ExpectWindowWidthStep))]
    [InlineData("expect-window-width screen", typeof(ExpectWindowWidthStep))]
    [InlineData("expect-window-centered", typeof(ExpectWindowCenteredStep))]
    [InlineData("drag-window-edge -400 over 8", typeof(DragWindowEdgeStep))]
    [InlineData("drag-window-edge-sideways -400 over 8", typeof(DragWindowEdgeStep))]
    [InlineData("remember-window-rect", typeof(RememberWindowRectStep))]
    [InlineData("expect-window-rect", typeof(ExpectWindowRectStep))]
    [InlineData("expect-window-start-rect", typeof(ExpectWindowStartRectStep))]
    [InlineData("library-read-failed on", typeof(LibraryReadFailedStep))]
    [InlineData("library-read-failed off", typeof(LibraryReadFailedStep))]
    [InlineData("startup-warning on", typeof(StartupWarningStep))]
    [InlineData("startup-warning off", typeof(StartupWarningStep))]
    [InlineData("expect-library-read-failed on", typeof(ExpectLibraryReadFailedStep))]
    [InlineData("expect-startup-warning off", typeof(ExpectStartupWarningStep))]
    [InlineData("expect-editor-in-view", typeof(ExpectEditorInViewStep))]
    [InlineData("set-text\na = 1\nend", typeof(SetTextStep))]
    [InlineData("compile", typeof(CompileStep))]
    [InlineData("expect-text a = 1", typeof(ExpectTextStep))]
    [InlineData("expect-result Placed", typeof(ExpectResultStep))]
    [InlineData("expect-no-errors", typeof(ExpectNoErrorsStep))]
    [InlineData("expect-errors 2", typeof(ExpectErrorsStep))]
    [InlineData("expect-error 1:5 isn't defined", typeof(ExpectErrorStep))]
    [InlineData("expect-blocks 3", typeof(ExpectBlocksStep))]
    [InlineData("caret-at-start", typeof(PlaceCaretStep))]
    [InlineData("caret-at-end", typeof(PlaceCaretStep))]
    [InlineData("complete", typeof(CompleteStep))]
    [InlineData("expect-completion t", typeof(ExpectCompletionStep))]
    [InlineData("expect-completion-no-detail function", typeof(ExpectCompletionStep))]
    [InlineData("expect-completion-missing op:", typeof(ExpectCompletionMissingStep))]
    [InlineData("expect-call-help logic(", typeof(ExpectCallHelpStep))]
    [InlineData("expect-call-help-active op:", typeof(ExpectCallHelpActiveStep))]
    [InlineData("expect-call-help-no-active", typeof(ExpectCallHelpActiveStep))]
    [InlineData("expect-no-completion", typeof(ExpectNoCompletionStep))]
    [InlineData("expect-no-call-help", typeof(ExpectNoCallHelpStep))]
    [InlineData("press ctrl+space", typeof(PressStep))]
    [InlineData("type b = a", typeof(TypeStep))]
    [InlineData("place-block lamp", typeof(PlaceBlockStep))]
    [InlineData("place-block lamp as lamp", typeof(PlaceBlockStep))]
    [InlineData("set-block-setting t mode global_velocity", typeof(SetBlockSettingStep))]
    [InlineData("name-group-block Lights logic as l", typeof(NameGroupBlockStep))]
    [InlineData("turn-off-style lamp", typeof(TurnOffStyleStep))]
    [InlineData("insert-selected", typeof(InsertSelectedStep))]
    [InlineData("undo", typeof(UndoStep))]
    [InlineData("redo", typeof(RedoStep))]
    [InlineData("place-start", typeof(PlaceStartStep))]
    [InlineData("test-mode", typeof(TestModeStep))]
    [InlineData("back-to-editor", typeof(BackToEditorStep))]
    [InlineData("wait 1", typeof(WaitStep))]
    [InlineData("screenshot compiled", typeof(ScreenshotStep))]
    [InlineData("log hello", typeof(LogStep))]
    [InlineData("expect-no-exceptions", typeof(ExpectNoExceptionsStep))]
    public void EachStep_ParsesToItsOwnClass(string line, Type type)
    {
        var scenario = ScenarioParser.Parse("steps", line, out var error);

        Assert.Null(error);
        Assert.IsType(type, Assert.Single(scenario.Steps));
    }

    [Theory]
    [InlineData("place-start now")]
    [InlineData("test-mode 5")]
    [InlineData("back-to-editor now")]
    public void TheTestModeSteps_TakeNothingAfterThem(string line)
    {
        var scenario = ScenarioParser.Parse("steps", line, out var error);

        Assert.Null(scenario);
        Assert.Contains("takes nothing after it", error);
    }

    [Theory]
    [InlineData("expect-completion a", "a", null)]
    [InlineData("expect-completion a logic", "a", "logic")]
    [InlineData("expect-completion a placed block", "a", "placed block")]
    public void ExpectCompletion_ALabel_ThenTheDetail(string line, string label, string detail)
    {
        var step = Assert.IsType<ExpectCompletionStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(label, step.Label);
        Assert.Equal(detail, step.Detail);
    }

    [Fact]
    public void ExpectCompletionNoDetail_ALabel_AndAnEmptyDetail()
    {
        var step = Assert.IsType<ExpectCompletionStep>(Assert.Single(ScenarioParser.Parse("steps", "expect-completion-no-detail f", out _).Steps));

        Assert.Equal("f", step.Label);
        Assert.Equal(string.Empty, step.Detail);
    }

    [Theory]
    [InlineData("expect-completion-no-detail", "takes a row's label")]
    [InlineData("expect-completion-no-detail f (a, a)", "with no spaces")]
    [InlineData("expect-completion-missing", "takes a row's label")]
    [InlineData("expect-completion-missing a b", "with no spaces")]
    public void TheLabelOnlyCompletionSteps_TakeOneLabel(string line, string message)
    {
        var scenario = ScenarioParser.Parse("steps", line, out var error);

        Assert.Null(scenario);
        Assert.Contains(message, error);
    }

    [Fact]
    public void ExpectCompletionMissing_ReadsTheLabel()
    {
        var step = Assert.IsType<ExpectCompletionMissingStep>(Assert.Single(ScenarioParser.Parse("steps", "expect-completion-missing a:", out _).Steps));

        Assert.Equal("a:", step.Label);
    }

    [Theory]
    [InlineData("expect-call-help-active op:", "op:")]
    [InlineData("expect-call-help-no-active", null)]
    public void ExpectCallHelpActive_ReadsTheLabel(string line, string label)
    {
        var step = Assert.IsType<ExpectCallHelpActiveStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(label, step.Label);
    }

    [Theory]
    [InlineData("press enter", "Return", '\n', (int)KeyModifiers.None)]
    [InlineData("press ctrl+space", "Space", ' ', (int)KeyModifiers.Control)]
    [InlineData("press ctrl+/", "Slash", '/', (int)KeyModifiers.Control)]
    [InlineData("press Shift+A", "A", 'A', (int)KeyModifiers.Shift)]
    [InlineData("press ctrl+shift+k", "K", 'K', (int)(KeyModifiers.Control | KeyModifiers.Shift))]
    [InlineData("press escape", "Escape", '\0', (int)KeyModifiers.None)]
    [InlineData("press 7", "Alpha7", '7', (int)KeyModifiers.None)]
    public void Press_ReadsTheKeyAndItsModifiers(string line, string key, char character, int modifiers)
    {
        var step = Assert.IsType<PressStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        var press = Assert.Single(step.Keys);
        Assert.Equal(key, press.Key);
        Assert.Equal(character, press.Character);
        Assert.Equal((KeyModifiers)modifiers, press.Modifiers);
    }

    [Fact]
    public void Press_SeveralKeys_KeepsTheirOrder()
    {
        var step = Assert.IsType<PressStep>(Assert.Single(ScenarioParser.Parse("steps", "press  escape enter", out _).Steps));

        Assert.Equal(["Escape", "Return"], step.Keys.Select(k => k.Key));
    }

    [Theory]
    [InlineData("press", "press takes 1 to 8 keys, such as enter or ctrl+space")]
    [InlineData("press a b c d e f g h i", "press takes 1 to 8 keys, such as enter or ctrl+space, not 9")]
    [InlineData("press ctrl+", "press doesn't know the key 'ctrl+'")]
    [InlineData("press hyper+a", "press doesn't know the key 'hyper+a'")]
    [InlineData("press (", "press doesn't know the key '('")]
    public void Press_WithoutKnownKeys_Fails(string line, string error)
    {
        ScenarioParser.Parse("steps", line, out var actual);

        Assert.Equal($"line 1: {error}", actual);
    }

    [Theory]
    [InlineData('a', "A", (int)KeyModifiers.None)]
    [InlineData('A', "A", (int)KeyModifiers.Shift)]
    [InlineData(' ', "Space", (int)KeyModifiers.None)]
    [InlineData('=', "Equals", (int)KeyModifiers.None)]
    [InlineData('(', "None", (int)KeyModifiers.None)]
    public void Type_SendsTheKeyForEachCharacter(char character, string key, int modifiers)
    {
        var press = KeyNames.ForCharacter(character);

        Assert.Equal(key, press.Key);
        Assert.Equal(character, press.Character);
        Assert.Equal((KeyModifiers)modifiers, press.Modifiers);
    }

    [Fact]
    public void ExpectError_APosition_ThenTheText()
    {
        var step = Assert.IsType<ExpectErrorStep>(Assert.Single(ScenarioParser.Parse("steps", "expect-error 12:3 'nope' isn't defined", out _).Steps));

        Assert.Equal((12, 3), (step.Position.Line, step.Position.Column));
        Assert.Equal("'nope' isn't defined", step.Text);
    }

    [Theory]
    [InlineData("expect-error 1:5")]
    [InlineData("expect-error nope")]
    [InlineData("expect-error 1 text")]
    [InlineData("expect-error 0:1 text")]
    [InlineData("expect-error 1:x text")]
    [InlineData("expect-error 1:2:3 text")]
    public void ExpectError_NeedsAOneBasedPosition_AndText(string line)
    {
        var scenario = ScenarioParser.Parse("steps", line, out var error);

        Assert.Null(scenario);
        Assert.Contains("takes a line and column", error);
    }

    [Theory]
    [InlineData("show-tab scripts", false)]
    [InlineData("show-tab libraries", true)]
    public void ShowTab_ReadsTheTab(string line, bool libraries)
    {
        var step = Assert.IsType<ShowTabStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(libraries, step.Libraries);
    }

    [Theory]
    [InlineData("expect-tab scripts", false)]
    [InlineData("expect-tab libraries", true)]
    public void ExpectTab_ReadsTheTab(string line, bool libraries)
    {
        var step = Assert.IsType<ExpectTabStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(libraries, step.Libraries);
    }

    [Theory]
    [InlineData("library-read-failed on", true)]
    [InlineData("library-read-failed off", false)]
    public void LibraryReadFailed_ReadsOnOrOff(string line, bool show)
    {
        var step = Assert.IsType<LibraryReadFailedStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(show, step.Show);
    }

    [Theory]
    [InlineData("startup-warning on", true)]
    [InlineData("startup-warning off", false)]
    public void StartupWarning_ReadsOnOrOff(string line, bool show)
    {
        var step = Assert.IsType<StartupWarningStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(show, step.Show);
    }

    [Theory]
    [InlineData("expect-library-read-failed on", true)]
    [InlineData("expect-library-read-failed off", false)]
    public void ExpectLibraryReadFailed_ReadsOnOrOff(string line, bool shown)
    {
        var step = Assert.IsType<ExpectLibraryReadFailedStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(shown, step.Shown);
    }

    [Theory]
    [InlineData("expect-startup-warning on", true)]
    [InlineData("expect-startup-warning off", false)]
    public void ExpectStartupWarning_ReadsOnOrOff(string line, bool shown)
    {
        var step = Assert.IsType<ExpectStartupWarningStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(shown, step.Shown);
    }

    [Theory]
    [InlineData("press-prompt ok", (int)PromptButton.Ok)]
    [InlineData("press-prompt cancel", (int)PromptButton.Cancel)]
    [InlineData("press-prompt delete", (int)PromptButton.Delete)]
    [InlineData("press-prompt keep-blocks", (int)PromptButton.KeepBlocks)]
    public void PressPrompt_ReadsTheButton(string line, int button)
    {
        var step = Assert.IsType<PressPromptStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal((PromptButton)button, step.Button);
    }

    [Theory]
    [InlineData("expect-prompt none", (int)ListPrompt.None)]
    [InlineData("expect-prompt new", (int)ListPrompt.New)]
    [InlineData("expect-prompt rename", (int)ListPrompt.Rename)]
    [InlineData("expect-prompt delete", (int)ListPrompt.Delete)]
    public void ExpectPrompt_ReadsThePrompt(string line, int prompt)
    {
        var step = Assert.IsType<ExpectPromptStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal((ListPrompt)prompt, step.Prompt);
    }

    [Theory]
    [InlineData("show-tab", "show-tab takes scripts or libraries")]
    [InlineData("show-tab blocks", "show-tab takes scripts or libraries, not 'blocks'")]
    [InlineData("expect-tab", "expect-tab takes scripts or libraries")]
    [InlineData("expect-tab blocks", "expect-tab takes scripts or libraries, not 'blocks'")]
    [InlineData("press-prompt yes", "press-prompt takes ok, cancel, delete or keep-blocks, not 'yes'")]
    [InlineData("expect-prompt open", "expect-prompt takes none, new, rename or delete, not 'open'")]
    [InlineData("set-window-height 0", "set-window-height takes a whole number of pixels above 0, not '0'")]
    [InlineData("set-window-height tall", "set-window-height takes a whole number of pixels above 0, not 'tall'")]
    [InlineData("expect-window-height", "expect-window-height takes a whole number of pixels above 0")]
    [InlineData("expect-window-height 12.5", "expect-window-height takes a whole number of pixels above 0, not '12.5'")]
    [InlineData("set-window-size", "set-window-size takes a width and a height, each a whole number of pixels above 0")]
    [InlineData("set-window-size 1600", "set-window-size takes a width and a height, each a whole number of pixels above 0, not '1600'")]
    [InlineData("set-window-size 1600 0", "set-window-size takes a width and a height, each a whole number of pixels above 0, not '1600 0'")]
    [InlineData("set-window-size wide 900", "set-window-size takes a width and a height, each a whole number of pixels above 0, not 'wide 900'")]
    [InlineData("set-window-size 1600 900 1", "set-window-size takes a width and a height, each a whole number of pixels above 0, not '1600 900 1'")]
    [InlineData("expect-window-size 1600x900", "expect-window-size takes a width and a height, each a whole number of pixels above 0, not '1600x900'")]
    [InlineData("center-window now", "center-window takes nothing after it")]
    [InlineData("expect-window-width", "expect-window-width takes a whole number of pixels above 0, or screen")]
    [InlineData("expect-window-width 0", "expect-window-width takes a whole number of pixels above 0, or screen, not '0'")]
    [InlineData("expect-window-width wide", "expect-window-width takes a whole number of pixels above 0, or screen, not 'wide'")]
    [InlineData("expect-window-centered now", "expect-window-centered takes nothing after it")]
    [InlineData("open-new now", "open-new takes nothing after it")]
    [InlineData("expect-footer-in-view now", "expect-footer-in-view takes nothing after it")]
    [InlineData("library-read-failed", "library-read-failed takes on or off")]
    [InlineData("library-read-failed yes", "library-read-failed takes on or off, not 'yes'")]
    [InlineData("startup-warning", "startup-warning takes on or off")]
    [InlineData("startup-warning yes", "startup-warning takes on or off, not 'yes'")]
    [InlineData("expect-library-read-failed", "expect-library-read-failed takes on or off")]
    [InlineData("expect-library-read-failed yes", "expect-library-read-failed takes on or off, not 'yes'")]
    [InlineData("expect-startup-warning", "expect-startup-warning takes on or off")]
    [InlineData("expect-startup-warning yes", "expect-startup-warning takes on or off, not 'yes'")]
    [InlineData("expect-editor-in-view now", "expect-editor-in-view takes nothing after it")]
    [InlineData("expect-editor-in-view-each-frame", "expect-editor-in-view-each-frame takes a whole number of 0 or more")]
    [InlineData("expect-editor-in-view-each-frame many", "expect-editor-in-view-each-frame takes a whole number of 0 or more, not 'many'")]
    [InlineData("drag-window-edge", "drag-window-edge takes the pixels to drag the window's edge down, below 0 for up, and optionally 'over <frames>'")]
    [InlineData("drag-window-edge 0", "drag-window-edge takes the pixels to drag the window's edge down, below 0 for up, and optionally 'over <frames>', not '0'")]
    [InlineData("drag-window-edge up", "drag-window-edge takes the pixels to drag the window's edge down, below 0 for up, and optionally 'over <frames>', not 'up'")]
    [InlineData("drag-window-edge -400 over 0", "drag-window-edge takes the pixels to drag the window's edge down, below 0 for up, and optionally 'over <frames>', not '-400 over 0'")]
    [InlineData("drag-window-edge -400 in 8", "drag-window-edge takes the pixels to drag the window's edge down, below 0 for up, and optionally 'over <frames>', not '-400 in 8'")]
    [InlineData("drag-window-edge -400 over", "drag-window-edge takes the pixels to drag the window's edge down, below 0 for up, and optionally 'over <frames>', not '-400 over'")]
    [InlineData("drag-window-edge-sideways", "drag-window-edge-sideways takes the pixels to drag the window's edge right, below 0 for left, and optionally 'over <frames>'")]
    [InlineData("drag-window-edge-sideways 0", "drag-window-edge-sideways takes the pixels to drag the window's edge right, below 0 for left, and optionally 'over <frames>', not '0'")]
    [InlineData("drag-window-edge-sideways left", "drag-window-edge-sideways takes the pixels to drag the window's edge right, below 0 for left, and optionally 'over <frames>', not 'left'")]
    [InlineData("drag-window-edge-sideways -400 over 0", "drag-window-edge-sideways takes the pixels to drag the window's edge right, below 0 for left, and optionally 'over <frames>', not '-400 over 0'")]
    [InlineData("remember-window-rect now", "remember-window-rect takes nothing after it")]
    [InlineData("expect-window-rect now", "expect-window-rect takes nothing after it")]
    [InlineData("expect-window-start-rect now", "expect-window-start-rect takes nothing after it")]
    public void TheLayoutSteps_WithAWrongArgument_Fail(string line, string error)
    {
        ScenarioParser.Parse("steps", line, out var actual);

        Assert.Equal($"line 1: {error}", actual);
    }

    [Fact]
    public void SetWindowHeight_ReadsThePixels()
    {
        var step = Assert.IsType<SetWindowHeightStep>(Assert.Single(ScenarioParser.Parse("steps", "set-window-height 300", out _).Steps));

        Assert.Equal(300, step.Height);
    }

    [Fact]
    public void SetWindowSize_ReadsTheWidthAndHeight()
    {
        var step = Assert.IsType<SetWindowSizeStep>(Assert.Single(ScenarioParser.Parse("steps", "set-window-size 1600 900", out _).Steps));

        Assert.Equal(1600, step.Width);
        Assert.Equal(900, step.Height);
    }

    [Fact]
    public void ExpectWindowSize_ReadsTheWidthAndHeight()
    {
        var step = Assert.IsType<ExpectWindowSizeStep>(Assert.Single(ScenarioParser.Parse("steps", "expect-window-size 1600 900", out _).Steps));

        Assert.Equal(1600, step.Width);
        Assert.Equal(900, step.Height);
    }

    [Theory]
    [InlineData("expect-window-width 64", 64)]
    [InlineData("expect-window-width screen", null)]
    public void ExpectWindowWidth_ReadsThePixels_OrTheScreen(string line, int? width)
    {
        var step = Assert.IsType<ExpectWindowWidthStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(width, step.Width);
    }

    [Fact]
    public void ExpectWindowHeight_ReadsThePixels()
    {
        var step = Assert.IsType<ExpectWindowHeightStep>(Assert.Single(ScenarioParser.Parse("steps", "expect-window-height 640", out _).Steps));

        Assert.Equal(640, step.Height);
    }

    [Theory]
    [InlineData("drag-window-edge 300", 300, 1, false)]
    [InlineData("drag-window-edge -1000 over 10", -1000, 10, false)]
    [InlineData("drag-window-edge +50  over  2", 50, 2, false)]
    [InlineData("drag-window-edge-sideways 300", 300, 1, true)]
    [InlineData("drag-window-edge-sideways -1000 over 10", -1000, 10, true)]
    public void DragWindowEdge_ReadsThePixels_TheFrames_AndTheDirection(string line, int pixels, int frames, bool sideways)
    {
        var step = Assert.IsType<DragWindowEdgeStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal((pixels, frames, sideways), (step.Pixels, step.Frames, step.Sideways));
    }

    [Fact]
    public void ExpectEditorInViewEachFrame_ReadsTheFrames()
    {
        var step = Assert.IsType<ExpectEditorInViewEachFrameStep>(Assert.Single(ScenarioParser.Parse("steps", "expect-editor-in-view-each-frame 5", out _).Steps));

        Assert.Equal(5, step.Frames);
    }

    [Theory]
    [InlineData("place-block lamp", "lamp", null)]
    [InlineData("place-block lamp as lamp_1", "lamp", "lamp_1")]
    [InlineData("place-block simple_telemetry as speed-gauge", "simple_telemetry", "speed-gauge")]
    public void PlaceBlock_AFunction_ThenOptionallyAName(string line, string function, string name)
    {
        var step = Assert.IsType<PlaceBlockStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal(function, step.Function);
        Assert.Equal(name, step.Name);
    }

    [Theory]
    [InlineData("place-block lamp as")]
    [InlineData("place-block lamp as a}b")]
    [InlineData("place-block lamp as two words")]
    public void PlaceBlock_TakesANameOfLettersDigitsDashesAndUnderscores(string line)
    {
        var scenario = ScenarioParser.Parse("steps", line, out var error);

        Assert.Null(scenario);
        Assert.Contains("takes a name of letters", error);
    }

    [Fact]
    public void SetBlockSetting_AName_ASetting_ThenAValue()
    {
        var step = Assert.IsType<SetBlockSettingStep>(Assert.Single(ScenarioParser.Parse("steps", "set-block-setting speed-gauge  mode global_velocity", out _).Steps));

        Assert.Equal("speed-gauge", step.Name);
        Assert.Equal("mode", step.Setting);
        Assert.Equal("global_velocity", step.Value);
    }

    [Theory]
    [InlineData("set-block-setting")]
    [InlineData("set-block-setting t")]
    [InlineData("set-block-setting t mode")]
    [InlineData("set-block-setting t mode basic now")]
    public void SetBlockSetting_TakesThreeWords(string line)
    {
        ScenarioParser.Parse("steps", line, out var error);

        Assert.Contains("set-block-setting takes a placed block's name, a Setting's name and its value", error);
    }

    [Fact]
    public void SetBlockSetting_TakesANameOfLettersDigitsDashesAndUnderscores()
    {
        ScenarioParser.Parse("steps", "set-block-setting a}b mode basic", out var error);

        Assert.Contains("takes a name of letters", error);
    }

    [Theory]
    [InlineData("name-group-block Lights logic as l", "Lights", "logic", "l")]
    [InlineData("name-group-block My Lights  power_display as display-1", "My Lights", "power_display", "display-1")]
    public void NameGroupBlock_AScript_AFunction_ThenAName(string line, string script, string function, string name)
    {
        var step = Assert.IsType<NameGroupBlockStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal((script, function, name), (step.Script, step.Function, step.Name));
    }

    [Theory]
    [InlineData("name-group-block")]
    [InlineData("name-group-block Lights logic")]
    [InlineData("name-group-block logic as l")]
    [InlineData("name-group-block Lights logic is l")]
    public void NameGroupBlock_TakesAScript_AFunction_AndAsAName(string line)
    {
        ScenarioParser.Parse("steps", line, out var error);

        Assert.Contains("name-group-block takes a Script's name, a Block Function's name and 'as <name>'", error);
    }

    [Fact]
    public void NameGroupBlock_TakesANameOfLettersDigitsDashesAndUnderscores()
    {
        ScenarioParser.Parse("steps", "name-group-block Lights logic as a}b", out var error);

        Assert.Contains("takes a name of letters", error);
    }

    [Theory]
    [InlineData("turn-off-style lamp", "lamp", null)]
    [InlineData("turn-off-style logic  wide", "logic", "wide")]
    public void TurnOffStyle_AFunction_ThenOptionallyAStyle(string line, string function, string style)
    {
        var step = Assert.IsType<TurnOffStyleStep>(Assert.Single(ScenarioParser.Parse("steps", line, out _).Steps));

        Assert.Equal((function, style), (step.Function, step.Style));
    }

    [Theory]
    [InlineData("turn-off-style")]
    [InlineData("turn-off-style logic wide now")]
    public void TurnOffStyle_TakesAFunction_AndOptionallyAStyle(string line)
    {
        ScenarioParser.Parse("steps", line, out var error);

        Assert.Contains("turn-off-style takes a Block Function's name, and optionally one of its styles", error);
    }

    [Theory]
    [InlineData("basic", 0)]
    [InlineData("global_velocity", 2)]
    public void SetBlockSetting_AnOption_IsStoredAsItsIndex(string value, int index)
    {
        var setting = SetBlockSettingStep.ToSetting(BlockManifest.SimpleTelemetryMode, value, out var error);

        Assert.Null(error);
        Assert.Equal(("xbs", SettingKind.Int, index), (setting.Key, setting.Kind, setting.IntValue));
    }

    [Theory]
    [InlineData("true", 1)]
    [InlineData("false", 0)]
    public void SetBlockSetting_ABool_IsStoredAsOneOrZero(string value, int stored)
    {
        var setting = SetBlockSettingStep.ToSetting(BlockManifest.DigitizerInvert, value, out var error);

        Assert.Null(error);
        Assert.Equal(("xt1", SettingKind.Int, stored), (setting.Key, setting.Kind, setting.IntValue));
    }

    [Fact]
    public void SetBlockSetting_APlayer_IsStoredAsItsIndex()
    {
        var setting = SetBlockSettingStep.ToSetting(BlockManifest.PlayerOrEveryone, "2", out var error);

        Assert.Null(error);
        Assert.Equal(("xpr", SettingKind.Int, 2), (setting.Key, setting.Kind, setting.IntValue));
    }

    [Theory]
    [InlineData("5", "5")]
    [InlineData("-12", "-12")]
    public void SetBlockSetting_ANumber_IsStoredAsText(string value, string stored)
    {
        var setting = SetBlockSettingStep.ToSetting(BlockManifest.RandomMinimum, value, out var error);

        Assert.Null(error);
        Assert.Equal(("xv1", SettingKind.Text, stored), (setting.Key, setting.Kind, setting.TextValue));
    }

    [Theory]
    [InlineData("mode", "fast", "'fast' isn't an option of mode. Use basic, local_velocity, global_velocity")]
    [InlineData("invert", "yes", "invert takes true or false, not 'yes'")]
    [InlineData("player", "5", "player takes all, 1, 2, 3 or 4, not '5'")]
    [InlineData("min", "1.5", "min takes a whole number, not '1.5'")]
    [InlineData("ticks", "0", "ticks must be at least 1, not '0'")]
    [InlineData("ticks", "99999999999", "ticks must be at most 2147483647, not '99999999999'")]
    public void SetBlockSetting_AValueTheSettingDoesNotTake_SaysWhatItTakes(string name, string value, string error)
    {
        var setting = new[]
        {
            BlockManifest.SimpleTelemetryMode,
            BlockManifest.DigitizerInvert,
            BlockManifest.PlayerOrEveryone,
            BlockManifest.RandomMinimum,
            BlockManifest.Delay.FindSetting("ticks"),
        }.Single(s => s.Name == name);

        Assert.Null(SetBlockSettingStep.ToSetting(setting, value, out var actual));
        Assert.Equal(error, actual);
    }

    [Fact]
    public void SetBlockSetting_ATogglersTargets_CantBeSet()
    {
        var targets = BlockManifest.All.SelectMany(t => t.Settings).First(s => s.Type == SettingType.Targets);

        Assert.Null(SetBlockSettingStep.ToSetting(targets, "a", out var error));
        Assert.Equal($"set-block-setting can't set {targets.Name}, a list of blocks", error);
    }

    [Fact]
    public void TheStepList_HoldsExactlyTheStepsTheDocsList()
    {
        var documented = DocsTable("### Steps").Select(step => step.Split(' ')[0]);

        Assert.Equal(documented.OrderBy(n => n), StepList.Names.OrderBy(n => n));
    }

    [Fact]
    public void TheDocs_ListExactlyTheRunScriptsOptions()
    {
        var script = RepoFiles.Read("tools/ingame-test.sh");
        var options = Regex.Matches(script, @"^\s*((?:-[-\w]+)(?: \| -[-\w]+)*)\)", RegexOptions.Multiline)
            .SelectMany(match => match.Groups[1].Value.Split(" | "));
        var documented = DocsTable("### Options").SelectMany(option => option.Split(", ")).Select(option => option.Split(' ')[0]);

        Assert.Equal(documented.OrderBy(o => o), options.Where(o => o != "-*").OrderBy(o => o));
    }

    [Theory]
    [InlineData("smoke.zlstest")]
    [InlineData("test-mode.zlstest")]
    [InlineData("keys.zlstest")]
    [InlineData("libraries.zlstest")]
    [InlineData("list-footer.zlstest")]
    [InlineData("startup-warning.zlstest")]
    [InlineData("select-layout-each-frame.zlstest")]
    [InlineData("set-block-setting.zlstest")]
    [InlineData("kept-state.zlstest")]
    [InlineData("group-block-turned-off.zlstest")]
    [InlineData("turned-off-back-on.zlstest")]
    [InlineData("window-size.zlstest")]
    [InlineData("placed-after-test-mode.zlstest")]
    public void TheShippedScenarios_CanBeRead(string file)
    {
        var text = RepoFiles.Read($"Zeepkist.LogicScripting.InGameTests/Scenarios/{file}");

        var scenario = ScenarioParser.Parse(file, text, out var error);

        Assert.Null(error);
        Assert.NotEmpty(scenario.Steps);
    }

    /// <summary>
    /// The first column of the table under <paramref name="heading"/> in docs/ingame-tests.md, without its backticks.
    /// </summary>
    private static string[] DocsTable(string heading)
    {
        var docs = RepoFiles.Read("docs/ingame-tests.md").Replace("\r\n", "\n");
        var section = docs.Substring(docs.IndexOf(heading + "\n", StringComparison.Ordinal));
        return [.. section.Split('\n')
            .SkipWhile(line => !line.StartsWith('|'))
            .TakeWhile(line => line.StartsWith('|'))
            .Skip(2)
            .Select(line => line.Split('|')[1].Trim().Replace("`", string.Empty))];
    }
}
