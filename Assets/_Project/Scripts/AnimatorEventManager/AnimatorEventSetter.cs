
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using static AnimatorClipEvent;
using static AnimatorStateEvent;

[RequireComponent(typeof(AnimatorClipEventStateBehavior))]
[RequireComponent(typeof(AnimatorStateEventStateBehavior))]
public class AnimatorEventSetter : StateMachineBehaviour
{
    protected int stateInfo = -1;
    public int StateNameHash => stateInfo == -1 ? Animator.StringToHash(this._stateName) : stateInfo;

    [SerializeField]
    private string _stateName;

    [SerializeField]
    public bool IsIntermediate = false;

    [SerializeField]
    public AnimatorClipEventStateBehavior ClipEventStateBehavior;

    [SerializeField]
    public AnimatorStateEventStateBehavior StateEventBehavior;

    public Dictionary<AnimatorStateEventType, bool> StateEventSubscriptionDict = new();

    [Serializable]
    public class StateEventSubscriptionEntry : ISerializable
    {
        public AnimatorStateEventType EventType;
        public AnimatorStateEventTriggers Subscription;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Event", this.EventType);
            info.AddValue("Subscription", this.Subscription);
        }
    }

    [SerializeField]
    private List<StateEventSubscriptionEntry> _stateEventSubscriptionList = new()
    {
        new StateEventSubscriptionEntry { EventType = AnimatorStateEventType.OnStartState, Subscription = AnimatorStateEventTriggers.OnStartState },
        new StateEventSubscriptionEntry { EventType = AnimatorStateEventType.OnEndState, Subscription = AnimatorStateEventTriggers.OnEndState }
    };

    public StateEventSubscriptionEntry GetTarget(AnimatorStateEventType type) => _stateEventSubscriptionList.Where(d => d.EventType == type).FirstOrDefault();
    public StateEventSubscriptionEntry[] GetTriggerSubscriptions(AnimatorStateEventTriggers[] triggers) => _stateEventSubscriptionList.Where(d => TriggerStateEventPredicate(triggers, d)).ToArray();
    private static bool TriggerStateEventPredicate(AnimatorStateEventTriggers[] triggers, StateEventSubscriptionEntry entry) => triggers.Where(t => entry.Subscription.Equals(t)).ToArray().Length > 0;
    public bool HasStateTrigger(AnimatorStateEventTriggers type) => _stateEventSubscriptionList.Where(d => d.Subscription == type).ToArray().Length > 0;

    [Serializable]
    public class ClipEventSubscriptionEntry : ISerializable
    {
        public AnimatorClipEventType EventType;
        [Range(0f, 1f)] public float TriggerTime;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Event", this.EventType);
            info.AddValue("TriggerTime", this.TriggerTime);
        }
    }

    [SerializeField]
    private List<ClipEventSubscriptionEntry> _clipEventSubscriptionList = new()
    {
        new ClipEventSubscriptionEntry { EventType = AnimatorClipEventType.OnStartClip, TriggerTime = 0f },
        new ClipEventSubscriptionEntry { EventType = AnimatorClipEventType.OnStopClip, TriggerTime = 0.99f }
    };

    public List<ClipEventSubscriptionEntry> getClipSubscriptions()  => _clipEventSubscriptionList;
}
