using System;
using UnityEngine;

[Serializable]
public class AnimatorState
{
    [Flags] public enum AnimatorStateStatus
    {
        Running = 1 << 0,
        Finished = 1 << 1,
        Idle = 1 << 2,
        None = 0
    }

    public int Hash;
    public string ClipName;
    public float ClipDuration;
    public bool WillLoop;
    public bool IsPlaying;
    public AnimatorStateStatus Status;
    public GameObject Owner;
    public bool IsIntermediate;
    public bool IsInitialized = false;

    public AnimatorState(int hash,
        string clipName,
        float clipDuration,
        bool willLoop,
        bool isPlaying,
        AnimatorStateStatus status,
        GameObject owner,
        bool isIntermediate)
    {
        Hash = hash == 0 ? 0 : hash;
        ClipName = clipName;
        ClipDuration = clipDuration;
        WillLoop = willLoop;
        IsPlaying = isPlaying;
        Status = status;
        Owner = owner;
        IsIntermediate = isIntermediate;
    }

    public bool EvalStatus(AnimatorStateStatus status) => (Status & status) != AnimatorStateStatus.None;
}
