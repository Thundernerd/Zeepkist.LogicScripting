using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Manifest;

/// <summary>
/// A Setting a block type takes as a named argument, and the key it's saved under.
/// </summary>
public sealed class BlockSetting
{
    /// <summary>
    /// <c>hide:</c>, which every Block Function takes: hides the block and its Wires in game.
    /// </summary>
    public static readonly BlockSetting Hide = Bool("hide", "lbhd");

    public const int MaxTargets = 8;

    private BlockSetting(
        string name,
        string key,
        SettingType type,
        IReadOnlyList<string> options,
        bool isRequired,
        long minimum = long.MinValue,
        long maximum = long.MaxValue)
    {
        Name = name;
        Key = key;
        Type = type;
        Options = options;
        IsRequired = isRequired;
        Minimum = minimum;
        Maximum = maximum;
    }

    public string Name { get; }
    public string Key { get; }
    public SettingType Type { get; }

    /// <summary>
    /// The option words in stored order; empty unless <see cref="Type"/> is <see cref="SettingType.Option"/> or
    /// <see cref="SettingType.Player"/>.
    /// </summary>
    public IReadOnlyList<string> Options { get; }

    /// <summary>
    /// Whether a call must give it, because the block's own value is known to break the level.
    /// </summary>
    public bool IsRequired { get; }

    /// <summary>
    /// The lowest number a <see cref="SettingType.Number"/> Setting accepts, for values known to break the level or
    /// that the block wouldn't keep.
    /// </summary>
    public long Minimum { get; }

    /// <summary>
    /// The highest number a <see cref="SettingType.Number"/> Setting accepts, for values the block wouldn't keep.
    /// </summary>
    public long Maximum { get; }

    /// <summary>
    /// The words a Script can give it, for completion: none for a number, and null for placed blocks, which are
    /// names the Script defines.
    /// </summary>
    public IReadOnlyList<string> Values => Type switch
    {
        SettingType.Bool => ["true", "false"],
        SettingType.Option or SettingType.Player => Options,
        SettingType.Targets => null,
        _ => []
    };

    public static BlockSetting Number(
        string name,
        string key,
        bool isRequired = false,
        long minimum = long.MinValue,
        long maximum = long.MaxValue)
    {
        return new BlockSetting(name, key, SettingType.Number, [], isRequired, minimum, maximum);
    }

    public static BlockSetting Bool(string name, string key)
    {
        return new BlockSetting(name, key, SettingType.Bool, [], false);
    }

    public static BlockSetting Option(string name, string key, params string[] options)
    {
        return new BlockSetting(name, key, SettingType.Option, options, false);
    }

    /// <summary>
    /// <c>player:</c>, with the players in the order the editor lists them; <paramref name="hasEveryone"/> when the
    /// list starts with everyone.
    /// </summary>
    public static BlockSetting Player(string key, bool hasEveryone)
    {
        string[] options = hasEveryone ? ["all", "1", "2", "3", "4"] : ["1", "2", "3", "4"];
        return new BlockSetting("player", key, SettingType.Player, options, false);
    }

    /// <summary>
    /// A required list of Placed Block References, stored as one key per target: the prefix and 1, 2, ….
    /// </summary>
    public static BlockSetting Targets(string name, string keyPrefix)
    {
        return new BlockSetting(name, keyPrefix, SettingType.Targets, [], true);
    }

    /// <summary>
    /// The value as the block stores it, from the word or whole number a Script gives it, such as
    /// <c>global_velocity</c>, <c>true</c> or <c>5</c>. Null, with why in <paramref name="error"/>, for a value the
    /// Setting doesn't take: only an option's error names the value. Not for <see cref="SettingType.Targets"/>, which
    /// takes a list of placed blocks.
    /// </summary>
    public Setting ToSetting(string value, out string error)
    {
        error = null;
        switch (Type)
        {
            case SettingType.Number:
                if (!long.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var number))
                    error = $"{Name} takes a whole number";
                else if (number < Minimum)
                    error = $"{Name} must be at least {Minimum}";
                else if (number > Maximum)
                    error = $"{Name} must be at most {Maximum}";

                return error == null ? Setting.Text(Key, number.ToString(CultureInfo.InvariantCulture)) : null;

            case SettingType.Bool:
                if (value is "true" or "false")
                    return Setting.Int(Key, value == "true" ? 1 : 0);

                error = $"{Name} takes true or false";
                return null;

            case SettingType.Player:
                if (IndexOf(value) >= 0)
                    return Choose(value);

                error = $"{Name} takes {string.Join(", ", Options.Take(Options.Count - 1))} or {Options[Options.Count - 1]}";
                return null;

            case SettingType.Targets:
                throw new InvalidOperationException($"{Name} takes a list of placed blocks, not one value");

            default:
                if (IndexOf(value) >= 0)
                    return Choose(value);

                error = $"'{value}' isn't an option of {Name}. Use {string.Join(", ", Options)}";
                return null;
        }
    }

    public Setting Choose(string option)
    {
        var index = IndexOf(option);
        if (index < 0)
            throw new ArgumentException($"'{option}' isn't an option of {Name}", nameof(option));

        return Setting.Int(Key, index);
    }

    public int IndexOf(string option)
    {
        for (var i = 0; i < Options.Count; i++)
        {
            if (Options[i] == option)
                return i;
        }

        return -1;
    }
}
