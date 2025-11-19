namespace KogamaToolsX.EditorStates;
internal class ESMultiSelect : ESStateBase
{
    public override void Enter(EditorStateMachine esm)
    {
        Plugin.Logger.LogInfo("ESTest enter");
    }

    public override void Execute(EditorStateMachine e)
    {
        Plugin.Logger.LogInfo("ESTest enter");
        e.PopState();
    }

    public override void Exit(EditorStateMachine esm)
    {
        Plugin.Logger.LogInfo("ESTest exit");
    }
}
