using System;
using UnityEngine;

[Serializable]
public class AnimatorState
{
    public enum AnimatorStateStatus
    {
        Running,
        Finished,
        Idle
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
}
