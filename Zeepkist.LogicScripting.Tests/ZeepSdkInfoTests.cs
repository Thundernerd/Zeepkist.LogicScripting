using System;
using Xunit;
using ZeepSDK.Storage;

namespace TNRD.Zeepkist.LogicScripting.Tests;

public class ZeepSdkInfoTests
{
    // Plugin itself can't be loaded here without UnityEngine, so this checks the constant its BepInDependency uses
    [Fact]
    public void TheVersion_IsTheZeepSdkThePluginIsBuiltAgainst()
    {
        var built = typeof(StorageApi).Assembly.GetName().Version;

        Assert.Equal(built.ToString(3), new Version(ZeepSdkInfo.Version).ToString(3));
    }
}
