using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace ECS.Sandbox.Runtime
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsWorld))]
    [BurstCompile]
    public partial struct BouncingCollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<BouncingTimeBombConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            var config = SystemAPI.GetSingleton<BouncingTimeBombConfig>();

            // Schedule job to handle collision of spawned entitites with environment
            state.Dependency = new CollisionJob()
            {
                spawnedEntityLookup = SystemAPI.GetComponentLookup<SpawnedEntity>(true),
                environmentEntityLookup = SystemAPI.GetComponentLookup<EnvironmentEntity>(true),
                destroyOnContactLookup = SystemAPI.GetComponentLookup<DestroyOnContact>(true),
                world = physicsWorldSingleton.PhysicsWorld,
                config = config,
                ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged)
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }

        [BurstCompile]
        private struct CollisionJob : ICollisionEventsJob
        {
            [ReadOnly] public ComponentLookup<SpawnedEntity> spawnedEntityLookup;
            [ReadOnly] public ComponentLookup<DestroyOnContact> destroyOnContactLookup;
            [ReadOnly] public ComponentLookup<EnvironmentEntity> environmentEntityLookup;
            public EntityCommandBuffer ecb;
            public PhysicsWorld world;
            public BouncingTimeBombConfig config;
            public void Execute(CollisionEvent collisionEvent)
            {
                if (spawnedEntityLookup.HasComponent(collisionEvent.EntityA) && environmentEntityLookup.HasComponent(collisionEvent.EntityB))
                {
                    SpawnedEntity s;
                    if (spawnedEntityLookup.TryGetComponent(collisionEvent.EntityA, out s))
                    {
                        if (destroyOnContactLookup.HasComponent(collisionEvent.EntityA))
                        {
                            // EntityA has already collided with the environment, this represents a second collision. Destroy the object
                            ecb.DestroyEntity(collisionEvent.EntityA);
                            var residualEntity = ecb.Instantiate(config.secondaryCollisionEntity);
                            ecb.SetComponent<LocalTransform>(residualEntity, new LocalTransform
                            {
                                Position = collisionEvent.CalculateDetails(ref world).AverageContactPointPosition,
                                Rotation = quaternion.identity,
                                Scale = config.effectScaler
                            });
                            return;
                        }

                        ecb.AddComponent<DestroyOnContact>(collisionEvent.EntityA);
                        // This is the first time EntityA has collided with the environment
                        var collisionEffectEntity = ecb.Instantiate(config.firstCollisionEntity);
                        ecb.AppendToBuffer<LinkedEntityGroup>(collisionEvent.EntityA, new LinkedEntityGroup { Value = collisionEffectEntity });
                        ecb.AddComponent<Parent>(collisionEffectEntity, new Parent() { Value = collisionEvent.EntityA });
                        ecb.SetComponent<LocalTransform>(collisionEffectEntity, new LocalTransform
                        {
                            Position = float3.zero,
                            Rotation = quaternion.identity,
                            Scale = config.effectScaler
                        });
                    }
                }
            }
        }
    }
}