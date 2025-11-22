
using System;
using HarmonyLib;
using static KogamaToolsX.Utils.CustomContextMenu;

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

        foreach (var item in GetButtons(wo))
            menu.AddButton(item, wo);
    }

    private static void AddButton(this ContextMenu menu, ContextMenuItem item, MVWorldObjectClient target)
    {
        Action callback = () =>
        {
            item.Callback(target);
            menu.Pop();
        };

        var button = UnityEngine.Object.Instantiate(menu.contextMenuButtonPrefab);
        button.Initialize(item.Label, callback);
        button.transform.SetParent(menu.transform, false);
        button.transform.SetSiblingIndex(0);
    }
}
