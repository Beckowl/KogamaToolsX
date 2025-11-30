using KogamaToolsX.Utils;
using MV.WorldObject;

namespace KogamaToolsX.EditorStates;
internal class ESAddObjectLinkFix : ESStateCustomBase
{
    private ObjectLink tempLink;
    private WorldObjectClientRef woRef;

    public override void Enter(EditorStateMachine e)
    {
        VoxelHit vhit = new();

        if (ObjectPicker.Pick(ref vhit) && vhit.woId != -1)
        {
            var wo = MVGameControllerBase.WOCM.GetWorldObjectClient(vhit.woId);

            if (wo == null)
            {
                e.PopState();
                return;
            }

            tempLink = new ObjectLink();      

            if (wo.selectedConnector != SelectedConnector.Object)
            {
                e.PopState();
                return;
            }

            tempLink.objectConnectorWOID = wo.id;

            MVGameControllerBase.MainCameraManager.LineDrawManager.SetTempObjectLink(tempLink);
            wo.HighlightConnector(true);
            woRef = MVGameControllerBase.WOCM.GetWorldObjectClientRef(wo.Id);
        }
    }

    public override void Execute(EditorStateMachine e)
    {
        if (woRef.WorldObjectClient == null)
        {
            LeaveAddLink(e);
            return;
        }

        if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
            return;


        VoxelHit hit = new VoxelHit();

        if (ObjectPicker.Pick(ref hit) && hit.woId != -1)
        {
            MVWorldObjectClient target = MVGameControllerBase.WOCM.GetWorldObjectClient(hit.woId);

            if (target != null && target.interactionFlags.HasFlag(InteractionFlags.HasCubeModel))
            {
                tempLink.objectWOID = hit.woId;
                MVGameControllerBase.OperationRequests.AddObjectLink(tempLink);
            }
        }

        e.DeSelectAll();
        LeaveAddLink(e);
    }

    private void LeaveAddLink(EditorStateMachine e)
    {
        if (e.ParentGroupID == MVGameControllerBase.WOCM.RootGroup.Id)
            e.Event = EditorEvent.ESTerrainEdit.ToIl2Cpp();
        else
            e.PopState();
    }

    public override void Exit(EditorStateMachine esm)
    {
        MVGameControllerBase.MainCameraManager.LineDrawManager.SetTempObjectLink(null);
    }
}
