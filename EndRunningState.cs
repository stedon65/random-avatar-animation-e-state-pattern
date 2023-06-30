
public class EndRunningState : State
{
    public EndRunningState(IAvatar avatar) : base(avatar)
    {
    }

    public override void OnEnter()
    {
        avatar.GetAvatarAnimator().SetTrigger("StopRun");
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        avatar.CalculateMovementDistance();

        if (avatar.GetMovementDistance() < 2.8f)
        {
            avatar.ChangeState((int)AvatarState.ENDWALKING);
        }
    }
}
