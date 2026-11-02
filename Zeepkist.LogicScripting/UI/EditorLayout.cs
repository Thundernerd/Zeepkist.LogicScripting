using Imui.Core;

namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// Where the center drew the Script editor and the error panel's header last frame.
/// </summary>
internal readonly struct EditorLayout
{
    public EditorLayout(ImRect center, ImRect field, ImRect errorsHeader, float minFieldHeight)
    {
        Center = center;
        Field = field;
        ErrorsHeader = errorsHeader;
        MinFieldHeight = minFieldHeight;
    }

    /// <summary>
    /// The whole center, between the Scripts and the Blocks sides.
    /// </summary>
    public ImRect Center { get; }

    public ImRect Field { get; }
    public ImRect ErrorsHeader { get; }

    /// <summary>
    /// The height of the editor's three rows.
    /// </summary>
    public float MinFieldHeight { get; }
}
