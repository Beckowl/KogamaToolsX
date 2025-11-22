using KogamaToolsX.Data;

namespace KogamaToolsX.EditorStates;

internal class ESGroupObjects : ESStateCustomBase
{
    // static is fine, ESWaitForGroup will lock state
    private static ESWaitForGroup grouper = new();

    public ESGroupObjects() => stateType = EditorEventExt.ESGroupObjects;

    public override void Enter(EditorStateMachine e)
    {
        int numTargets = 0;

        foreach (var wo in e.SelectedWOs)
        {
            if (CanGroupObject(wo))
                numTargets++;
            else
                TextCommand.NotifyUser($"<color=yellow>Cannot group {wo.type.ToString()}.");
        }

        if (numTargets <= 0)
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
        }

        grouper.Execute(e);

        // transfer wos is the last state
        if (grouper.responseReceived && grouper.state == ESWaitForGroup.WaitForGroupsState.WaitingForTransferWos)
            e.PopState();
    }

    public override void Exit(EditorStateMachine e)
        => TextCommand.NotifyUser($"<color=cyan>Objects grouped successfully.");

    private static bool CanGroupObject(MVWorldObjectClient wo)
    {
        MV.WorldObject.WorldObjectType type = wo.type;

        // how do i check if a wo has a model???
        return type != MV.WorldObject.WorldObjectType.WorldObjectSpawnerVehicle &&
           type != MV.WorldObject.WorldObjectType.JetPack &&
           type != MV.WorldObject.WorldObjectType.Teleporter &&
           type != MV.WorldObject.WorldObjectType.CollectTheItemCollectable &&
           type != MV.WorldObject.WorldObjectType.CollectTheItemDropOff &&
           type != MV.WorldObject.WorldObjectType.CubeModel;
    }
}
