using HarmonyLib;

namespace KogamaToolsX.Patches;

[HarmonyPatch]
internal static class NoAntiAFK
{
    [HarmonyPatch(typeof(AwayMonitor), "Update")]
    [HarmonyPrefix]
    private static bool Update_Prefix() => false;
}
