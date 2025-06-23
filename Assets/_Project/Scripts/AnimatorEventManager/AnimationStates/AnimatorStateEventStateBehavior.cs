using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityServiceLocator;
using static AnimatorStateEvent;
using static AnimatorEventSetter;

[DisallowMultipleComponent]
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

    private AnimatorEventSetter _properties;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (_properties == null)
        {
            this.stateInfo = stateInfo;
            AnimatorEventSetter[] propertiesArray = animator.GetBehaviours<AnimatorEventSetter>();
            _properties = propertiesArray.First(behavior => behavior.StateNameHash == stateInfo.shortNameHash);
        }

        if (stateInfo.shortNameHash == _properties.StateNameHash)
        {
            AnimatorStateEventTriggers[] triggers = new AnimatorStateEventTriggers[]
            {
                AnimatorStateEventTriggers.OnStartState,
            };
            StateEventSubscriptionEntry[] eventsToTrigger = _properties.GetTriggerSubscriptions(triggers);

            if (eventsToTrigger.Length > 0)
            {
                foreach (StateEventSubscriptionEntry entry in eventsToTrigger)
                {
                    NotifyReceiver(animator, StateInfo, entry.EventType);
                }
            }

            if (_properties.StateEventSubscriptionDict[AnimatorStateEventType.TransformPlayer])
            {
                NotifyReceiver(animator, StateInfo, _properties.IsIntermediate == true ? AnimatorStateEventType.StartIntermediateState : AnimatorStateEventType.TransformPlayer);
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.shortNameHash == _properties.StateNameHash)
        {
            AnimatorStateEventTriggers[] triggers = new AnimatorStateEventTriggers[]
            {
                AnimatorStateEventTriggers.OnUpdateState
            };
            StateEventSubscriptionEntry[] eventsToTrigger = _properties.GetTriggerSubscriptions(triggers);
            if (eventsToTrigger.Length > 0)
            {
                foreach (StateEventSubscriptionEntry entry in eventsToTrigger)
                {
                    NotifyReceiver(animator, StateInfo, entry.EventType);
                }
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.shortNameHash == _properties.StateNameHash)
        {
            AnimatorStateEventTriggers[] triggers = new AnimatorStateEventTriggers[]
            {
                AnimatorStateEventTriggers.OnEndState
            };
            StateEventSubscriptionEntry[] eventsToTrigger = _properties.GetTriggerSubscriptions(triggers);
            if (eventsToTrigger.Length > 0)
            {
                foreach (StateEventSubscriptionEntry entry in eventsToTrigger)
                {
                    NotifyReceiver(animator, StateInfo, entry.EventType);
                }
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
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    AnimatorStateInfo nextStateInfo = animator.GetNextAnimatorStateInfo(0);
                    AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
                    AnimatorClipInfo[] nextClipInfoArray = animator.GetNextAnimatorClipInfo(0);
                    if (nextClipInfoArray.Length > 0)
                    {
                        AnimatorClipInfo nextClipInfo = animator.GetNextAnimatorClipInfo(0)[0];
                    }

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
