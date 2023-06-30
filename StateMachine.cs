using UnityEngine;


public class StateMachine
{
    private State actualState = null;

    public State CurrentState
    {
        get
        {
            return actualState;
        }
        private set
        {
            actualState = value;
        }
    }

    public void Init(State state)
    {
        CurrentState = state;
        state.OnEnter();
    }

    public void ChangeState(State state)
    {
        CurrentState.OnExit();
        CurrentState = state;
        state.OnEnter();
    }
}
