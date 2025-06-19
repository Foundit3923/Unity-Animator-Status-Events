//Place this on the monster or player whose animator should be monitored

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AnimatorClipEvent;
using static AnimatorStateEvent;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Animator))]
public class AnimatorEventManager : MonoBehaviour
{
    public enum ASMStatus
    {
        Running,
        Finished,
        Transitioning,
        Idle,
        Error
    }

    [Flags]
    public enum StateFlags
    {
        None = 0,
        Incoming = 1 << 0,
        Current = 1 << 1,
        Previous = 1 << 2
    }

    Animator animator;

    [SerializeField]
    public List<AnimatorState> _animatorStates = new();

    [SerializeField]
    private AnimatorState _previousState;

    public AnimatorState PreviousState
    {
        get
        {
            if (_previousState != null && _previousState.IsInitialized)
            {
                return _previousState;
            }

            return null;
        }
        set => _previousState = value;
    }
    [SerializeField]
    private AnimatorState _currentState;

    public AnimatorState CurrentState
    {
        get
        {
            if (_currentState != null && _currentState.IsInitialized)
            {
                return _currentState;
            }

            return null;
        }
        set => _currentState = value;
    }
    [SerializeField]
    private AnimatorState _incomingState;

    public AnimatorState IncomingState
    {
        get
        {
            if (_incomingState != null && _incomingState.IsInitialized)
            {
                return _incomingState;
            }

            return null;
        }
        set
        {
            if (CurrentState == null)
            {
                if (_incomingState != null && _incomingState.IsInitialized)
                {
                    CurrentState = _incomingState;
                    _incomingState = value;
                }
                else
                {
                    CurrentState = value;
                }
            }
            else
            {
                _incomingState = value;
            }
        }
    }

    public ASMStatus Status;

    //Previous | Current | Incoming | State
    //0          0         0          Idle
    //0          1         0          Running
    //0          0         1          Error
    //0          1         1          Transitioning
    //1          0         0          Finished
    //1          1         0          Running
    //1          0         1          Error
    //1          1         1          Transitioning
    public Dictionary<StateFlags, ASMStatus> StatusDict = new()
    {
        { StateFlags.None, ASMStatus.Idle },
        { StateFlags.Current, ASMStatus.Running },
        { StateFlags.Incoming, ASMStatus.Error },
        { (StateFlags.Current | StateFlags.Incoming), ASMStatus.Transitioning },
        { StateFlags.Previous, ASMStatus.Finished },
        { StateFlags.Previous | StateFlags.Current, ASMStatus.Running },
        { StateFlags.Previous | StateFlags.Incoming, ASMStatus.Error },
        { StateFlags.Previous | StateFlags.Current | StateFlags.Incoming, ASMStatus.Transitioning }
    };

    public void CatchAnimatorClipEvents(GameObject owner, AnimatorStateInfo stateInfo, AnimatorClipInfo clipInfo, AnimatorClipEventType eventType)
    {
        if (owner == this.gameObject)
        {
            RegisterState(owner, stateInfo, clipInfo);
            switch (eventType)
            {
                case AnimatorClipEventType.OnStartClip:
                    _animatorStates.Where(x => x.ClipName == clipInfo.clip.name).FirstOrDefault().IsPlaying = true;
                    break;
                case AnimatorClipEventType.OnStopClip:
                    _animatorStates.Where(x => x.ClipName == clipInfo.clip.name).FirstOrDefault().IsPlaying = false;
                    break;
                default:
                    break;
            }

            StartCoroutine("EvalStatus");
        }
    }

    public void CatchAnimatorStateEvents(GameObject owner, AnimatorStateInfo stateInfo, AnimatorClipInfo clipInfo, AnimatorStateEventType eventType)
    {
        if (owner == this.gameObject)
        {
            RegisterState(owner, stateInfo, clipInfo);
            switch (eventType)
            {
                case AnimatorStateEventType.OnStartState:
                    StartState(stateInfo);
                    break;
                case AnimatorStateEventType.OnEndState:
                    EndState(stateInfo);
                    break;
                case AnimatorStateEventType.StartIntermediateState:
                    _animatorStates.Where(x => x.Hash == stateInfo.shortNameHash).FirstOrDefault().IsIntermediate = true;
                    StartState(stateInfo);
                    break;
                case AnimatorStateEventType.EndIntermediateState:
                    _animatorStates.Where(x => x.Hash == stateInfo.shortNameHash).FirstOrDefault().IsIntermediate = true;
                    EndState(stateInfo);
                    break;
                case AnimatorStateEventType.OnEnterStateMachine:
                    break;
                case AnimatorStateEventType.OnExitStaeMachine:
                    break;
                //case AnimatorStateEventType.OnUpdateState:
                //    This functionality is already covered by RegisterState
                //    StartState(stateInfo);
                //    break;
                default:
                    break;
            }

            StartCoroutine("EvalStatus");
        }
    }

    private void RegisterState(GameObject owner, AnimatorStateInfo stateInfo, AnimatorClipInfo clipInfo)
    {
        //clipInfo.clip == null indicates that the state's clip is not the current or next clip. Indicates no change.
        if (clipInfo.clip != null)
        {
            if (_animatorStates.Count > 0)
            {
                AnimatorState[] matchingState = _animatorStates.Where(x => x.Hash == stateInfo.shortNameHash).ToArray();
                if (IsExistingState(matchingState) && matchingState[0].ClipName != clipInfo.clip.name)
                {
                    //I think this will mostly be used for blend trees
                    //When there is a state that already exists AND it has a new clip
                    AnimatorState state = matchingState[0];
                    string oldClipName = state.ClipName;
                    string incomingClipName = clipInfo.clip.name;
                    state.ClipName = incomingClipName;
                    state.ClipDuration = clipInfo.clip.length;
                    state.WillLoop = clipInfo.clip.isLooping;
                }
                else
                {
                    AnimatorState newState = new(
                        stateInfo.shortNameHash,
                        clipInfo.clip.name,
                        clipInfo.clip.length,
                        clipInfo.clip.isLooping,
                        false,
                        AnimatorState.AnimatorStateStatus.Idle,
                        owner,
                        false)
                    {
                        IsInitialized = true
                    };
                    if (clipInfo.clip != null)
                    {
                        _animatorStates.Add(newState);
                    }
                }
            }
            else
            {
                AnimatorState newState = new(
                    stateInfo.shortNameHash,
                    clipInfo.clip.name,
                    clipInfo.clip.length,
                    clipInfo.clip.isLooping,
                    false,
                    AnimatorState.AnimatorStateStatus.Running,
                    owner,
                    false)
                {
                    IsInitialized = true
                };
                _animatorStates.Add(newState);
            }
        }
    }

    public bool IsExistingState(AnimatorState[] stateArray) => stateArray.Length > 0;

    private void StartState(AnimatorStateInfo stateInfo)
    {
        AnimatorState state;
        //Check if the state is allready in the queue.
        //If so, reintroduce the state to the queue and remove it from it's previous position
        if (CurrentState?.Hash == stateInfo.shortNameHash)
        {
            state = CurrentState;
            CurrentState = null;
        }
        else if (PreviousState?.Hash == stateInfo.shortNameHash)
        {
            state = PreviousState;
            PreviousState = null;
        }
        else
        {
            //If the state is not already in the queue, get it from _animatorStates
            state = _animatorStates.Where(x => x.Hash == stateInfo.shortNameHash).FirstOrDefault();
        }

        //Ensure the status is correct
        state.Status = AnimatorState.AnimatorStateStatus.Running;
        //introduce the state to the queue via IncomingState
        IncomingState = state;
    }
    private void EndState(AnimatorStateInfo stateInfo)
    {
        //theoretically this should always be CurrentState
        if (CurrentState != null && CurrentState.Hash == stateInfo.shortNameHash)
        {
            CurrentState.Status = AnimatorState.AnimatorStateStatus.Finished;
            PreviousState = CurrentState;
            //Call EvalStatus here and hav it send an event indicating a Finished state?
            //Empty the CurrentState so that Incoming state works properly
            CurrentState = null;
            //introduce a null into IncomingState to propogate any IncomingStates into CurrentState
            IncomingState = null;
        }
        else if (IncomingState != null && IncomingState.Hash == stateInfo.shortNameHash)
        {
            //If the IncomingState ends before the transition is finished set it as the previous state
            IncomingState.Status = AnimatorState.AnimatorStateStatus.Finished;
            PreviousState = IncomingState;
            IncomingState = null;
        }
        else
        {
            _animatorStates.Where(x => x.Hash == stateInfo.shortNameHash).FirstOrDefault().Status = AnimatorState.AnimatorStateStatus.Finished;
        }
    }

    private IEnumerator EvalStatus()
    {
        //Previous | Current | Incoming | State
        //0          0         0          Idle
        //0          1         0          Running
        //0          0         1          Error
        //0          1         1          Transitioning
        //1          0         0          Finished
        //1          1         0          Running
        //1          0         1          Error
        //1          1         1          Transitioning
        StateFlags stateFlags = StateFlags.None;
        if (PreviousState != null) { stateFlags |= StateFlags.Previous; }

        if (CurrentState != null) { stateFlags |= StateFlags.Current; }

        if (IncomingState != null) { stateFlags |= StateFlags.Incoming; }

        Status = StatusDict[stateFlags];

        yield return null;
    }

    public ASMStatus GetStatus()
    {
        //Previous | Current | Incoming | State
        //0          0         0          Idle
        //0          1         0          Running
        //0          0         1          Error
        //0          1         1          Transitioning
        //1          0         0          Finished
        //1          1         0          Running
        //1          0         1          Error
        //1          1         1          Transitioning
        StateFlags stateFlags = StateFlags.None;
        if (PreviousState != null) { stateFlags |= StateFlags.Previous; }

        if (CurrentState != null) { stateFlags |= StateFlags.Current; }

        if (IncomingState != null) { stateFlags |= StateFlags.Incoming; }

        return StatusDict[stateFlags];
    }
}
