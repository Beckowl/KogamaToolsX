
using System;
using HarmonyLib;
using KogamaToolsX.Utils;

namespace KogamaToolsX.Patches;

[HarmonyPatch]
internal static class ContextMenuPatch
{
    [HarmonyPatch(typeof(ContextMenuController), "PopGizmos")]
    [HarmonyPrefix]
    private static void PopGizmos_Prefix(ContextMenuController __instance)
    {
        MVWorldObjectClient wo = __instance.selectedWorldObject;
        ContextMenu menu = __instance.currentContextMenu;

        if (wo == null || menu == null)
            return;

        foreach (var item in CustomContextMenu.GetButtons(wo))
        {
            Action callback = () =>
            {
                item.Callback(wo);
                menu.Pop();
            };

            menu.AddButton(item.Label, callback);
        }
    }
}
