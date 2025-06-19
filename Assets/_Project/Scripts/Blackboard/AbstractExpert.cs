public interface IExpert
{
    //Relates to the number of operations that depend on the provided data and how 
    public enum InsistenceLevel
    {
        None,
        EnvironmentData,
        EnemyData,
        PlayerData,
        UI,
        NetworkData,
        DependencyData,
        Init
    }

    bool IsPriority(bool set = true);
    int GetInsistence(Blackboard blackboard);
    void Execute(Blackboard blackboard);
}
