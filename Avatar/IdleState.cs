
public class IdleState : State
{
    public IdleState(IAvatar avatar) : base(avatar)
    {
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        avatar.CalculateShortDistance();
        avatar.CalculateForwardDirection();

        avatar.ChangeState((int)AvatarState.WALKING);
    }
}
