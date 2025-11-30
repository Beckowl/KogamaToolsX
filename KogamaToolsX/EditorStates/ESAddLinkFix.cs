using KogamaToolsX.Utils;
using MV.WorldObject;
using UnityEngine;

namespace KogamaToolsX.EditorStates;

internal class ESAddLinkFix : ESStateCustomBase
{
    private Link tempLink;
    private WorldObjectClientRef woRef;
    private SelectedConnector startSelectorType;

    public override void Enter(EditorStateMachine e)
    {
        VoxelHit vhit = new();
        startSelectorType = SelectedConnector.None;

        if (ObjectPicker.Pick(ref vhit) && vhit.woId != -1)
        {
            var wo = MVGameControllerBase.WOCM.GetWorldObjectClient(vhit.woId);

            if (wo == null)
            {
                e.PopState();
                return;
            }

            tempLink = new Link();

            startSelectorType = GetSelectedConnector(wo);

            if (startSelectorType == SelectedConnector.None)
            {
                e.PopState();
                return;
            }

            AssignLink(wo, startSelectorType);

            MVGameControllerBase.MainCameraManager.LineDrawManager.SetTempLink(tempLink);
            wo.HighlightConnector(true);
            woRef = MVGameControllerBase.WOCM.GetWorldObjectClientRef(wo.Id);
        }
    }

    public override void Execute(EditorStateMachine e)
    {
        if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
            return;

        if (woRef.WorldObjectClient == null)
        {
            LeaveAddLink(e);
            return;
        }

        VoxelHit vhit = new();

        if (ObjectPicker.Pick(ref vhit) && vhit.woId != -1)
        {
            var wo = MVGameControllerBase.WOCM.GetWorldObjectClient(vhit.woId);

            if (wo != null)
            {
                var selector = GetSelectedConnector(wo);

                if (selector != SelectedConnector.None && selector != startSelectorType)
                {
                    AssignLink(wo, selector);
                    MVGameControllerBase.OperationRequests.AddLink(tempLink);
                }
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

    public override void Exit(EditorStateMachine e)
    {
        if (woRef.WorldObjectClient != null)
            woRef.WorldObjectClient.HighlightConnector(false);

        MVGameControllerBase.MainCameraManager.LineDrawManager.SetTempLink(null);
    }

    private SelectedConnector GetSelectedConnector(MVWorldObjectClient wo)
    {
        var connector = SelectedConnector.None;
        Vector3 mousePos = MVInputWrapper.GetPointerPosition();

        if (wo.HasInputConnector && wo.IsPointOverInputConnector(mousePos))
            connector = SelectedConnector.Input;

        if (wo.HasOutputConnector && wo.IsPointOverOutputConnector(mousePos))
            connector = SelectedConnector.Output;

        return connector;
    }

    private void AssignLink(MVWorldObjectClient wo, SelectedConnector connector)
    {
        switch (connector)
        {
            case SelectedConnector.Input:
                tempLink.inputWOID = wo.id;
                break;
            case SelectedConnector.Output:
                tempLink.outputWOID = wo.id;
                break;
        }
    }
}
