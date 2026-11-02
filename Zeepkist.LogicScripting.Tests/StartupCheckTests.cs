using System.Linq;
using BepInEx.Logging;
using TNRD.Zeepkist.LogicScripting.Blocks;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;
using Xunit;

namespace TNRD.Zeepkist.LogicScripting.Tests;

public class StartupCheckTests
{
    private readonly StartupCheck _check = new(new ManualLogSource("Tests"));

    [Fact]
    public void TurnOff_AddsTheStyle_AsTheCheckDoes()
    {
        var type = BlockManifest.All.First(t => t.HasStyles);
        var style = type.Styles[1];

        Assert.True(_check.TurnOff(type, style));

        Assert.Equal([style.BlockId], _check.DisabledBlockIds);
        Assert.Equal([type.Describe(style)], _check.DisabledBlocks);
    }

    [Fact]
    public void TurnOff_AStyleThatIsAlreadyOff_ChangesNothing()
    {
        var type = BlockManifest.All.First(t => t.Function == "lamp");
        _check.TurnOff(type, type.DefaultStyle);

        Assert.False(_check.TurnOff(type, type.DefaultStyle));

        Assert.Single(_check.DisabledBlocks);
    }

    [Fact]
    public void TurnTestStylesBackOn_TurnsOnWhatTurnOffTurnedOff()
    {
        var type = BlockManifest.All.First(t => t.Function == "lamp");
        _check.TurnOff(type, type.DefaultStyle);

        _check.TurnTestStylesBackOn();

        Assert.Empty(_check.DisabledBlockIds);
        Assert.Empty(_check.DisabledBlocks);
    }
}
