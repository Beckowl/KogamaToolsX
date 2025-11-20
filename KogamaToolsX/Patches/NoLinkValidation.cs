using HarmonyLib;
using MV.WorldObject;

namespace KogamaToolsX.Patches;

[HarmonyPatch]
internal class NoLinkValidation
{
    // the og loop detection is poorly written,
    // and will freeze the game when the logic is large
    // we can just disable the checks entirely because the server does loop detection as well

    [HarmonyPatch(typeof(LogicObjectManager), "ValidateLink", [typeof(int), typeof(int), typeof(IWorldObjectManager), typeof(bool)], [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref])]
    [HarmonyPrefix]
    private static bool ValidateLink(ref bool loopDetected)
    {
        loopDetected = false;
        return false;
    }
}
