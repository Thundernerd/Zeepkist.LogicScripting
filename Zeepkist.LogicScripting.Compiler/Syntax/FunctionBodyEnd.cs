namespace TNRD.Zeepkist.LogicScripting.Compiler.Syntax;

/// <summary>
/// Where a Function's body ends, and why.
/// </summary>
internal readonly struct FunctionBodyEnd
{
    public FunctionBodyEnd(int index, FunctionBodyEndReason reason)
    {
        Index = index;
        Reason = reason;
    }

    /// <summary>
    /// The token the body ends at: its <c>end</c>, the next Function's keyword, or the end of the Script.
    /// </summary>
    public int Index { get; }

    public FunctionBodyEndReason Reason { get; }
}
