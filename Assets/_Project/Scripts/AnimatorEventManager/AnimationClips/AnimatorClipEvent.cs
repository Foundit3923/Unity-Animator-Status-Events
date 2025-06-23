using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AnimatorClipEvent
{
    public enum AnimatorClipEventType
    {
        OnStartClip,
        OnStopClip
    }

    public AnimatorClipEventType EventName;
    public UnityEvent<GameObject, AnimatorStateInfo, AnimatorClipInfo, AnimatorClipEventType> OnAnimatorClipEvent;
    public bool HasTriggered = false;
}
