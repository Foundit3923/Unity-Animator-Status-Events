# Unity Animator Status Events
This repository builds off of the work of [Adammyhre's](https://github.com/adammyhre) [Improved Unity Animation Events](https://github.com/adammyhre/Improved-Unity-Animation-Events) to evaluate and categorize the status of a Unity Animator, Unity Animator States, all aspects and features of Improved Unity Animation Events, and provide a level of transparency that I have found to be missing in the Unity Animator. The current implementation also makes use of [alexnaraghi's](https://github.com/alexnaraghi) [Unity Service Locator](https://github.com/alexnaraghi/UnityServiceLocator) as well as the blackboard system from [Adammyhre's](https://github.com/adammyhre) [Unity Behavior Tree](https://github.com/adammyhre/Unity-Behaviour-Trees/tree/master).

# Key Features
- **Custom State Events**: Configure and trigger events using Unity's built in `StateMachineBehavior` functions or implement your own logic to trigger events.
- **State Queue**: An Animator's Previous, Current, and Incoming states can be viewed in the editor.
- **Blend-Tree Support**: Blend-Tree states will have their animations updated as they change.
- **State Information**: The following details are available for all states ShortHash, ClipName, ClipDuration, WillLoop, IsPlaying, AnimatorStateStatus, Owner, and IsIntermediate.
- **Intermediate States**: Intermediate states are the pre-requisite states of a sequence that ends in a goal state (I.e. Crouch(previous) -> StandUp(intermediate) -> Chase(goal), StandUp would be an Intermediate state). I find this useful for indicating that a state is part of a larger sequence and if logic for the goal should be executed.
- **AnimatorStateProperties**: The AnimatorStateProperties script reduces the 7 required AnimatorStateEventBehavior components into a much more managable list.

# Future Plans
- **Blend Tree Support**: Currently blend trees are only minimally supported
- **Service Locator Alternative**: I manage access to "Singletons" using a service locator. This will be phased out for more generic implementation.
- **Blackboard Alternative**: I register receivers with the blackboard and access that list to communicate events. There's a better way to do this.
- **Combine Receiver and Manager**: I think that combining the receiver and manager would make for a more streamlined system.
- **AnimatorStateProperties AnimatorClipEventStateBehavior list**: I would like to make AnimatorClipEventStateBehaviors managable from AnimatorStateProperties at some point.

# Improved Unity Animation Events

![AnimationEvents](https://github.com/user-attachments/assets/ab3b9e80-1533-454b-b551-78ff8d92169f)

This repository offers a powerful, flexible system for managing **Animation Events** in Unity, with advanced capabilities for configuring, previewing, and controlling events during animation playback. It extends Unity's default animation workflow, giving developers greater control over animation events and their integration.

With this system, events can be triggered from any state in your Animator Controller, including **Blend Trees**, allowing precise control over when events fire during animations. The system also features a customizable event receiver component that links animation events to UnityEvents, making event handling more flexible and maintainable.

## Key Features

- **Custom Animation Events**: Configure and trigger animation events at specific, normalized times (0 to 1) within an animation state.
- **Blend Tree Support**: Trigger events at the blend tree level, allowing precise event control when working with multiple animations.
- **Event Receiver Component**: A reusable component that enables animation events to trigger UnityEvents, streamlining and maintaining event handling.
- **Inspector Preview Mode (Scene View)**: Preview animations and blend trees *directly in the Scene view from the Unity Editor*, providing real-time visual feedback and dramatically improving workflow efficiency.
- **T-Pose Reset**: Quickly reset characters to their default T-pose, aiding in animation rigging and debugging.
- **StateMachineBehaviour Integration**: Uses `StateMachineBehaviour` to manage events tied to specific animation states.

This system offers enhanced flexibility and control over Unity’s Animation Event system, particularly valuable for managing complex animation events in both runtime and editor-time workflows.

## YouTube

[**Improved Animation Events in Unity**](https://youtu.be/XEDi7fUCQos?sub_confirmation=1)

You can also check out my [YouTube channel](https://www.youtube.com/@git-amend?sub_confirmation=1) for more Unity content.
