using UnityEngine;


public class BryceAvatar : IAvatar
{
    private Transform bryceTransform = null;
    private Animator animator = null;

    private StateMachine avatarStateMachine = null;

    public enum AvatarState { IDLE, WALKING, RUNNING, ENDRUNNING, ENDWALKING, TURNING }

    private IdleState idleState;
    private WalkingState walkingState;
    private RunningState runningState;
    private EndRunningState endRunningState;
    private EndWalkingState endWalkingState;
    private TurningState turningState;

    private float maxDistance = 100.0f;
    private float shortDist = 1.0f * Mathf.Pow(10, 6);
    private Vector3 shortDistHP = Vector3.zero;
    private bool firstTime = true;
    private float movementDistance = 0.0f;


    public BryceAvatar()
    {
        bryceTransform = GameObject.Find("Bryce").transform;
        animator = bryceTransform.GetComponent<Animator>();

        avatarStateMachine = new StateMachine();

        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        runningState = new RunningState(this);
        endRunningState = new EndRunningState(this);
        endWalkingState = new EndWalkingState(this);
        turningState = new TurningState(this);

        avatarStateMachine.Init(idleState);
    }

    public Transform GetAvatarTransform()
    {
        return bryceTransform;
    }

    public Animator GetAvatarAnimator()
    {
        return animator;
    }

    public StateMachine GetAvatarStateMachine()
    {
        return avatarStateMachine;
    }

    public float GetShortDistance()
    {
        return shortDist;
    }

    public float GetMovementDistance()
    {
        return movementDistance;
    }

    public void CalculateShortDistance()
    {
        shortDist = 0.0f;
        shortDistHP = Vector3.zero;
        firstTime = true;

        ComputeShortDistance();
    }

    public void CalculateMovementDistance()
    {
        ComputeMovementDistance();
    }

    public void CalculateForwardDirection()
    {
        Vector3 hitPoint = shortDistHP;
        hitPoint.y = 0.0f;
        Vector3 direction = hitPoint - bryceTransform.position;

        bryceTransform.forward = direction;

        Quaternion rotation = Quaternion.AngleAxis(1.0f, bryceTransform.up);
        Vector3 forward = rotation * bryceTransform.forward;

        bryceTransform.forward = forward;
    }

    public void DrawRays()
    {
        DrawDistanceRays();
    }

    public void ChangeState(int state)
    {
        switch (state)
        {
            case (int)AvatarState.IDLE:
                avatarStateMachine.ChangeState(idleState);

                return;

            case (int)AvatarState.WALKING:
                avatarStateMachine.ChangeState(walkingState);

                return;

            case (int)AvatarState.RUNNING:
                avatarStateMachine.ChangeState(runningState);

                return;

            case (int)AvatarState.ENDRUNNING:
                avatarStateMachine.ChangeState(endRunningState);

                return;
            case (int)AvatarState.ENDWALKING:
                avatarStateMachine.ChangeState(endWalkingState);

                return;

            case (int)AvatarState.TURNING:
                avatarStateMachine.ChangeState(turningState);

                return;

            default:
                return;
        }
    }

    private void ComputeShortDistance()
    {
        float increment = 0.0f;
        Vector3 rayOrigin = bryceTransform.position;
        rayOrigin.y = 0.2f;

        for (int i = 0; i < 40; i++, increment += 0.5f)
        {
            Quaternion rotation = Quaternion.AngleAxis(-10.0f + increment, bryceTransform.up);
            Vector3 forward = rotation * bryceTransform.forward;
            Ray ray = new Ray(rayOrigin, forward.normalized);

            RaycastHit hitPoint;

            if (Physics.Raycast(ray, out hitPoint, maxDistance))
            {
                Vector3 rayDirection = hitPoint.point - rayOrigin;

                if (firstTime)
                {
                    shortDist = rayDirection.magnitude;
                    shortDistHP = hitPoint.point;
                    shortDistHP.y = 0.4f;
                    firstTime = false;
                }
                else
                {
                    if (rayDirection.magnitude < shortDist)
                    {
                        shortDist = rayDirection.magnitude;
                        shortDistHP = hitPoint.point;
                        shortDistHP.y = 0.4f;
                    }
                }
            }
        }
    }

    public void ComputeMovementDistance()
    {
        Vector3 lineOrigin = bryceTransform.position;
        lineOrigin.y = 0.4f;

        if (shortDistHP != Vector3.zero)
        {
            Debug.DrawLine(lineOrigin, shortDistHP, Color.red);

            movementDistance = Vector3.Distance(lineOrigin, shortDistHP);
        }
    }

    private void DrawDistanceRays()
    {
        float increment = 0.0f;
        Vector3 rayOrigin = bryceTransform.position;
        rayOrigin.y = 0.2f;

        for (int i = 0; i < 40; i++, increment += 0.5f)
        {
            Quaternion rotation = Quaternion.AngleAxis(-10.0f + increment, bryceTransform.up);
            Vector3 forward = rotation * bryceTransform.forward;
            Ray ray = new Ray(rayOrigin, forward.normalized);

            RaycastHit hitPoint1;

            if (Physics.Raycast(ray, out hitPoint1, maxDistance))
            {
                Debug.DrawLine(ray.origin, hitPoint1.point, Color.green);
            }
        }
    }
}
