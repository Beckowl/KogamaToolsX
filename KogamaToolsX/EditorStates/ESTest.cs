using KogamaToolsX.Data;

namespace KogamaToolsX.EditorStates;
internal class ESTest : ESStateCustomBase
{
    public ESTest()
    {
        this.stateType = EditorEventExt.ESTest;
    }

    public override void Enter(EditorStateMachine esm)
    {
        Plugin.Logger.LogInfo("ESTest enter");
    }

    public override void Execute(EditorStateMachine e)
    {
        Plugin.Logger.LogInfo("ESTest execute");
        e.PopState();
    }

    public override void Exit(EditorStateMachine esm)
    {
        Plugin.Logger.LogInfo("ESTest exit");
    }
}
