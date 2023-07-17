using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace ECS.Sandbox.Runtime
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    [BurstCompile]
    public partial class SpawnableEntitySystem : SystemBase
    {
        private Random rand;

        [BurstCompile]
        protected override void OnCreate()
        {
            base.OnCreate();
            rand = new Random(117);
            RequireForUpdate<BouncingTimeBombConfig>();
            RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            var config = SystemAPI.GetSingleton<BouncingTimeBombConfig>();

            if (Input.GetKeyDown(KeyCode.S))
            {
                for (int i = 0; i < config.spawnCount; i++)
                {
                    var newEntity = EntityManager.Instantiate(config.spawnPrefab);
                    EntityManager.SetComponentData(newEntity, new LocalTransform
                    {
                        Position = new float3(rand.NextFloat(-18, 18), rand.NextFloat(8, 20), rand.NextFloat(-18, 18)),
                        Rotation = quaternion.Euler(new float3(rand.NextFloat(-15, 15), rand.NextFloat(8, 10), rand.NextFloat(-15, 15))),
                        Scale = 1
                    });
                    EntityManager.SetComponentData(newEntity, new PhysicsVelocity
                    {
                        Linear = new float3(rand.NextFloat(-5, 5), rand.NextFloat(-30, -15), rand.NextFloat(-5, 5))
                    });
                }
            }

            // Schedule ForEach to update time and destroy if 'perished'
            float dT = World.Time.DeltaTime;
            Entities
                .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
                .ForEach(
                    (Entity entity, EntityCommandBuffer ecb, ref PerishableEntity e/*, in MaxLifeValue l*/) =>
                    {
                        e.time += dT;
                        if (e.time > e.maxLife)
                        {
                            ecb.DestroyEntity(entity);
                        }
                    }
                ).ScheduleParallel();
        }
    }
}
