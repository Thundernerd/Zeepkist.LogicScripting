using System.Collections.Generic;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;
using Xunit;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Tests;

/// <summary>
/// <c>placed(...)</c> calls the compiler rejects, so the reference stands for nothing, though <see cref="Level"/> has
/// a block with the UID they're written with. <c>{0}</c> is that UID: <c>T</c>, a Simple Telemetry, for its outputs,
/// or <c>L</c>, a Lamp, for its inputs.
/// </summary>
internal static class RejectedPlacedCalls
{
    public static readonly Dictionary<string, PlacedBlock> Level = new()
    {
        ["T"] = new PlacedBlock(BlockManifest.SimpleTelemetry.DefaultStyle.BlockId),
        ["L"] = new PlacedBlock(BlockManifest.Lamp.DefaultStyle.BlockId),
        ["GT"] = new PlacedBlock(BlockManifest.SimpleTelemetry.DefaultStyle.BlockId, isInBlockGroup: true),
        ["GL"] = new PlacedBlock(BlockManifest.Lamp.DefaultStyle.BlockId, isInBlockGroup: true)
    };

    public static readonly string[] Calls =
    [
        "placed(\"G{0}\")",
        "placed(\"{0}\", 1)",
        "placed(\"{0}\", \"{0}\")",
        "placed(\"{0}\", mode: 1)",
        "placed(\"{0}\" & 1)",
    ];

    public static TheoryData<string> All => new(Calls);
}
