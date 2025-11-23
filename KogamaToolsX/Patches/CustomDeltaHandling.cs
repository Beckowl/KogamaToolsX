using HarmonyLib;
using KogamaToolsX.Model;

namespace KogamaToolsX.Patches;

[HarmonyPatch]
internal class CustomDeltaHandling
{
    [HarmonyPatch(typeof(RuntimePrototypeCubeModel), "HandleDelta")]
    [HarmonyPrefix]
    private static bool HandleDelta_Prefix(RuntimePrototypeCubeModel __instance)
    {
        if (__instance.deltaCubes.Count > 0)
            CustomDeltaQueue.Enqueue(__instance);

        return false;
    }
}
