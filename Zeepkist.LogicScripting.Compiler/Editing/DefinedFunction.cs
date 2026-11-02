using System.Collections.Generic;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Editing;

/// <summary>
/// A Function a Script defines, as the editor reads it.
/// </summary>
public sealed class DefinedFunction
{
    public DefinedFunction(
        string name,
        TextSpan span,
        IReadOnlyList<string> inputs,
        IReadOnlyList<string> outputs,
        TextSpan body,
        bool isRejected)
    {
        Name = name;
        Span = span;
        Inputs = inputs;
        Outputs = outputs;
        Body = body;
        IsRejected = isRejected;
    }

    /// <summary>
    /// Null when the first line has none; the compiler drops such a Function, but its body is still a scope of its own.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Where the name is written on the Function's first line, or the keyword when it has none.
    /// </summary>
    public TextSpan Span { get; }

    /// <summary>
    /// Read loosely, so they're there even when <see cref="IsRejected"/>.
    /// </summary>
    public IReadOnlyList<string> Inputs { get; }

    /// <inheritdoc cref="Inputs"/>
    public IReadOnlyList<string> Outputs { get; }

    /// <summary>
    /// Whether the compiler rejects the first line, so a call to the Function stands for nothing.
    /// </summary>
    public bool IsRejected { get; }

    /// <summary>
    /// The <see cref="Inputs"/> a call to the Function can give, which are none when <see cref="IsRejected"/>.
    /// </summary>
    public IReadOnlyList<string> UsableInputs => IsRejected ? [] : Inputs;

    /// <summary>
    /// The <see cref="Outputs"/> a call to the Function has, which are none when <see cref="IsRejected"/>.
    /// </summary>
    public IReadOnlyList<string> UsableOutputs => IsRejected ? [] : Outputs;

    /// <summary>
    /// From the line after the first line to the end of its <c>end</c>; while it has none, to the next Function or the
    /// end of the text.
    /// </summary>
    public TextSpan Body { get; }

    /// <summary>
    /// Whether <paramref name="index"/> is in the body, which sees only the Function's inputs and its own definitions.
    /// Right after its <c>end</c> counts, so the word being typed there is still the body's.
    /// </summary>
    public bool Contains(int index)
    {
        return Body.Start <= index && index <= Body.End;
    }
}
