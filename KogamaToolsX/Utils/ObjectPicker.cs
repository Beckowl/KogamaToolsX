using UnityEngine;

namespace KogamaToolsX.Utils;

internal static class ObjectPicker
{
    internal static LinkObjectBase PickLink(ref VoxelHit hit)
    {
        if (!MVGameControllerBase.MainCameraManager.IsLogicRendered)
            return null;

        float dist = float.PositiveInfinity;

        if (Pick(ref hit))
            dist = hit.distance;

        Ray ray = MVGameControllerBase.MainCameraManager.MainCamera.ScreenPointToRay(MVInputWrapper.GetPointerPosition());
        int layerMask = LayerMask.GetMask("Logic", "LogicSelected");

        RaycastHit raycastHit;

        Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, layerMask);

        if (raycastHit.collider != null && raycastHit.distance < dist)
        {
            hit.point = raycastHit.point;
            return raycastHit.collider.gameObject.GetComponentInChildren<LinkObjectBase>();
        }

        return null;
    }

    internal static bool Pick(ref VoxelHit vhit, Il2CppSystem.Collections.Generic.HashSet<int> ignoreWoIds = null, int layerMask = -262149)
    {
        Ray ray = Camera.main.ScreenPointToRay(MVInputWrapper.GetPointerPosition());
        var hits = CollisionDetection.MVHitAll(ray, float.PositiveInfinity, ignoreWoIds, layerMask);

        if (hits.Count == 0)
            return false;

        return TryGetClosestHit(ray, hits, ref vhit);
    }

    private static bool DrawPlaneHit(ref float drawPlaneDist, Ray ray)
    {
        if (!DrawPlane.IsDrawPlaneActive)
            return false;

        var planeHit = Vector3.zero;
        bool pick = DrawPlane.Pick(ref planeHit);

        if (pick)
            drawPlaneDist = (planeHit - ray.origin).magnitude;

        return pick;
    }

    private static bool TryGetClosestHit(Ray ray, Il2CppSystem.Collections.Generic.List<VoxelHit> hits, ref VoxelHit vhit)
    {
        float drawPlaneDist = float.PositiveInfinity;
        bool drawPlaneHit = DrawPlaneHit(ref drawPlaneDist, ray);

        float closestDist = float.PositiveInfinity;
        bool hitDetected = false;

        foreach (VoxelHit voxelHit in hits)
        {
            if ((voxelHit.distance < drawPlaneDist || !drawPlaneHit) && voxelHit.transform.gameObject.activeInHierarchy)
            {
                float hitDist = Vector3.Distance(ray.origin, voxelHit.point);
                if (hitDist < closestDist)
                {
                    closestDist = hitDist;
                    vhit = voxelHit;
                    hitDetected = true;
                }
            }
        }

        return hitDetected;
    }
}