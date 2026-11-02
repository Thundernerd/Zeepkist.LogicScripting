using System.Linq;
using BepInEx.Configuration;
using TNRD.Zeepkist.LogicScripting.Blocks;
using TNRD.Zeepkist.LogicScripting.Compiler;
using TNRD.Zeepkist.LogicScripting.Compiler.Storage;
using ZeepSDK.Utilities;

namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// What the Script editor's help reads from outside the editor, shared by every Script's editor.
/// </summary>
internal sealed class EditorAssist
{
    private readonly ConfigEntry<bool> _completionWhileTyping;
    private readonly StartupCheck _startupCheck;

    public EditorAssist(ConfigEntry<bool> completionWhileTyping, LibraryList libraries, StartupCheck startupCheck)
    {
        _completionWhileTyping = completionWhileTyping;
        _startupCheck = startupCheck;
        Libraries = libraries;
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
    /// The block in the open level with a UID; null outside the editor, when there's none, or when its style is turned
    /// off for this game version, since the compiler rejects a reference to it.
    /// </summary>
    public PlacedBlock FindPlacedBlock(string uid)
    {
        var central = ComponentCache.Get<LEV_LevelEditorCentral>();
        var block = central == null ? null : PlacedBlocks.FindWithoutWires(central, uid);
        return block == null || _startupCheck.DisabledBlockIds.Contains(block.BlockId) ? null : block;
    }
}
