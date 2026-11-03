using System;
using System.Collections.Generic;
using System.Linq;
using TNRD.Zeepkist.LogicScripting.Compiler.Editing;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;
using Xunit;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Tests;

/// <summary>
/// Texts mark the caret with <c>|</c>.
/// </summary>
public class CallHelpTests
{
    private static readonly Dictionary<string, PlacedBlock> _level = new()
    {
        ["L"] = new PlacedBlock(BlockManifest.Lamp.DefaultStyle.BlockId),
        ["OTHER"] = new PlacedBlock(1)
    };

    [Fact]
    public void Items_InputsInOrder_ThenSettings_ThenHide()
    {
        var help = At("x = logic(|");

        Assert.Equal("logic", help.Callee);
        Assert.Equal(["a", "b", "op_in", "op:", "hide:"], help.Items.Select(i => i.Label));
        Assert.Equal("input", help.Items[0].Detail);
        Assert.Equal(BlockReference.DescribeValues(BlockSetting.Hide), help.Items[4].Detail);
    }

    [Fact]
    public void Items_IncludeStyle_WhenTheTypeHasStyles()
    {
        Assert.Equal(["x", "style:", "hide:"], At("power_display(|").Items.Select(i => i.Label));
    }

    [Fact]
    public void Items_ForAnAlias_AreItsBlockFunctions()
    {
        var help = At("x = mux(|");

        Assert.Equal("mux", help.Callee);
        Assert.Equal(At("x = multiplexer(|").Items.Select(i => i.Label), help.Items.Select(i => i.Label));
    }

    [Theory]
    [InlineData("lamp_1 = placed(\"L\")\nlamp_1(|")]
    [InlineData("lamp_1 = placed((\"L\"))\nlamp_1(|")]
    public void Items_ForAPlacedBlockReference_OnlyItsInputs(string text)
    {
        var help = At(text, uid => _level.GetValueOrDefault(uid));

        Assert.Equal(["intensity", "red", "green", "blue", "range", "angle"], help.Items.Select(i => i.Label));
    }

    [Fact]
    public void Items_ForANameDefinedTwice_TheFirstDefinitionsInputs()
    {
        var help = At("lamp_1 = placed(\"L\")\nlamp_1 = logic(1, 2)\nlamp_1(|", uid => _level.GetValueOrDefault(uid));

        Assert.Equal("intensity", help.Items[0].Label);
    }

    [Fact]
    public void Items_ForAPlacedBlockReference_NotAFunctionsInputOfTheSameName()
    {
        var help = At("d = placed(\"L\")\nfunction g(d)\nend\nd(|", uid => _level.GetValueOrDefault(uid));

        Assert.Equal("intensity", help.Items[0].Label);
    }

    [Fact]
    public void Items_ForAPlacedBlockReference_OnlyInItsOwnScope()
    {
        Assert.Null(At("d = placed(\"L\")\nfunction g()\n    d(|\nend", uid => _level.GetValueOrDefault(uid)));
    }

    [Theory]
    [InlineData("function edge(x, n) -> y\nend\nz = edge(|")]
    [InlineData("z = edge(|\nfunction edge(x, n) -> y\nend")]
    public void Items_ForAFunction_ItsInputs(string text)
    {
        var help = At(text);

        Assert.Equal(["x", "n"], help.Items.Select(i => i.Label));
        Assert.All(help.Items, i => Assert.True(i.IsInput));
        Assert.Equal(0, help.Active);
    }

    [Theory]
    [InlineData("x = logic(a, |", "b")]
    [InlineData("x = logic(a, |)", "b")]
    [InlineData("x = logic(a, b|)", "b")]
    [InlineData("x = logic(|a, b)", "a")]
    [InlineData("x = logic(a|, b)", "a")]
    [InlineData("x = logic(a, op: |", "op:")]
    [InlineData("x = logic(a, op: an|", "op:")]
    [InlineData("x = logic(a, o|p: and)", "op:")]
    [InlineData("x = logic(a, |op: and)", "op:")]
    [InlineData("x = logic(a, (1 & 2), |", "op_in")]
    [InlineData("x = logic(a, 1 & |", "b")]
    [InlineData("x = logic(a, (1, |", "b")]
    [InlineData("x = logic(a, b, op_in: |", "op_in")]
    [InlineData("x = logic(a, op|", "b")]
    [InlineData("x = logic(\n    a,\n    |", "b")]
    [InlineData("x = logic(a,\n    op: |", "op:")]
    [InlineData("toggler(1, targets: [|", "targets:")]
    [InlineData("toggler(1, targets: [d, |", "targets:")]
    [InlineData("x = logic(logic(1, 2), |", "b")]
    [InlineData("x = logic([1, (2)], |", "b")]
    [InlineData("x = logic(a, logic(1, 2), op: |", "op:")]
    [InlineData("x = logic(logic(1, |), 2)", "b")]
    [InlineData("x = logic(1, logic(|", "a")]
    [InlineData("function f(a, b) -> c\nend\nx = f(1, |", "b")]
    public void Active_TheInputOrSettingTheArgumentAtTheCaretGives(string text, string active)
    {
        var help = At(text);

        Assert.Equal(active, help.Items[help.Active].Label);
    }

    [Theory]
    [InlineData("x = logic(a, b, c, |")]
    [InlineData("x = logic(a, color: |")]
    [InlineData("x = logic(a, hide: true, |")]
    [InlineData("x = logic(op: and, a, |")]
    [InlineData("x = logic(a, b: 1, |")]
    public void Active_None_PastTheLastInputForAnUnknownNameOrAfterANamedArgument(string text)
    {
        var help = At(text);

        Assert.NotNull(help);
        Assert.Equal(-1, help.Active);
    }

    [Fact]
    public void Active_ALaterLinesNamedArgumentIsntTheCaretsArgument()
    {
        var help = At("x = logic(a, |\n    op: and)");

        Assert.Equal("b", help.Items[help.Active].Label);
    }

    [Theory]
    [InlineData("x = logic(a, delay(|", "delay", "x")]
    [InlineData("x = logic(a, delay(b, ticks: |", "delay", "ticks:")]
    [InlineData("x = logic(a, delay(b, ticks: 1), |", "logic", "op_in")]
    [InlineData("x = logic(a, delay(b)|", "logic", "b")]
    [InlineData("x = logic(a, nothing(|", "logic", "b")]
    public void NestedCalls_TheInnermostOne(string text, string callee, string active)
    {
        var help = At(text);

        Assert.Equal(callee, help.Callee);
        Assert.Equal(active, help.Items[help.Active].Label);
    }

    [Theory]
    [InlineData("x = logic|(a)")]
    [InlineData("x = logic(a)|")]
    [InlineData("x = |")]
    [InlineData("x = (|")]
    [InlineData("x = logic(a, # b|")]
    [InlineData("x = placed(\"L|\")")]
    [InlineData("x = placed(|")]
    [InlineData("x = trigger(|")]
    [InlineData("x = nothing(|")]
    [InlineData("x = logic(\ny = |")]
    [InlineData("a = logic(1, 2)\na(|")]
    [InlineData("a = placed(\"UNKNOWN\")\na(|")]
    [InlineData("a = placed(\"OTHER\")\na(|")]
    [InlineData("a(|\na = placed(\"L\")")]
    public void None_OutsideACallThatHasHelp(string text)
    {
        Assert.Null(At(text, uid => _level.GetValueOrDefault(uid)));
    }

    [Theory]
    [MemberData(nameof(RejectedPlacedCalls.All), MemberType = typeof(RejectedPlacedCalls))]
    public void None_ForAPlacedCallTheCompilerRejects(string call)
    {
        Assert.Null(At($"l = {string.Format(call, "L")}\nl(|", RejectedPlacedCalls.Level.GetValueOrDefault));
    }

    [Theory]
    [MemberData(nameof(RejectedFirstLines.All), MemberType = typeof(RejectedFirstLines))]
    public void None_ForAFunctionWhoseFirstLineTheCompilerRejects(string firstLine)
    {
        Assert.Null(At($"{firstLine}\nend\nfunction g()\nend\nx = f(|"));
    }

    [Fact]
    public void None_ForAFunctionDefinedTwice_WhenTheCompilerRejectsTheFirstDefinitionsFirstLine()
    {
        Assert.Null(At("function f(a, a)\nend\nfunction f(b)\nend\nx = f(|"));
    }

    [Fact]
    public void NestedCalls_InACallToAFunctionWhoseFirstLineTheCompilerRejects_TheCallAroundIt()
    {
        Assert.Equal("logic", At("function f(a, a)\nend\nx = logic(1, f(|").Callee);
    }

    [Fact]
    public void None_ForAQualifiedCall_EvenWhenTheScriptDefinesItsFunction()
    {
        Assert.Null(At("function f(a, b) -> c\nend\nx = timing.f(|"));
    }

    [Fact]
    public void NestedCalls_InAQualifiedCall_TheCallAroundIt()
    {
        Assert.Equal("logic", At("function f(a, b) -> c\nend\nx = logic(timing.f(|").Callee);
    }

    [Theory]
    [InlineData("x = timing.edge(1, |", 1)]
    [InlineData("x = timing.edge(|", 0)]
    [InlineData("x = timing.edge(b: |", 1)]
    [InlineData("function edge(x)\nend\nx = timing.edge(1, |", 1)]
    [InlineData("x = logic(timing.edge(1, |", 1)]
    public void Items_ForALibrarysFunction_ItsInputs(string text, int active)
    {
        var help = At(text, findLibrary: Library(TwoInputLibrary));

        Assert.Equal("edge", help.Callee);
        Assert.Equal(["a", "b"], help.Items.Select(i => i.Label));
        Assert.All(help.Items, i => Assert.True(i.IsInput));
        Assert.Equal(active, help.Active);
    }

    [Theory]
    [InlineData("x = timing.edge(|", TwoInputLibrary + "\nx = 1")]
    [InlineData("x = timing.other(|", TwoInputLibrary)]
    [InlineData("x = nothing.edge(|", TwoInputLibrary)]
    [InlineData("x = a.timing.edge(|", TwoInputLibrary)]
    [InlineData("x = timing .edge(|", TwoInputLibrary)]
    [InlineData("x = timing.pulse(|", "function pulse() -> b\n  b = true\nend")]
    public void None_ForALibrarysFunction_FromALibraryWithAnErrorOrWithoutInputs(string text, string libraryText)
    {
        Assert.Null(At(text, findLibrary: Library(libraryText)));
    }

    [Fact]
    public void NestedCalls_InACallToALibraryWithAnError_TheCallAroundIt()
    {
        Assert.Equal("logic", At("x = logic(timing.edge(|", findLibrary: Library(TwoInputLibrary + "\nx = 1")).Callee);
    }

    [Fact]
    public void Session_ForALibrarysFunction_ItsInputs()
    {
        var session = new CallHelpSession(findLibrary: Library(TwoInputLibrary));

        session.Update(Snapshot("x = timing.edge(1, |"), focused: true);

        Assert.Equal("edge", session.Help.Callee);
    }

    [Fact]
    public void Session_Dismissed_InACallToALibrarysFunction_StaysHiddenWhileTheCaretIsInIt()
    {
        var session = new CallHelpSession(findLibrary: Library(TwoInputLibrary));
        session.Update(Snapshot("x = timing.edge(|"), focused: true);
        session.Dismiss();

        session.Update(Snapshot("x = timing.edge(1, |"), focused: true);

        Assert.Null(session.Help);
    }

    [Fact]
    public void OpenParen_IsTheCallsParenthesis()
    {
        Assert.Equal(9, At("x = logic(a, |").OpenParen);
    }

    [Fact]
    public void Session_ShowsTheHelpWhileFocused()
    {
        var session = new CallHelpSession();

        session.Update(Snapshot("x = logic(a, |"), focused: true);
        Assert.Equal("logic", session.Help.Callee);

        session.Update(Snapshot("x = logic(a, |"), focused: false);
        Assert.Null(session.Help);
    }

    [Fact]
    public void Session_Dismiss_HidesUntilTheCaretLeavesTheCall()
    {
        var session = new CallHelpSession();
        session.Update(Snapshot("x = logic(a, |"), focused: true);

        session.Dismiss();
        Assert.Null(session.Help);

        session.Update(Snapshot("x = logic(a, b|"), focused: true);
        Assert.Null(session.Help);

        session.Update(Snapshot("x = logic(a, b)|"), focused: true);
        Assert.Null(session.Help);

        session.Update(Snapshot("x = logic(a, b|)"), focused: true);
        Assert.Equal("logic", session.Help.Callee);
    }

    [Fact]
    public void Session_Dismiss_StaysHiddenInACallInsideTheDismissedOne()
    {
        var session = new CallHelpSession();
        session.Update(Snapshot("x = logic(a, |"), focused: true);
        session.Dismiss();

        session.Update(Snapshot("x = logic(a, delay(|"), focused: true);
        Assert.Null(session.Help);

        session.Update(Snapshot("x = logic(a, delay(b)|"), focused: true);
        Assert.Null(session.Help);
    }

    [Fact]
    public void Session_Dismiss_ShowsAnotherCallsHelp()
    {
        var session = new CallHelpSession();
        session.Update(Snapshot("x = logic(1, 2)\ny = logic(a, delay(|"), focused: true);
        session.Dismiss();

        session.Update(Snapshot("x = logic(1, |2)\ny = logic(a, delay("), focused: true);

        Assert.Equal("logic", session.Help.Callee);
    }

    [Fact]
    public void Session_Dismiss_ShowsANewCallWithItsParenAtTheSamePlace()
    {
        var session = new CallHelpSession();
        session.Update(Snapshot("v = logic(|"), focused: true);
        session.Dismiss();

        session.Update(Snapshot("w = logic(|"), focused: true);

        Assert.Equal("logic", session.Help.Callee);
    }

    [Fact]
    public void Session_Dismiss_ShowsANewCalleeWithItsParenAtTheSamePlace()
    {
        var session = new CallHelpSession();
        session.Update(Snapshot("v = logic(|"), focused: true);
        session.Dismiss();

        session.Update(Snapshot("v = delay(|"), focused: true);

        Assert.Equal("delay", session.Help.Callee);
    }

    [Fact]
    public void Session_Dismiss_ForgetsTheCallWhenALineAboveChanges()
    {
        var session = new CallHelpSession();
        session.Update(Snapshot("a = 1\nv = logic(|"), focused: true);
        session.Dismiss();

        session.Update(Snapshot("b = 1\nv = logic(|"), focused: true);

        Assert.Equal("logic", session.Help.Callee);
    }

    [Fact]
    public void Session_Dismiss_WithoutHelpHidesNothing()
    {
        var session = new CallHelpSession();
        session.Update(Snapshot("x = 1|"), focused: true);
        session.Dismiss();

        session.Update(Snapshot("x = logic(|"), focused: true);

        Assert.Equal("logic", session.Help.Callee);
    }

    private const string TwoInputLibrary = "function edge(a, b) -> lo\n  lo = a & b\nend";

    private static CallHelp At(string marked, Func<string, PlacedBlock> findPlacedBlock = null, Func<string, string> findLibrary = null)
    {
        var snapshot = Snapshot(marked);
        return CallHelp.At(snapshot.Text, snapshot.Caret, findPlacedBlock, findLibrary);
    }

    /// <summary>
    /// A lookup with one Library, <c>timing</c>.
    /// </summary>
    private static Func<string, string> Library(string text)
    {
        return name => name == "timing" ? text : null;
    }

    private static TextSnapshot Snapshot(string marked)
    {
        var caret = marked.IndexOf('|');
        return new TextSnapshot(marked.Remove(caret, 1), caret);
    }
}
