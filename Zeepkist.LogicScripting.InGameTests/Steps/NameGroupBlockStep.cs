using System.Collections;
using System.Linq;
using TNRD.Zeepkist.LogicScripting.Blocks;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Names a block of a Logic Block type, by its Block Function's name, in a Script's Block Group, the first one the level
/// holds when there are several, so the scenario's later steps can use it as they use a block
/// <c>place-block ... as &lt;name&gt;</c> placed. No other step can reach a Block Group's block: Insert selected skips
/// them.
/// </summary>
internal sealed class NameGroupBlockStep : ScenarioStep
{
    public NameGroupBlockStep(string script, string function, string name)
    {
        Script = script;
        Function = function;
        Name = name;
    }

    public string Script { get; }
    public string Function { get; }
    public string Name { get; }

    public override IEnumerator Run(StepContext context)
    {
        var central = context.RequireEditor();
        if (central == null)
            yield break;

        var scriptId = context.Window.FindScriptId(Script);
        if (scriptId == null)
        {
            context.FailNoScript(Script);
            yield break;
        }

        if (BlockManifest.All.All(t => t.Function != Function))
        {
            context.Fail($"no Logic Block type is called '{Function}'");
            yield break;
        }

        var group = BlockGroups.Find(central, scriptId);
        var block = group.FirstOrDefault(b => BlockManifest.TryGetBlock(b.blockID, out var type, out _) && type.Function == Function);
        if (block == null)
        {
            var functions = group.Select(b => BlockManifest.GetPlacedType(b.blockID).Function).Distinct();
            context.Fail($"\"{Script}\"'s Block Group has no {Function} block: add compile first", Function, group.Count == 0 ? "(no blocks)" : string.Join(", ", functions));
            yield break;
        }

        if (!context.PlacedBlocks.Remember(Name, block.UID))
        {
            context.Fail($"a block was already placed or named as \"{Name}\"", "a new name", context.PlacedBlocks.Listed);
            yield break;
        }

        context.Detail = $"{Function} in \"{Script}\"'s Block Group of {group.Count}, block {block.blockID}, UID \"{block.UID}\"";
        yield return null;
    }
}
