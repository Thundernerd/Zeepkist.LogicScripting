using System;

namespace TNRD.Zeepkist.LogicScripting.Compiler.Editing;

/// <summary>
/// What the editor's completion, call help, hover and definition jump read from outside the Script's text.
/// </summary>
public sealed class EditorLookups
{
    /// <summary>
    /// Finds no block in the level and no Library.
    /// </summary>
    public static readonly EditorLookups None = new();

    private readonly Func<string, PlacedBlock> _findPlacedBlock;
    private readonly Func<string, string> _findLibrary;

    /// <param name="findPlacedBlock">
    /// The block in the level with a UID, or null when there's none; it gives a Placed Block Reference its type. Null
    /// finds no block.
    /// </param>
    /// <param name="findLibrary">
    /// A Library's text by its name, or null when there's none; it gives a Library its Functions, and a Library's
    /// Function its inputs and outputs. Null finds no Library.
    /// </param>
    public EditorLookups(Func<string, PlacedBlock> findPlacedBlock = null, Func<string, string> findLibrary = null)
    {
        _findPlacedBlock = findPlacedBlock;
        _findLibrary = findLibrary;
    }

    /// <summary>
    /// Null when there's no block in the level with <paramref name="uid"/>.
    /// </summary>
    public PlacedBlock FindPlacedBlock(string uid)
    {
        return _findPlacedBlock?.Invoke(uid);
    }

    /// <summary>
    /// Null when there's no Library by <paramref name="name"/>.
    /// </summary>
    public string FindLibrary(string name)
    {
        return _findLibrary?.Invoke(name);
    }
}
