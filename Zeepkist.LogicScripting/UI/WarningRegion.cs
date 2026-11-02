using System.Collections.Generic;
using Imui.Controls;
using Imui.Core;

namespace TNRD.Zeepkist.LogicScripting.UI;

/// <summary>
/// Warnings pinned above what they're about, in a region of their own. Where there isn't room for all of them the
/// region keeps a row, which the window's minimum height makes room for, and the rest scrolls.
/// </summary>
internal static class WarningRegion
{
    /// <summary>
    /// The height the warnings take in a region <paramref name="width"/> wide, out of <paramref name="available"/>,
    /// leaving <paramref name="minRestHeight"/> for what's under it.
    /// </summary>
    public static float GetHeight(ImGui gui, IReadOnlyList<string> warnings, float width, float available, float minRestHeight)
    {
        var spacing = gui.Style.Layout.Spacing;
        var settings = WrappedText.GetSettings(gui);
        var wanted = spacing * (warnings.Count + 1);
        foreach (var warning in warnings)
            wanted += WrappedText.GetHeight(gui, warning, in settings, width - (spacing * 2), minHeight: 0);

        return FooterSplit.GetWarningHeight(available, wanted, minRestHeight, WindowRegion.GetRowHeight(gui));
    }

    /// <summary>
    /// <paramref name="id"/> gives the region's scroll area an id of its own, apart from the regions drawn after it.
    /// </summary>
    public static void Draw(ImGui gui, ImRect rect, IReadOnlyList<string> warnings, string id)
    {
        var settings = WrappedText.GetSettings(gui);
        gui.PushId(id);
        WindowRegion.Begin(gui, rect);
        foreach (var warning in warnings)
            gui.Text(warning, in settings, WindowColors.Error);

        WindowRegion.End(gui);
        gui.PopId();
    }
}
