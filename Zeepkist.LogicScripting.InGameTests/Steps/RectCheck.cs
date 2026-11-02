using System.Globalization;
using Imui.Core;
using UnityEngine;

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

    /// <summary>
    /// Whether the rects have the same position and size.
    /// </summary>
    public static bool Matches(ImRect a, ImRect b)
    {
        return Mathf.Abs(a.X - b.X) <= Tolerance
            && Mathf.Abs(a.Y - b.Y) <= Tolerance
            && Mathf.Abs(a.W - b.W) <= Tolerance
            && Mathf.Abs(a.H - b.H) <= Tolerance;
    }

    /// <summary>
    /// Whether the rect has this size.
    /// </summary>
    public static bool HasSize(ImRect rect, float width, float height)
    {
        return Mathf.Abs(rect.W - width) <= Tolerance && Mathf.Abs(rect.H - height) <= Tolerance;
    }

    public static string Describe(ImRect rect)
    {
        return $"x {Pixels(rect.X)}, y {Pixels(rect.Y)}, {Size(rect.W, rect.H)} px";
    }

    /// <summary>
    /// Such as <c>1600 by 900</c>.
    /// </summary>
    public static string Size(float width, float height)
    {
        return $"{Pixels(width)} by {Pixels(height)}";
    }

    public static string Pixels(float pixels)
    {
        return pixels.ToString("0", CultureInfo.InvariantCulture);
    }
}
