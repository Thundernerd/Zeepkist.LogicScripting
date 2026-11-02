using System.Collections;
using System.Globalization;
using System.Linq;
using TNRD.Zeepkist.LogicScripting.Blocks;
using TNRD.Zeepkist.LogicScripting.Compiler;
using TNRD.Zeepkist.LogicScripting.Compiler.Manifest;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Changes a Setting of a block <c>place-block ... as &lt;name&gt;</c> placed, as changing it in the block's own
/// settings panel does: through the block's editor, so the change is one undo step and the level's block holds it. The
/// Setting and its value are written as a Script writes them, such as <c>mode global_velocity</c>.
/// </summary>
internal sealed class SetBlockSettingStep : ScenarioStep
{
    public SetBlockSettingStep(string name, string setting, string value)
    {
        Name = name;
        Setting = setting;
        Value = value;
    }

    public string Name { get; }
    public string Setting { get; }
    public string Value { get; }

    /// <summary>
    /// The value as the block stores it, as a Compile would write it. Null, with why in <paramref name="error"/>, for a
    /// value the Setting doesn't take.
    /// </summary>
    public static Setting ToSetting(BlockSetting setting, string value, out string error)
    {
        error = null;
        switch (setting.Type)
        {
            case SettingType.Number:
                if (!long.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var number))
                    error = $"{setting.Name} takes a whole number, not '{value}'";
                else if (number < setting.Minimum)
                    error = $"{setting.Name} must be at least {setting.Minimum}, not '{value}'";
                else if (number > setting.Maximum)
                    error = $"{setting.Name} must be at most {setting.Maximum}, not '{value}'";

                return error == null ? Compiler.Setting.Text(setting.Key, number.ToString(CultureInfo.InvariantCulture)) : null;

            case SettingType.Bool:
                if (value is "true" or "false")
                    return Compiler.Setting.Int(setting.Key, value == "true" ? 1 : 0);

                error = $"{setting.Name} takes true or false, not '{value}'";
                return null;

            case SettingType.Player:
                if (setting.IndexOf(value) >= 0)
                    return setting.Choose(value);

                var players = string.Join(", ", setting.Options.Take(setting.Options.Count - 1));
                error = $"{setting.Name} takes {players} or {setting.Options[setting.Options.Count - 1]}, not '{value}'";
                return null;

            case SettingType.Targets:
                error = $"set-block-setting can't set {setting.Name}, a list of blocks";
                return null;

            default:
                if (setting.IndexOf(value) >= 0)
                    return setting.Choose(value);

                error = $"'{value}' isn't an option of {setting.Name}. Use {string.Join(", ", setting.Options)}";
                return null;
        }
    }

    public override IEnumerator Run(StepContext context)
    {
        var central = context.RequireEditor();
        if (central == null)
            yield break;

        if (!context.PlacedBlocks.TryGetUid(Name, out var uid))
        {
            context.Fail($"no block was placed as \"{Name}\": add place-block ... as {Name} first", "a placed block's name", context.PlacedBlocks.Listed);
            yield break;
        }

        var block = LevelBlocks.Find(central, uid);
        if (block == null)
        {
            context.Fail($"the level has no block with \"{Name}\"'s UID \"{uid}\" any more");
            yield break;
        }

        if (!BlockManifest.TryGetBlock(block.blockID, out var type, out _))
        {
            context.Fail($"\"{Name}\", block {block.blockID}, isn't a Logic Block type the mod knows");
            yield break;
        }

        var setting = type.FindSetting(Setting);
        if (setting == null)
        {
            var names = type.Settings.Select(s => s.Name).Append(BlockSetting.Hide.Name);
            context.Fail($"{type.Function} has no Setting called '{Setting}'", string.Join(", ", names), Setting);
            yield break;
        }

        var stored = ToSetting(setting, Value, out var error);
        if (stored == null)
        {
            context.Fail(error);
            yield break;
        }

        var blockEdit = block.GetAllBlockEditV18sFromThisBlock().FirstOrDefault(e => e.IsLogicBlock());
        if (blockEdit == null)
        {
            context.Fail($"\"{Name}\" has no settings panel to change its Settings in");
            yield break;
        }

        // What the settings panel's slots do: note the block for undo, write the value, then let the block's editor
        // load it, redraw an open panel and register the undo step
        var before = central.undoRedo.currentHistoryPosition;
        var properties = blockEdit.GetCustomBlockProperties();
        blockEdit.RegisterUndoRedo();
        if (stored.Kind == SettingKind.Int)
            properties.SetIntDictionaryValue(stored.Key, stored.IntValue);
        else
            properties.SetStringDictionaryValue(stored.Key, stored.TextValue);

        blockEdit.GetUpdateFromSlot(0, updateSlots: true);
        yield return null;

        var kept = stored.Kind == SettingKind.Int
            ? block.customBlockEditProperties.GetIntDictionaryValue(stored.Key).ToString(CultureInfo.InvariantCulture)
            : block.customBlockEditProperties.GetStringDictionaryValue(stored.Key);
        var expected = stored.Kind == SettingKind.Int ? stored.IntValue.ToString(CultureInfo.InvariantCulture) : stored.TextValue;
        if (kept != expected)
        {
            context.Fail($"the block didn't keep {Setting} {Value} under \"{stored.Key}\"", expected, kept);
            yield break;
        }

        context.Detail = $"{type.Function} \"{Name}\": {Setting} {Value}, stored as \"{stored.Key}\" = {expected}, "
            + $"history position {before} → {central.undoRedo.currentHistoryPosition}";
    }
}
