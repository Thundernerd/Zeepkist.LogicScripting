using System.Linq;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;
using Xunit;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Tests;

public class BlockTypeTests
{
    private static readonly BlockType _styled = new(
        "styled",
        BlockCategory.Operators,
        [new BlockStyle("normal", 1, 1), new BlockStyle("compact", 2, 1)],
        [new BlockConnector("a", 0), new BlockConnector("b", 1)],
        [new BlockConnector("out", 2)],
        [BlockSetting.Option("mode", "m", "one", "two")]);

    [Fact]
    public void Arguments_AreInputs_ThenSettings_ThenStyle_ThenHide()
    {
        Assert.Equal(
            [
                ("a", BlockArgumentKind.Input),
                ("b", BlockArgumentKind.Input),
                ("mode", BlockArgumentKind.Setting),
                ("style", BlockArgumentKind.Style),
                ("hide", BlockArgumentKind.Setting)
            ],
            _styled.Arguments.Select(a => (a.Name, a.Kind)));
    }

    [Fact]
    public void Arguments_DescribeWhatTheyAccept()
    {
        Assert.Equal(
            ["input", "input", "one, two", "normal, compact", BlockReference.DescribeValues(BlockSetting.Hide)],
            _styled.Arguments.Select(a => a.Detail));
    }

    [Fact]
    public void Arguments_OfASettingAreThatSetting()
    {
        Assert.Equal(
            [null, null, _styled.Settings[0], null, BlockSetting.Hide],
            _styled.Arguments.Select(a => a.Setting));
    }

    [Fact]
    public void Arguments_LabelSettingsAndStyleWithAColon()
    {
        Assert.Equal(["a", "b", "mode:", "style:", "hide:"], _styled.Arguments.Select(a => a.Label));
    }

    [Fact]
    public void FindClosestName_PrefersStyle_WhenAsCloseToHide()
    {
        Assert.Equal("style", _styled.FindClosestName("hyle"));
    }

    [Fact]
    public void Arguments_LeaveOutStyle_WhenTheTypeHasOneStyle()
    {
        Assert.DoesNotContain(BlockManifest.Logic.Arguments, a => a.Kind == BlockArgumentKind.Style);
    }

    [Fact]
    public void Arguments_OfAPlacedOnlyType_LeaveOutHide()
    {
        Assert.DoesNotContain(BlockManifest.Lamp.Arguments, a => a.Setting == BlockSetting.Hide);
    }
}
