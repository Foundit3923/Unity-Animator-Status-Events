using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;
using static AnimatorClipEvent;

public class AnimatorClipEventReceiver : MonoBehaviour
{
    [SerializeField] List<AnimatorClipEvent> _animationEvents = new();

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

    private void Awake() => blackboard.SetListValue(BlackboardController.BlackboardKeyStrings.AnimationEventReceivers, this);

    public void OnAnimationEventTriggered(AnimatorClipEventType eventName, AnimatorStateInfo stateInfo, AnimatorClipInfo clipInfo, GameObject callingObject)
    {
        AnimatorClipEvent matchingEvent = _animationEvents.Find(se => se.EventName == eventName);
        matchingEvent?.OnAnimatorClipEvent?.Invoke(callingObject, stateInfo, clipInfo, eventName);
    }
}
