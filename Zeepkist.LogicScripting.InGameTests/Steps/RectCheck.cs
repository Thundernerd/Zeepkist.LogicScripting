using System.Globalization;
using Imui.Core;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// For the steps that check where the window drew something.
/// </summary>
internal static class RectCheck
{
    // Rects are laid out in fractions of a pixel
    public const float Tolerance = 0.5f;

    public static bool Contains(ImRect outer, ImRect inner)
    {
        return inner.H >= 0
            && inner.Left >= outer.Left - Tolerance
            && inner.Right <= outer.Right + Tolerance
            && inner.Bottom >= outer.Bottom - Tolerance
            && inner.Top <= outer.Top + Tolerance;
    }

    public static string Describe(ImRect rect)
    {
        return $"x {Pixels(rect.X)}, y {Pixels(rect.Y)}, {Pixels(rect.W)} by {Pixels(rect.H)} px";
    }

    public static string Pixels(float pixels)
    {
        return pixels.ToString("0", CultureInfo.InvariantCulture);
    }
}
