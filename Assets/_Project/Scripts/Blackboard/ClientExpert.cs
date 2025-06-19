public interface IClientExpert : IExpert
{
    //All interactions with this expert should happen on the client side blackboard
    new public enum ClientInsistenceLevel
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
