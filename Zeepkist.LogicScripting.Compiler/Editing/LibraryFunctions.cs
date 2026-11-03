using System.Collections.Generic;
using System.Linq;
using TNRD.Zeepkist.LogicScripting.Compiler.Graph;
using TNRD.Zeepkist.LogicScripting.Compiler.Syntax;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Editing;

/// <summary>
/// The Functions a Script can call in a Library, which the editor offers and describes. None come from a Library with
/// an error, since the compiler rejects every call into it.
/// </summary>
internal static class LibraryFunctions
{
    public static IEnumerable<FunctionDefinition> GetCallable(string library, EditorLookups lookups)
    {
        var functions = GraphBuilder.DescribeLibrary(library, lookups.FindLibrary, out var hasErrors);
        return hasErrors ? [] : functions.Select(f => f.Function);
    }

    /// <summary>
    /// Null when the Library has no callable Function by that name.
    /// </summary>
    public static FunctionDefinition Find(string library, string name, EditorLookups lookups)
    {
        return GetCallable(library, lookups).FirstOrDefault(f => f.Name.Name == name);
    }
}
