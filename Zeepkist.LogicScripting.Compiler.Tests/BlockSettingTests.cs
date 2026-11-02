using System;
using System.Linq;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;
using Xunit;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Tests;

public class BlockSettingTests
{
    [Theory]
    [InlineData("basic", 0)]
    [InlineData("global_velocity", 2)]
    public void ToSetting_AnOption_IsStoredAsItsIndex(string value, int index)
    {
        var setting = BlockManifest.SimpleTelemetryMode.ToSetting(value, out var error);

        Assert.Null(error);
        Assert.Equal(("xbs", SettingKind.Int, index), (setting.Key, setting.Kind, setting.IntValue));
    }

    [Theory]
    [InlineData("true", 1)]
    [InlineData("false", 0)]
    public void ToSetting_ABool_IsStoredAsOneOrZero(string value, int stored)
    {
        var setting = BlockManifest.DigitizerInvert.ToSetting(value, out var error);

        Assert.Null(error);
        Assert.Equal(("xt1", SettingKind.Int, stored), (setting.Key, setting.Kind, setting.IntValue));
    }

    [Fact]
    public void ToSetting_APlayer_IsStoredAsItsIndex()
    {
        var setting = BlockManifest.PlayerOrEveryone.ToSetting("2", out var error);

        Assert.Null(error);
        Assert.Equal(("xpr", SettingKind.Int, 2), (setting.Key, setting.Kind, setting.IntValue));
    }

    [Theory]
    [InlineData("5", "5")]
    [InlineData("-12", "-12")]
    public void ToSetting_ANumber_IsStoredAsText(string value, string stored)
    {
        var setting = BlockManifest.RandomMinimum.ToSetting(value, out var error);

        Assert.Null(error);
        Assert.Equal(("xv1", SettingKind.Text, stored), (setting.Key, setting.Kind, setting.TextValue));
    }

    [Theory]
    [InlineData("mode", "fast", "'fast' isn't an option of mode. Use basic, local_velocity, global_velocity")]
    [InlineData("invert", "yes", "invert takes true or false")]
    [InlineData("invert", null, "invert takes true or false")]
    [InlineData("player", "5", "player takes all, 1, 2, 3 or 4")]
    [InlineData("player", null, "player takes all, 1, 2, 3 or 4")]
    [InlineData("min", "1.5", "min takes a whole number")]
    [InlineData("ticks", "0", "ticks must be at least 1")]
    [InlineData("ticks", "99999999999", "ticks must be at most 2147483647")]
    public void ToSetting_AValueTheSettingDoesNotTake_SaysWhatItTakes(string name, string value, string error)
    {
        var setting = new[]
        {
            BlockManifest.SimpleTelemetryMode,
            BlockManifest.DigitizerInvert,
            BlockManifest.PlayerOrEveryone,
            BlockManifest.RandomMinimum,
            BlockManifest.Delay.FindSetting("ticks"),
        }.Single(s => s.Name == name);

        Assert.Null(setting.ToSetting(value, out var actual));
        Assert.Equal(error, actual);
    }

    [Fact]
    public void ToSetting_ATogglersTargets_IsNotOneValue()
    {
        var targets = BlockManifest.All.SelectMany(t => t.Settings).First(s => s.Type == SettingType.Targets);

        Assert.Throws<InvalidOperationException>(() => targets.ToSetting("a", out _));
    }
}
