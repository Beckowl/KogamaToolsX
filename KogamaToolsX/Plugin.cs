using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using KogamaToolsX.Utils;

namespace KogamaToolsX;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static ManualLogSource Logger;
    private readonly Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        Logger = base.Log;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        harmony.PatchAll();
    }

    [InvokeOnInit]
    private static void DoGreeting()
    {
        const string msg =
            "<color=cyan>Welcome to {0} v{1}!</color>\n\n" +
            "The quick brown fox jumped over the lazy dog.";

        TextCommand.NotifyUser(string.Format(msg, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION));
    }
}
