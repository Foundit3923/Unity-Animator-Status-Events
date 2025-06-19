using System;
using System.Collections.Generic;
using System.Linq;

public class PriorityGroup
{
    readonly List<IExpert> experts = new();

    public bool processingPriorityActions = false;

    SortedDictionary<int, List<IExpert>> sorted = new();

    List<int> keys = new();

    List<IExpert> temp = new();

    public List<IExpert> GetExperts() => experts.ToList();

    public void RegisterPriorityAction(IExpert expert)
    {
        Preconditions.CheckNotNull(expert);
        experts.Add(expert);
        expert.IsPriority(); //tag all priority expterts
    }

    public void DeregisterPriorityAction(IExpert expert)
    {
        Preconditions.CheckNotNull(expert);
        experts.Remove(expert);
        expert.IsPriority(false);
    }

    public bool HasPriorityActions(Blackboard blackboard)
    {
        keys.Clear();
        SortActions(blackboard);
        processingPriorityActions = keys.Count > 0;
        return processingPriorityActions;
    }

    public List<Action> BlackboardIteration(Blackboard blackboard)
    {
        var actions = new List<Action>();
        foreach (int index in keys)
        {
            if (index > 0)
            {
                foreach (IExpert expert in sorted[index])
                {
                    expert?.Execute(blackboard);
                }

                actions.InsertRange(0, blackboard.PassedActions);
                blackboard.ClearActions();
            }
        }

        keys.Clear();

        // Return or execute the actions here
        return actions;
    }

    private void SortActions(Blackboard blackboard)
    {
        List<IExpert> output = new();
        foreach (IExpert expert in experts)
        {
            int insistence = expert.GetInsistence(blackboard);
            if (!keys.Contains(insistence) && insistence != 0)
            {
                keys.Add(insistence);
            }

            if (sorted.TryGetValue(insistence, out output))
            {
                sorted[insistence].Add(expert);
            }
            else
            {
                temp = new List<IExpert>
                {
                    expert
                };
                sorted[insistence] = temp;
            }
        }

        keys.OrderByDescending(k => k);
    }
}
