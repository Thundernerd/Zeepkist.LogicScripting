using HarmonyLib;
using Imui.IO.UGUI;
using UnityEngine;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Patches;

/// <summary>
/// Imui finds the mouse where <see cref="ImuiMouse"/> put it, while it holds one.
/// </summary>
[HarmonyPatch(typeof(ImuiUnityGUIBackend), nameof(ImuiUnityGUIBackend.GetMousePosition))]
internal static class ImuiUnityGUIBackend_GetMousePosition
{
    [HarmonyPrefix]
    private static bool Prefix(ref Vector2 __result)
    {
        if (ImuiMouse.Position is not { } position)
            return true;

        __result = position;
        return false;
    }
}
