using System;
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
    /// <param name="library"></param>
    /// <param name="findLibrary">A Library's text by its name, or null when there's none; null finds no Library.</param>
    public static IEnumerable<FunctionDefinition> GetCallable(string library, Func<string, string> findLibrary)
    {
        if (findLibrary == null)
            return [];

        var functions = GraphBuilder.DescribeLibrary(library, findLibrary, out var hasErrors);
        return hasErrors ? [] : functions.Select(f => f.Function);
    }

    /// <summary>
    /// Null when the Library has no callable Function by that name.
    /// </summary>
    public static FunctionDefinition Find(string library, string name, Func<string, string> findLibrary)
    {
        return GetCallable(library, findLibrary).FirstOrDefault(f => f.Name.Name == name);
    }
}
