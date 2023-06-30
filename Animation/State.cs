
public abstract class State
{
    protected IAvatar avatar;
    protected enum AvatarState { IDLE, WALKING, RUNNING, ENDRUNNING, ENDWALKING, TURNING }

    protected State(IAvatar avatar)
    {
        this.avatar = avatar;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void OnInput()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnFixedUpdate()
    {
        avatar.DrawRays();
    }
}
