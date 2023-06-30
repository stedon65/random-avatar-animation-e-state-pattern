using UnityEngine;


public interface IAvatar
{
    public Transform GetAvatarTransform();
    public Animator GetAvatarAnimator();
    public StateMachine GetAvatarStateMachine();
    public float GetShortDistance();
    public float GetMovementDistance();
    public void CalculateShortDistance();
    public void CalculateMovementDistance();
    public void CalculateForwardDirection();
    public void DrawRays();
    public void ChangeState(int state);   
}
