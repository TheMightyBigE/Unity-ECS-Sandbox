using Unity.Burst;
using Unity.Entities;
namespace ECS.Sandbox.Runtime
{
    /// <summary>
    /// Singleton for containing spawn configuration data
    /// </summary>
    [BurstCompile]
    public struct BouncingTimeBombConfig : IComponentData
    {
        public Entity spawnPrefab;
        public Entity firstCollisionEntity;
        public Entity secondaryCollisionEntity;
        public float effectScaler;
        public int spawnCount;
    }

    [BurstCompile]
    public struct PerishableEntity : IComponentData
    {
        public float time;
        public float maxLife;
    }

    #region Tag Components (Components with no data)
    [BurstCompile]
    public struct SpawnedEntity : IComponentData { }
    [BurstCompile]
    public struct DestroyOnContact : IComponentData { }
    [BurstCompile]
    public struct EnvironmentEntity : IComponentData { }
    #endregion
}