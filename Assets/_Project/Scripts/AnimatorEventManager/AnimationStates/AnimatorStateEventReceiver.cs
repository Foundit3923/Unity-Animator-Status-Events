using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;
using static AnimatorStateEvent;

public class AnimatorStateEventReceiver : MonoBehaviour
{
    [SerializeField] public List<AnimatorStateEvent> _animationStateEvents = new();

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

    private void Awake() => blackboard.SetListValue(BlackboardController.BlackboardKeyStrings.AnimationStateEventReceivers, this);

    public void OnAnimationStateEventTriggered(AnimatorStateEventType eventName, AnimatorStateInfo stateInfo, AnimatorClipInfo clipInfo, GameObject callingObject)
    {
        AnimatorStateEvent matchingEvent = _animationStateEvents.Find(se => se.StateName == eventName);
        matchingEvent?.OnAnimatorStateEvent?.Invoke(callingObject, stateInfo, clipInfo, eventName);
    }
}
