using UnityEngine;


public class TurningState : State
{
    public float timer = 0.0f;
    public int turnNumber = 0;

    public TurningState(IAvatar avatar) : base(avatar)
    {
    }

    public override void OnEnter()
    {
        turnNumber = Random.Range(0, 2);
        timer = 0.0f;

        avatar.GetAvatarAnimator().SetTrigger("LeftTurn90");
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        timer += Time.deltaTime;

        if ((turnNumber > 0) && (timer >= 1.8f))
        {
            turnNumber--;
            timer = 0.0f;
            avatar.GetAvatarAnimator().SetTrigger("LeftTurn90");
        }

        if ((turnNumber == 0) && (timer >= 1.8f))
        {
            avatar.CalculateShortDistance();

            if (avatar.GetShortDistance() > 5.0f)
            {
                avatar.CalculateForwardDirection();

                avatar.ChangeState((int)AvatarState.WALKING);
            }
            else
            {
                turnNumber = 1;
                timer = 10.0f;
            }
        }
    }
}
