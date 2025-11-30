using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using KogamaToolsX.Data;
using KogamaToolsX.Utils;

namespace KogamaToolsX.EditorStates;

internal class ESGroupObjects : ESStateCustomBase
{
    // static is fine, ESWaitForGroup will lock state
    private static ESWaitForGroup grouper = new();
    private bool success = false;

    public ESGroupObjects() 
        => stateType = EditorEventExt.ESGroupObjects;

    public override void Enter(EditorStateMachine e)
    {
        success = false;
        int numTargets = 0;
        var selected = new Il2CppStructArray<int>(IntPtr.Zero);

        e.SelectedIDs.CopyTo(selected);

        foreach (var id in selected)
        {
            var wo = MVGameControllerBase.WOCM.GetWorldObjectClient(id);

            if (CanGroupObject(wo))
                numTargets++;
            else
                e.DeSelectWorldObject(wo);
        }

        if (numTargets < 2)
        {
            TextCommand.NotifyUser($"<color=yellow>Cannot group less than two objects.");
            e.PopState();

            return;
        }

        TextCommand.NotifyUser($"Grouping ({numTargets}) objects...");

        grouper.Enter(e);
    }

    public override void Execute(EditorStateMachine e)
    {
        if (grouper.abort)
        {
            TextCommand.NotifyUser($"<color=yellow>Failed to group objects.");
            e.PopState();

            return;
        }

        grouper.Execute(e);

        // transfer wos is the last state
        if (grouper.responseReceived && grouper.state == ESWaitForGroup.WaitForGroupsState.WaitingForTransferWos)
        {
            success = true;
            e.PopState();
        }
    }

    public override void Exit(EditorStateMachine e)
    {
        if (success)
            TextCommand.NotifyUser($"<color=cyan>Objects grouped successfully.");
    }

    private static bool CanGroupObject(MVWorldObjectClient wo)
    {
        // TODO: add some kind of override to this
        bool hasModel = wo.GetModel() != null;

        if (hasModel)
            TextCommand.NotifyUser($"<color=yellow>Cannot group {wo.type.ToString()} because it contains a cube model.");

        return !hasModel;
    }
}
