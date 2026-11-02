using Xunit;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Tests;

/// <summary>
/// First lines of a Function <c>f</c> the compiler rejects, so a call to it stands for nothing.
/// </summary>
internal static class RejectedFirstLines
{
    public static readonly string[] Lines =
    [
        "function f(a) -> hi, hi",
        "function f(a) -> hi, a",
        "function f(a) -> hi, lo extra",
        "function f(a) -> hi, lo,",
        "function f(a) -> hi, , lo",
        "function f(a, a) -> hi, lo",
        "function f(a b) -> hi, lo",
        "function f(a,) -> hi, lo",
        "function f(a -> hi, lo",
        "function f(a) extra -> hi, lo",
        "function f(a) -> hi, logic",
        "function f(a, true) -> hi, lo",
        "function f(a) -> hi, g",
        "function f(a, f) -> hi, lo",
    ];

    public static TheoryData<string> All => new(Lines);
}
