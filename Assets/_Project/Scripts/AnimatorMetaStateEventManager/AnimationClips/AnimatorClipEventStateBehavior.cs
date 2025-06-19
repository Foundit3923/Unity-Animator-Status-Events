using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityServiceLocator;
using static AnimatorClipEvent;

public class AnimatorClipEventStateBehavior : StateMachineBehaviour
{
    protected AnimatorStateInfo stateInfo;
    public AnimatorStateInfo StateInfo => stateInfo;

    public AnimatorClipEventType EventName;
    [Range(0f, 1f)] public float TriggerTime;

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

    bool _hasTriggered;

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
            _hasTriggered = false;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.shortNameHash == _properties.StateNameHash)
        {
            float currentTime = stateInfo.normalizedTime % 1f;

            if (!_hasTriggered && currentTime >= TriggerTime)
            {
                NotifyReceiver(animator);
                _hasTriggered = true;
            }
        }
    }

    void NotifyReceiver(Animator animator)
    {
        if (blackboard.TryGetValue(BlackboardController.BlackboardKeyStrings.AnimationEventReceivers, out List<AnimatorClipEventReceiver> receivers))
        {
            if (receivers.Count > 0)
            {
                foreach (AnimatorClipEventReceiver receiver in receivers)
                {
                    receiver?.OnAnimationEventTriggered(EventName, animator.GetCurrentAnimatorStateInfo(0), animator.GetCurrentAnimatorClipInfo(0)[0], animator.gameObject);
                }
            }
        }
    }
}
