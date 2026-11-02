using System.Collections.Generic;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Syntax;

/// <summary>
/// <c>function name(inputs) -> outputs</c>, as the parser reads it. The editor reads it the same way, so it marks the
/// Functions whose first line the compiler rejects without compiling the Script.
/// </summary>
internal sealed class FunctionFirstLine
{
    public FunctionFirstLine(FunctionParameter name, List<FunctionParameter> inputs, List<FunctionParameter> outputs, Diagnostic error)
    {
        Name = name;
        Inputs = inputs;
        Outputs = outputs;
        Error = error;
    }

    /// <summary>
    /// Null when the line has no name to call the Function by.
    /// </summary>
    public FunctionParameter Name { get; }

    /// <summary>
    /// The names read before <see cref="Error"/>, when there is one.
    /// </summary>
    public List<FunctionParameter> Inputs { get; }

    /// <inheritdoc cref="Inputs"/>
    public List<FunctionParameter> Outputs { get; }

    /// <summary>
    /// The line's syntax error, or null when it reads the way the parser needs it.
    /// </summary>
    public Diagnostic Error { get; }
}
