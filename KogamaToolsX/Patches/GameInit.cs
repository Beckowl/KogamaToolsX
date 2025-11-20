using HarmonyLib;
using KogamaToolsX.Utils;

namespace KogamaToolsX.Patches;

[HarmonyPatch]
internal static class GameInit
{
    [HarmonyPatch(typeof(ModeControllerBase), "Initialize")]
    [HarmonyPostfix]
    private static void Initialize_Postfix(ModeControllerBase __instance)
    {
        if (__instance == MVGameControllerDesktop.Instance.modeController)
            InitCallbacks.InvokeInitMethods();
    }
}
