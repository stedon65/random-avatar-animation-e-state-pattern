
public class RunningState : State
{
    private bool startRun = true;

    public RunningState(IAvatar avatar) : base(avatar)
    {
    }

    public override void OnEnter()
    {
        avatar.GetAvatarAnimator().SetTrigger("StartRun");
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (startRun)
        {
            avatar.CalculateForwardDirection();

            startRun = false;
        }

        avatar.CalculateMovementDistance();

        if (avatar.GetMovementDistance() < 6.0f)
        {
            avatar.ChangeState((int)AvatarState.ENDRUNNING);
        }
    }
}
