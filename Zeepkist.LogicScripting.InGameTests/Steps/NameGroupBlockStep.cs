using System;
using System.Collections;
using System.Linq;
using TNRD.Zeepkist.LogicScripting.Blocks;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Names a block of a Logic Block type, by its Block Function's name, in a Script's Block Group, the first by UID when
/// there are several, so the scenario's later steps can use it as they use a block
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

        var type = context.RequireBlockType(Function);
        if (type == null)
            yield break;

        var group = BlockGroups.Find(central, scriptId);
        var block = group
            .Where(b => BlockManifest.GetPlacedType(b.blockID) == type)
            .OrderBy(b => b.UID, StringComparer.Ordinal)
            .FirstOrDefault();
        if (block == null)
        {
            if (group.Count == 0)
                context.Fail($"\"{Script}\" has no Block Group: add compile first", Function, "(no blocks)");
            else
                context.Fail($"\"{Script}\"'s Block Group has no {Function} block", Function, string.Join(", ", group.Select(b => BlockManifest.GetPlacedType(b.blockID).Function).Distinct()));

            yield break;
        }

        if (!context.PlacedBlocks.Remember(Name, block.UID))
        {
            context.FailNameTaken(Name);
            yield break;
        }

        context.Detail = $"{Function} in \"{Script}\"'s Block Group of {group.Count}, block {block.blockID}, UID \"{block.UID}\"";
        yield return null;
    }
}
