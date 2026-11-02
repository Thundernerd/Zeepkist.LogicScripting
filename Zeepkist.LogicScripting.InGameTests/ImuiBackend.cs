using Imui.IO.UGUI;

namespace TNRD.Zeepkist.LogicScripting.InGameTests;

/// <summary>
/// Imui's backend, which the game's UI and ZeepSDK's windows take their input from.
/// </summary>
internal static class ImuiBackend
{
    private static ImuiUnityGUIBackend _backend;

    /// <summary>
    /// Null when there's none.
    /// </summary>
    public static ImuiUnityGUIBackend Find()
    {
        if (_backend == null)
            _backend = UnityEngine.Object.FindObjectOfType<ImuiUnityGUIBackend>();

        return _backend;
    }
}
