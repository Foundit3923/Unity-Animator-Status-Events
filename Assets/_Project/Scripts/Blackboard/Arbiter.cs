using System;
using System.Collections.Generic;

public class Arbiter
{
    readonly List<IExpert> experts = new();

    //Transition away from checking every expert towards experts expressing interest via event trigger

    public void RegisterExpert(IExpert expert)
    {
        Preconditions.CheckNotNull(expert);
        experts.Add(expert);
    }

    public void DeregisterExpert(IExpert expert)
    {
        Preconditions.CheckNotNull(expert);
        experts.Remove(expert);
    }

    //TODO: Evaluate workload of experts. If a high priority expert is picked too many times in a row move it to the priority queue
    public List<Action> BlackboardIteration(Blackboard blackboard)
    {
        IExpert bestExpert = null;
        int highestInsistence = 0;

        foreach (IExpert expert in experts)
        {
            int insistence = expert.GetInsistence(blackboard);
            if (insistence > highestInsistence)
            {
                highestInsistence = insistence;
                bestExpert = expert;
            }
        }

        bestExpert?.Execute(blackboard);

        var actions = new List<Action>(blackboard.PassedActions);
        blackboard.ClearActions();

        // Return or execute the actions here
        return actions;
    }
}
