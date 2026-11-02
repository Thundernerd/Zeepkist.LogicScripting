using Imui.Core;

namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// Where the center drew the Script editor and the error panel's header, the last frame it drew them.
/// </summary>
internal readonly struct EditorLayout
{
    public EditorLayout(int frame, ImRect center, ImRect field, ImRect errorsHeader, float minFieldHeight, bool hasStorageWarning)
    {
        Frame = frame;
        Center = center;
        Field = field;
        ErrorsHeader = errorsHeader;
        MinFieldHeight = minFieldHeight;
        HasStorageWarning = hasStorageWarning;
    }

    /// <summary>
    /// Unity's frame count when it was drawn.
    /// </summary>
    public int Frame { get; }

    /// <summary>
    /// The whole center, between the Scripts and the Blocks sides.
    /// </summary>
    public ImRect Center { get; }

    public ImRect Field { get; }
    public ImRect ErrorsHeader { get; }

    /// <summary>
    /// The height the editor keeps however short the window is.
    /// </summary>
    public float MinFieldHeight { get; }

    /// <summary>
    /// Whether the center drew the storage warning above the editor.
    /// </summary>
    public bool HasStorageWarning { get; }
}
