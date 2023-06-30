
public class WalkingState : State
{
    public WalkingState(IAvatar avatar) : base(avatar)
    {
    }

    public override void OnEnter()
    {
        avatar.GetAvatarAnimator().SetTrigger("StartWalk");
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        avatar.CalculateMovementDistance();

        if (avatar.GetShortDistance() > 11f)
        {
            avatar.CalculateForwardDirection();

            avatar.ChangeState((int)AvatarState.RUNNING);
        }
        else
        {
            if (avatar.GetMovementDistance() < 2.8f)
            {
                avatar.ChangeState((int)AvatarState.ENDWALKING);
            }
        }
    }
}
