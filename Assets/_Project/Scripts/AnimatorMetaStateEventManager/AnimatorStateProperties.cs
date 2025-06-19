
using System.Collections.Generic;
using UnityEngine;
using static AnimatorStateEvent;

public class AnimatorStateProperties : StateMachineBehaviour
{
    protected int stateInfo = -1;
    public int StateNameHash => stateInfo == -1 ? Animator.StringToHash(this._stateName) : stateInfo;

    [SerializeField]
    private string _stateName;
    //public int StateNameHash = -1;

    [SerializeField]
    public bool IsIntermediate = false;

    [SerializeField]
    private List<AnimatorStateEventType> _stateEventSubscriptionList = new();
    public Dictionary<AnimatorStateEventType, bool> StateEventSubscriptionDict = new();

    //[SerializeField]
    //public List<AnimatorClipEventType> _clipEventSubscriptionList = new();
    //public Dictionary<AnimatorClipEventType, bool> ClipEventSubscriptionDict = new();

    private void Awake()
    {
        //if (this.StateNameHash == -1) { this.StateNameHash = Animator.StringToHash(this._stateName); }

        foreach (AnimatorStateEventType stateEventType in System.Enum.GetValues(typeof(AnimatorStateEventType)))
        {
            if (!this.StateEventSubscriptionDict.ContainsKey(stateEventType))
            {
                this.StateEventSubscriptionDict.Add(stateEventType, this._stateEventSubscriptionList.Contains(stateEventType) == true ? true : false);
            }
        }

        //foreach(AnimatorClipEventType clipEventType in System.Enum.GetValues(typeof(AnimatorClipEventType)))
        //{
        //    if (!ClipEventSubscriptionDict.ContainsKey(clipEventType))
        //    {
        //        ClipEventSubscriptionDict.Add(clipEventType, _clipEventSubscriptionList.Contains(clipEventType) == true ? true : false);
        //    }
        //}
    }
}
