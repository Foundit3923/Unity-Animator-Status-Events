using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityServiceLocator;
using static AnimatorClipEvent;
using static AnimatorEventSetter;

[DisallowMultipleComponent]
public class AnimatorClipEventStateBehavior : StateMachineBehaviour
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

    private List<ClipEventSubscriptionEntry> _clipEventSubscriptionList = new();

    private void OnEnable()
    {
        _clipEventSubscriptionList.Clear();
        _clipEventSubscriptionList = _properties.getClipSubscriptions();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_properties == null)
        {
            this.stateInfo = stateInfo;
            AnimatorEventSetter[] propertiesArray = animator.GetBehaviours<AnimatorEventSetter>();
            _properties = propertiesArray.First(behavior => behavior.StateNameHash == stateInfo.shortNameHash);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.shortNameHash == _properties.StateNameHash)
        {
            float currentTime = stateInfo.normalizedTime % 1f;
            ClipEventSubscriptionEntry[] subscriptionsToProc = _clipEventSubscriptionList.Where(c => c.TriggerTime <= currentTime).ToArray();
            if (subscriptionsToProc.Length > 0)
            {
                foreach(ClipEventSubscriptionEntry entry in subscriptionsToProc) 
                {
                    NotifyReceiver(animator, entry.EventType);
                    _clipEventSubscriptionList.Remove(entry);
                }
            }
        }
    }

    void NotifyReceiver(Animator animator, AnimatorClipEventType eventType)
    {
        if (blackboard.TryGetValue(BlackboardController.BlackboardKeyStrings.AnimationEventReceivers, out List<AnimatorClipEventReceiver> receivers))
        {
            if (receivers.Count > 0)
            {
                foreach (AnimatorClipEventReceiver receiver in receivers)
                {
                    receiver?.OnAnimationEventTriggered(eventType, animator.GetCurrentAnimatorStateInfo(0), animator.GetCurrentAnimatorClipInfo(0)[0], animator.gameObject);
                }
            }
        }
    }
}
