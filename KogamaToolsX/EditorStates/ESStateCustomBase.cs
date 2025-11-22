using KogamaToolsX.Data;
using UnityEngine;

namespace KogamaToolsX.EditorStates;

// I tried inheriting custom states from ESStateBase but it wouldn't work.
// Registering custom states in il2cpp didn't help
internal class ESStateCustomBase
{
    private MVWorldObjectClientManager WOCM => MVGameControllerBase.WOCM;
    protected EditorEventExt stateType;

    protected WorldObjectClientRef tintedWo = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
    private EditorEventExt StateType => stateType;

    public void SetStateType(EditorEventExt stateTypeEvent) => stateType = stateTypeEvent;

    public virtual void Enter(EditorStateMachine esm) { }
    public virtual void Execute(EditorStateMachine e) { }
    public virtual void Exit(EditorStateMachine esm) { }

    public void Enter(FSMEntity e) => Enter((EditorStateMachine)e);
    public void Execute(FSMEntity e) => Execute((EditorStateMachine)e);
    public void Exit(FSMEntity e) => Exit((EditorStateMachine)e);

    protected void DeTintCurrent()
    {
        if (tintedWo.WorldObjectClient != null)
        {
            tintedWo.WorldObjectClient.DeSelect();
            tintedWo = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
        }
    }

    protected static bool SelectionIsAllowedByLogicEnabled(int woId)
    {
        MVWorldObjectClient wo = MVGameControllerBase.WOCM.GetWorldObjectClient(woId);

        if (wo == null || wo.GroupId == -1)
            return false;

        MVWorldObjectClient root = MVGameControllerBase.WOCM.GetWorldObjectClientRoot(woId);

        if (MVGameControllerBase.MainCameraManager.IsLogicRendered || root == null)
            return true;

        int layerNumber = LayerUtil.GetLayerNumber(LayerFlags.Default);

        if (root.GameObject.layer == layerNumber)
            return true;

        Transform[] componentsInChildren = root.GameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (componentsInChildren[i].gameObject.layer == layerNumber && componentsInChildren[i].gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }

    protected void TintObjectsOnMouseOver(EditorStateMachine e)
    {
        VoxelHit voxelHit = default;
        // todo: reimplement object picker

        //bool flag = EditModeObjectPicker.Pick(ref voxelHit, null, -262149);
        //TintObjectsOnMouseOver(e, flag, voxelHit);
    }

    protected void TintObjectsOnMouseOver(EditorStateMachine e, bool pickSuccess, VoxelHit hit)
    {
        if (tintedWo.WorldObjectClient != null && e.IsSelected(tintedWo.WorldObjectClient.Id))
            tintedWo = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();

        if (!pickSuccess)
        {
            MVWorldObjectClient wo = WOCM.GetWorldObjectClient(hit.woId);
            wo?.DeSelect();
            DeTintCurrent();
            return;
        }

        bool selectable = (hit.interactionFlags & InteractionFlags.Selectable) != 0;

        if (!selectable)
        {
            DeTintCurrent();
            return;
        }

        WorldObjectClientRef target = GetSelectableTarget(e, hit);

        if (target.WorldObjectClient == null)
            return;

        if (tintedWo.WorldObjectClient == null)
        {
            tintedWo = target;
            tintedWo.WorldObjectClient.Select(new Color(0f, 0.8f, 0f, 1f));
        }
        else if (tintedWo.WorldObjectClient.Id != target.WorldObjectClient.Id)
        {
            DeTintCurrent();
            tintedWo = target;
            tintedWo.WorldObjectClient.Select(new Color(0f, 0.8f, 0f, 1f));
        }
    }

    private WorldObjectClientRef GetSelectableTarget(EditorStateMachine e, VoxelHit hit)
    {
        if ((hit.interactionFlags & InteractionFlags.DirectlySelectable) != 0)
            return WOCM.GetWorldObjectClientRef(hit.woId);

        int parentBelow = MVGroup.GetParentBelow(e.ParentGroupID, hit.woId);

        if (parentBelow == -1 || e.IsSelected(parentBelow))
            return MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();

        return WOCM.GetWorldObjectClientRef(parentBelow);
    }
}
