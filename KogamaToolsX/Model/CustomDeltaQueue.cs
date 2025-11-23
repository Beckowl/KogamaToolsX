using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine;

namespace KogamaToolsX.Model;
internal class CustomDeltaQueue : MonoBehaviour
{
    private static CustomDeltaQueue instance;
    private static Dictionary<int, RuntimePrototypeCubeModel> pendingModels = new();
    private static bool isRunning = false;

    private const int MAX_CUBES = 500;
    private const float WAIT_TIME = 7.0f;

    private void Awake()
    {
        instance = this;
    }

    internal static void Enqueue(RuntimePrototypeCubeModel model)
    {
        if (!pendingModels.ContainsKey(model.prototypeId))
            pendingModels.Add(model.prototypeId, model);

        if (!isRunning)
            instance.StartCoroutine(ProcessQueue().WrapToIl2Cpp());
    }

    private static void HandleSingleDelta(RuntimePrototypeCubeModel model)
    {
        if (model == null || model.deltaCubes.Count == 0)
        {
            pendingModels.Remove(model.prototypeId);
            return;
        }

        var data = model.deltaCubes.Dequeue(model);

        if (data == null)
            return;

        switch (model.prototypeState)
        {
            case PrototypeState.Registered:
                MVGameControllerBase.OperationRequests.UpdatePrototype(model.prototypeId, data);
                break;
            case PrototypeState.Pending:
                model.pendingDeltaCubes.AddRange(new Il2CppSystem.Collections.Generic.IEnumerable<byte>(data.Pointer));
                break;
        }
    }

    private static IEnumerator ProcessQueue()
    {
        isRunning = true;
        int count = 0;

        while (pendingModels.Count > 0)
        {
            var keys = pendingModels.Keys.ToList();

            foreach (var key in keys)
            {
                pendingModels.TryGetValue(key, out var model);

                if (model == null)
                    continue;

                HandleSingleDelta(model);
                count++;

                if (count >= MAX_CUBES)
                {
                    count = 0;
                    yield return new WaitForSeconds(WAIT_TIME);
                }

                yield return null;
            }
        }

        isRunning = false;
    }
}
