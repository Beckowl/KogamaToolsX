using System;
using System.Linq;
using System.Reflection;

namespace KogamaToolsX.Utils;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
internal class InvokeOnInitAttribute : Attribute
{
    public int Priority { get; }

    public InvokeOnInitAttribute(int priority = int.MaxValue)
    {
        Priority = priority;
    }
}

internal static class InitCallbacks
{
    internal static void InvokeInitMethods()
    {
        var methods = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            .Select(m => (Method: m, Attr: m.GetCustomAttribute<InvokeOnInitAttribute>()))
            .Where(x => x.Attr != null)
            .OrderBy(x => x.Attr!.Priority)
            .ToList();

        foreach (var (method, _) in methods)
        {
            try
            {
                Plugin.Logger.LogInfo($"Invoking {method.Name}");

                method.Invoke(null, null);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to invoke {method.Name}: {ex.Message}");
            }
        }
    }
}
