using System;
using System.Collections.Generic;
using HarmonyLib;
using KogamaToolsX.Data;
using KogamaToolsX.EditorStates;
using KogamaToolsX.Utils;

namespace KogamaToolsX.Patches;

[HarmonyPatch]
internal static class CustomFSMStates
{
    // Add am entry to this dictonary to override an existing state or to register a custom state

    private static readonly Dictionary<EditorEventExt, ESStateCustomBase> overrideStates = new()
    {
        { EditorEventExt.ESAddLink, new ESAddLinkFix() },
        { EditorEventExt.ESAddObjectLink, new ESAddObjectLinkFix() },
        { EditorEventExt.ESTest, new ESTest() },
        { EditorEventExt.ESGroupObjects, new ESGroupObjects() }
    };

    private static bool HandleCustomState(FSMEntity e, Action<ESStateCustomBase, FSMEntity> invoke)
    {
        EditorEventExt evt = e.curEvent.ToEnum<EditorEventExt>();

        if (!overrideStates.TryGetValue(evt, out var state) || state == null)
            return true;

        invoke(state, e);
        return false;
    }

    [HarmonyPatch(typeof(ESStateBase), "Enter", [typeof(FSMEntity)])]
    [HarmonyPrefix]
    private static bool ESStateBase_Enter_Prefix(FSMEntity e) 
        => HandleCustomState(e, (s, ent) => s.Enter(ent));

    [HarmonyPatch(typeof(ESStateBase), "Execute", [typeof(FSMEntity)])]
    [HarmonyPrefix]
    private static bool ESStateBase_Execute_Prefix(FSMEntity e) 
        => HandleCustomState(e, (s, ent) => s.Execute(ent));

    [HarmonyPatch(typeof(ESStateBase), "Exit", [typeof(FSMEntity)])]
    [HarmonyPrefix]
    private static bool ESStateBase_Exit_Prefix(FSMEntity e) 
        => HandleCustomState(e, (s, ent) => s.Exit(ent));

    [HarmonyPatch(typeof(FSMEntity), nameof(FSMEntity.Event), MethodType.Setter)]
    [HarmonyPrefix]
    private static bool FSMEntity_Event_Setter_Prefix(FSMEntity __instance, Il2CppSystem.Object value)
    {
        FSMEntity e = __instance;
        EditorEventExt next = value.ToEnum<EditorEventExt>();

        if (!overrideStates.TryGetValue(next, out var state))
            return true;

        if (e.lockState)
            return false;

        e.nextEvent = value;

        if (value == null)
        {
            e.currentState.Exit(e);
            e.currentState = null;
            return false;
        }

        if (state != null)
        {
            if (e.currentState != null)
                e.currentState.Exit(e);

            e.stateName = next.ToString();
            e.nextEvent = null;
            e.prevEvent = e.curEvent;
            e.curEvent = value;

            state.Enter(e);
            e.data.Clear();
        }

        if (e.clearStack)
            e.stateStack.Clear();
        else
            e.clearStack = true;

        return false;
    }

    internal static void PushState(this FSMEntity e, EditorEventExt nextState)
        => e.PushState(nextState, EditorEventExt.UndefinedState);

    internal static void PushState(this FSMEntity e, EditorEventExt nextState, EditorEventExt overridePushState)
        => e.PushState((EditorEvent)nextState, (EditorEvent)overridePushState);
}
