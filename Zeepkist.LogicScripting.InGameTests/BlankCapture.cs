using System.Linq;

namespace TNRD.Zeepkist.LogicScripting.InGameTests;

/// <summary>
/// A screenshot where every pixel is the same color, such as the transparent one a game started without a monitor
/// saves, shows nothing a run could check. It reads the capture's raw texture data, a pixel after the other, and gives
/// the first pixel's bytes in their order, such as <c>#00000000</c>.
/// </summary>
internal static class BlankCapture
{
    public static bool IsBlank(byte[] pixels, int bytesPerPixel, out string color)
    {
        color = "#" + string.Concat(pixels.Take(bytesPerPixel).Select(b => b.ToString("X2")));
        for (var i = bytesPerPixel; i < pixels.Length; i++)
        {
            if (pixels[i] != pixels[i % bytesPerPixel])
                return false;
        }

        return true;
    }
}
