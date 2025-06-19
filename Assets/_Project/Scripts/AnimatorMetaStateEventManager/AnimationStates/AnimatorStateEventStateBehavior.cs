using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityServiceLocator;
using static AnimatorStateEvent;

public class AnimatorStateEventStateBehavior : StateMachineBehaviour
{
    protected AnimatorStateInfo stateInfo;
    public AnimatorStateInfo StateInfo => stateInfo;

    Blackboard _blackboard;
    Blackboard blackboard
    {
        get
        {
            if (_blackboard != null)
            {
                return _blackboard;
            }

            return _blackboard = blackboardController.GetBlackboard();
        }
        set => _blackboard = value;
    }
    BlackboardController _blackboardController;
    BlackboardController blackboardController
    {
        get
        {
            if (_blackboardController != null)
            {
                return _blackboardController;
            }

            return _blackboardController = ServiceLocator.Global.Get<BlackboardController>();
        }
        set => _blackboardController = value;
    }

    private AnimatorStateProperties _properties;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (_properties == null)
        {
            this.stateInfo = stateInfo;
            AnimatorStateProperties[] propertiesArray = animator.GetBehaviours<AnimatorStateProperties>();
            _properties = propertiesArray.First(behavior => behavior.StateNameHash == stateInfo.shortNameHash);
        }

        if (stateInfo.shortNameHash == _properties.StateNameHash)
        {
            if (_properties.StateEventSubscriptionDict[AnimatorStateEventType.OnStartState] ||
                _properties.StateEventSubscriptionDict[AnimatorStateEventType.StartIntermediateState])
            {
                NotifyReceiver(animator, StateInfo, _properties.IsIntermediate == true ? AnimatorStateEventType.StartIntermediateState : AnimatorStateEventType.OnStartState);
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.shortNameHash == _properties.StateNameHash)
        {
            if (_properties.StateEventSubscriptionDict[AnimatorStateEventType.OnUpdateState])
            {
                NotifyReceiver(animator, StateInfo, AnimatorStateEventType.OnUpdateState);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.shortNameHash == _properties.StateNameHash)
        {
            if (_properties.StateEventSubscriptionDict[AnimatorStateEventType.OnEndState] ||
                _properties.StateEventSubscriptionDict[AnimatorStateEventType.EndIntermediateState])
            {
                NotifyReceiver(animator, StateInfo, _properties.IsIntermediate == true ? AnimatorStateEventType.EndIntermediateState : AnimatorStateEventType.OnEndState);
            }
        }
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash) => base.OnStateMachineEnter(animator, stateMachinePathHash);

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash) => base.OnStateMachineExit(animator, stateMachinePathHash);

    void NotifyReceiver(Animator animator, AnimatorStateInfo eventOwnerInfo, AnimatorStateEventType eventType)
    {
        if (blackboard.TryGetValue(BlackboardController.BlackboardKeyStrings.AnimationStateEventReceivers, out List<AnimatorStateEventReceiver> receivers))
        {
            if (receivers.Count > 0)
            {
                foreach (AnimatorStateEventReceiver receiver in receivers)
                {
                    AnimatorClipInfo eventOwnerClipInfo = default;

                    if (eventOwnerInfo.Equals(animator.GetCurrentAnimatorStateInfo(0)))
                    {
                        eventOwnerClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
                    }
                    else if (eventOwnerInfo.Equals(animator.GetNextAnimatorStateInfo(0)))
                    {
                        eventOwnerClipInfo = animator.GetNextAnimatorClipInfo(0)[0];
                    }

                    receiver?.OnAnimationStateEventTriggered(eventType, eventOwnerInfo, eventOwnerClipInfo, animator.gameObject);
                }
            }
        }
    }
}
