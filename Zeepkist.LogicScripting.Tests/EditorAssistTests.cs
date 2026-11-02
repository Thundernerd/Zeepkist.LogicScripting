using BepInEx.Logging;
using TNRD.Zeepkist.LogicScripting.Blocks;
using TNRD.Zeepkist.LogicScripting.UI;
using Xunit;

namespace TNRD.Zeepkist.LogicScripting.Tests;

public class EditorAssistTests
{
    [Fact]
    public void ForALibrary_FindsNoPlacedBlock_WithoutLookingInTheLevel()
    {
        var assist = new EditorAssist(null, null, new StartupCheck(new ManualLogSource("Tests")));

        Assert.Null(assist.ForLibrary().FindPlacedBlock("lamp"));
    }
}
