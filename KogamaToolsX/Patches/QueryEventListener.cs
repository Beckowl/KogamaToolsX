using System;
using HarmonyLib;

namespace KogamaToolsX.Patches;

[HarmonyPatch]
internal static class QueryEventListener
{
    internal static event Action<MVWorldObjectClient, int> OnQueryEvent;

    [HarmonyPatch(typeof(WorldNetwork), "CreateQueryEvent")]
    [HarmonyPostfix]
    private static void CreateQueryEvent_Postfix(MVWorldObjectClient root, int instigatorActorNumber)
    {
        OnQueryEvent?.Invoke(root, instigatorActorNumber);
    }
}
