
public class EndWalkingState : State
{
    public EndWalkingState(IAvatar avatar) : base(avatar)
    {
    }

    public override void OnEnter()
    {
        avatar.GetAvatarAnimator().SetTrigger("StopWalk");
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        avatar.CalculateForwardDirection();

        avatar.ChangeState((int)AvatarState.TURNING);
    }
}
