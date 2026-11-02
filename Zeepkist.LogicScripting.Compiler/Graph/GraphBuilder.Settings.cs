using System.Collections.Generic;
using System.Globalization;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;
using TNRD.Zeepkist.LogicScripting.Compiler.Syntax;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Graph;

internal sealed partial class GraphBuilder
{
    /// <summary>
    /// One per stored key, so several for a Toggler's targets. Null after reporting why the value is wrong.
    /// </summary>
    private IReadOnlyList<Setting> GetSettingValues(BlockSetting setting, NamedArgument argument)
    {
        switch (setting.Type)
        {
            case SettingType.Targets:
                return GetTargets(setting, argument);

            case SettingType.Number when argument.Value is not IntegerLiteral:
                ValueError(argument, $"{setting.Name} takes a whole number, like {setting.Name}: 5");
                return null;

            case SettingType.Option when argument.Value is not NameExpression:
                ValueError(argument, $"{setting.Name} takes a word: {string.Join(", ", setting.Options)}");
                return null;
        }

        var value = argument.Value switch
        {
            IntegerLiteral number => number.Value.ToString(CultureInfo.InvariantCulture),
            NameExpression name => name.Name,
            _ => null
        };

        var stored = setting.ToSetting(value, out var error);
        if (stored == null)
        {
            ValueError(argument, error);
            return null;
        }

        return [stored];
    }

    /// <summary>
    /// Null after reporting why the value is wrong.
    /// </summary>
    private IReadOnlyList<Setting> GetTargets(BlockSetting setting, NamedArgument argument)
    {
        var example = $"{setting.Name}: [door_1, door_2]";
        var items = argument.Value is ListExpression list ? list.Items : [argument.Value];
        if (items.Count == 0)
        {
            ValueError(argument, $"{setting.Name} takes 1 to {BlockSetting.MaxTargets} placed blocks, like {example}");
            return null;
        }

        var failed = false;
        if (items.Count > BlockSetting.MaxTargets)
        {
            Error(items[BlockSetting.MaxTargets], $"A Toggler switches at most {BlockSetting.MaxTargets} blocks");
            failed = true;
        }

        List<Setting> targets = [];
        HashSet<string> uids = [];
        foreach (var item in items)
        {
            var value = Evaluate(item);
            if (value is FailedValue)
            {
                failed = true;
            }
            else if (value.PlacedBlock is not { } block)
            {
                Error(item, $"'{Describe(item)}' isn't a placed block, and {setting.Name} takes only names defined with {PlacedFunction}(...)");
                failed = true;
            }
            else if (!uids.Add(block.Uid))
            {
                Error(item, $"'{Describe(item)}' is already a target");
                failed = true;
            }
            else
            {
                targets.Add(Setting.Text(setting.Key + uids.Count.ToString(CultureInfo.InvariantCulture), block.Uid));
            }
        }

        return failed ? null : targets;
    }
}
