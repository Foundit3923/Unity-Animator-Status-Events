using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AnimatorStateEvent
{
    public enum AnimatorStateEventType
    {
        OnStartState,
        OnEndState,
        StartIntermediateState,
        EndIntermediateState,
        TransformPlayer,
        OnEnterStateMachine,
        OnExitStaeMachine,
        OnUpdateState
    }

    public AnimatorStateEventType StateName;
    public UnityEvent<GameObject, AnimatorStateInfo, AnimatorClipInfo, AnimatorStateEventType> OnAnimatorStateEvent;
}
