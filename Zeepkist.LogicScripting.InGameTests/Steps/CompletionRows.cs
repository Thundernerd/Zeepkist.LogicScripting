using System.Collections.Generic;
using System.Linq;
using TNRD.Zeepkist.LogicScripting.Compiler.Editing;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// How the completion steps write the list's rows in their results.
/// </summary>
internal static class CompletionRows
{
    public const string Closed = "(the list is closed)";

    public static string Describe(string label, string detail)
    {
        return detail.Length == 0 ? label : $"{label} ({detail})";
    }

    /// <summary>
    /// Every row, or <see cref="Closed"/> for none.
    /// </summary>
    public static string Describe(IReadOnlyList<CompletionItem> items)
    {
        return items.Count == 0 ? Closed : string.Join(", ", items.Select(i => Describe(i.Label, i.Detail)));
    }
}
