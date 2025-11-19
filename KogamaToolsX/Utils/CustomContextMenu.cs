using System;
using System.Collections.Generic;

namespace KogamaToolsX.Utils;
internal static class CustomContextMenu
{
    internal record ContextMenuItem(
        string Label,
        Func<MVWorldObjectClient, bool> ShouldShow,
        Action<MVWorldObjectClient> Callback
    );

    private static readonly List<ContextMenuItem> Items = new();

    internal static void AddButton(string label, Func<MVWorldObjectClient, bool> condition, Action<MVWorldObjectClient> callback) => Items.Add(new ContextMenuItem(label, condition, callback));

    internal static void RemoveButton(string label) => Items.RemoveAll(item => item.Label == label);

    internal static IEnumerable<ContextMenuItem> GetButtons(MVWorldObjectClient obj)
    {
        foreach (var item in Items)
        {
            if (item.ShouldShow(obj))
                yield return item;
        }
    }
}
