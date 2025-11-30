using Assets.Scripts.WorldObjectTypes.EditablePickupItem;
using MV.WorldObject;
using WorldObjectTypes.MVDoor;
using WorldObjectTypes.VehicleEnergy;

namespace KogamaToolsX.Utils;
internal static class ModelUtils
{
    // shoutout to MauryDev for the original method
    internal static MVCubeModelBase GetModel(this MVWorldObjectClient wo)
    {
        var type = wo.GetIl2CppType();

        // switching name is not the best practice but the original read like a wall
        switch (type.Name)
        {
            case "MVWorldObjectSpawnerVehicle":
                {
                    var spawner = wo.Cast<MVWorldObjectSpawnerVehicle>();
                    var spawned = MVGameControllerBase.WOCM.GetWorldObjectClient(spawner.SpawnWorldObjectID);
                    return GetModel(spawned);
                }
            case "MVSentryGunBlueprint":
                return wo.Cast<MVSentryGunBlueprint>().EditableCubesWO;

            case "MVMovingPlatformGroup":
                return wo.Cast<MVMovingPlatformGroup>().Platform.CubeModel;

            case "MVRotator":
                return wo.Cast<MVRotator>().CubeModel;

            case "CollectTheItemCollectable":
                return wo.Cast<CollectTheItemCollectable>().editableCubeModelWrapper.CubeModel;

            case "CollectTheItemDropOff":
                return wo.Cast<CollectTheItemDropOff>().editableCubeModelWrapper.CubeModel;

            case "MVAdvancedGhost":
                return wo.Cast<MVAdvancedGhost>().editableCubeModelWrapper.CubeModel;

            case "MVJetPack":
                return wo.Cast<MVJetPack>().editableCubeModelWrapper.CubeModel;

            case "MVHoverCraft":
                return wo.Cast<MVHoverCraft>().editableCubeModelWrapper.CubeModel;

            case "MVWorldObjectSpawnerVehicleEnergy":
                return wo.Cast<MVWorldObjectSpawnerVehicleEnergy>().vehicleEnergyChild.CubeModelInstance;

            case "MVDoorBlueprint":
                return wo.Cast<MVDoorBlueprint>().DoorLogic.doorModelInstance;

            case "MVCubeModelInstance":
                return wo.Cast<MVCubeModelBase>();

            default:
                if (typeof(MVEditablePickupItemBaseBlueprint).GetIl2Type().IsAssignableFrom(type))
                    return wo.Cast<MVEditablePickupItemBaseBlueprint>().editableCubeModel;

                return null!;
        }
    }

    internal static bool IsOwned(this RuntimePrototypeCubeModel model)
        => model.AuthorProfileID == -1 || model.AuthorProfileID == MVGameControllerBase.LocalPlayer.ProfileID;

    internal static bool IsOwned(this MVCubeModelBase model) => model.prototypeCubeModel.IsOwned();

    internal static byte[] GetCompressedCubeData(RuntimePrototypeCubeModel prototype)
    {
        var bp = new BytePacker();

        foreach (CubeModelChunk chunk in prototype.chunks.Values)
        {
            var enumerator = chunk.cells.GetEnumerator();

            // can't use foreach here because of il2cpp weirdness
            while (enumerator.MoveNext())
            {
                IntVector pos = enumerator.Current.Key;

                // can't get cube from enumerator as well
                Cube cube = prototype.GetCube(pos);

                CubeDataPacker.WriteCompressedCube(bp, pos.x, pos.y, pos.z, cube.byteCorners, cube.faceMaterials);
            }
        }

        return bp.ToArray();
    }
}
