using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private IState currentState;

    private void Start()
    {
        SetState(new IdleState(this));
    }

    private void Update()
    {
        currentState.Tick();
    }

    public void SetState(IState state)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = state;
        Debug.Log(gameObject.name + "is in " + state.GetType().Name);

        if (currentState != null)
        {
            currentState.OnStateEnter();
        }
    }
}
