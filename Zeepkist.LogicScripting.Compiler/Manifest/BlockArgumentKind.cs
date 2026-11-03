namespace TNRD.Zeepkist.LogicScripting.Compiler.Manifest;

public enum BlockArgumentKind
{
    /// <summary>
    /// An input Connector, which a positional argument or a named one can wire.
    /// </summary>
    Input,

    /// <summary>
    /// One of the type's Settings, or <c>hide:</c>; only given by name.
    /// </summary>
    Setting,

    /// <summary>
    /// <c>style:</c>, which picks one of the type's styles; only given by name.
    /// </summary>
    Style
}
