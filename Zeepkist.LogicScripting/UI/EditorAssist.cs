using System;
using System.Linq;
using BepInEx.Configuration;
using TNRD.Zeepkist.LogicScripting.Blocks;
using TNRD.Zeepkist.LogicScripting.Compiler;
using TNRD.Zeepkist.LogicScripting.Compiler.Storage;
using ZeepSDK.Utilities;

namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// What the Script editor's help reads from outside the editor, shared by every Script's editor, and in its
/// <see cref="ForLibrary"/> form by every Library's.
/// </summary>
internal sealed class EditorAssist
{
    private readonly ConfigEntry<bool> _completionWhileTyping;
    private readonly StartupCheck _startupCheck;
    private readonly Func<string, PlacedBlock> _findPlacedBlock;

    public EditorAssist(ConfigEntry<bool> completionWhileTyping, LibraryList libraries, StartupCheck startupCheck)
    {
        _completionWhileTyping = completionWhileTyping;
        _startupCheck = startupCheck;
        _findPlacedBlock = FindInLevel;
        Libraries = libraries;
    }

    private EditorAssist(EditorAssist assist, Func<string, PlacedBlock> findPlacedBlock)
    {
        _completionWhileTyping = assist._completionWhileTyping;
        _startupCheck = assist._startupCheck;
        _findPlacedBlock = findPlacedBlock;
        Libraries = assist.Libraries;
    }

    /// <summary>
    /// The Libraries on this machine, whose Functions the editor colors, and whose outputs completion lists after a
    /// call's dot.
    /// </summary>
    public LibraryList Libraries { get; }

    /// <summary>
    /// Whether typing a name opens the completion list; Ctrl+Space opens it either way.
    /// </summary>
    public bool CompletionWhileTyping => _completionWhileTyping.Value;

    /// <summary>
    /// The block in the open level with a UID; null outside the editor, when there's none, when its style is turned off
    /// for this game version, or in a Library's editor, since the compiler rejects a reference to it.
    /// </summary>
    public PlacedBlock FindPlacedBlock(string uid)
    {
        return _findPlacedBlock(uid);
    }

    /// <summary>
    /// The same help for a Library's editor, which finds no placed blocks: the compiler rejects every placed(...) in a
    /// Library, since a UID belongs to one level.
    /// </summary>
    public EditorAssist ForLibrary()
    {
        return new EditorAssist(this, _ => null);
    }

    private PlacedBlock FindInLevel(string uid)
    {
        var central = ComponentCache.Get<LEV_LevelEditorCentral>();
        var block = central == null ? null : PlacedBlocks.FindWithoutWires(central, uid);
        return block == null || _startupCheck.DisabledBlockIds.Contains(block.BlockId) ? null : block;
    }
}
