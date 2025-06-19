public interface IServerExpert : IExpert
{
    //All interactions with this expert should happen on the server side blackboard
    new public enum ServerInsistenceLevel
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
}
