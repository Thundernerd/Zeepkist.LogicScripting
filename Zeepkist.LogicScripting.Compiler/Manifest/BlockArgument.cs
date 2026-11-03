namespace TNRD.Zeepkist.LogicScripting.Compiler.Manifest;

/// <summary>
/// One argument a Block Function takes, as call help and completion list it.
/// </summary>
public sealed class BlockArgument
{
    public BlockArgument(string name, BlockArgumentKind kind, string detail, BlockSetting setting = null)
    {
        Name = name;
        Kind = kind;
        Detail = detail;
        Setting = setting;
    }

    public string Name { get; }
    public BlockArgumentKind Kind { get; }

    /// <summary>
    /// What it accepts, as the Reference tab describes it.
    /// </summary>
    public string Detail { get; }

    /// <summary>
    /// The Setting a <see cref="BlockArgumentKind.Setting"/> argument gives; null for any other kind.
    /// </summary>
    public BlockSetting Setting { get; }

    public bool IsInput => Kind == BlockArgumentKind.Input;

    public static BlockArgument ForSetting(BlockSetting setting)
    {
        return new BlockArgument(setting.Name, BlockArgumentKind.Setting, BlockReference.DescribeValues(setting), setting);
    }
}
