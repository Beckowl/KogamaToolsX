namespace KogamaToolsX.Data;

public enum EditorEventExt
{
    // vanilla states

    EditCubes,
    SelectCubes = 3,
    Rotating = 5,
    ObjectSelected = 12,
    UndefinedState = 18,
    ESTerrainEdit = 47,
    ESWaitForSelect = 19,
    ESSettingsMenu,
    ESWaitForGroup = 27,
    ESWaitForUngroup,
    ESTranslate = 34,
    ESWaitForClone,
    ESAddLink = 39,
    ESWalkMode = 41,
    ESInsert,
    ESAddObjectLink,
    ESBlueprintCreator,
    CERoam,
    CEEditBody,
    ESBodyCreator = 48,
    ESAddToMarketPlaceState,
    CEAvatarAccessory,
    PMAvatarAccessory,
    CERoamUUI,
    CEEditBodyUUI,
    CEAvatarAccessoryUUI,
    ESEnterCubeTutorial,
    ESEditCubeTutorial,
    ESLeaveCubeTutorial,
    ESWaitForPlayModeAvatar,
    ESWaitForBuildModeAvatar,

    ExtStateBegin = 10000,

    // custom states

    ESTest,
    ESGroupObjects,
}