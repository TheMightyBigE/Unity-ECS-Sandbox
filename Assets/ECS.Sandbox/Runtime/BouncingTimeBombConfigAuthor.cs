using Unity.Entities;
using UnityEngine;

namespace ECS.Sandbox.Runtime
{
    /// <summary>
    /// Singleton for containing spawn configuration data
    /// </summary>
    public class BouncingTimeBombConfigAuthor : MonoBehaviour
    {
        public GameObject spawnablePrefab;
        public GameObject postCollisionPrefab;
        public GameObject residualEffectPrefab;
        public int spawnCount;

        [Range(0.1f, 10)]
        public float spawnedEffectScalar;

        class Baker : Baker<BouncingTimeBombConfigAuthor>
        {
            public override void Bake(BouncingTimeBombConfigAuthor authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                // Each authoring field corresponds to a component field of the same name.
                AddComponent(entity, new BouncingTimeBombConfig
                {
                    spawnPrefab = GetEntity(authoring.spawnablePrefab, TransformUsageFlags.Dynamic),
                    firstCollisionEntity = GetEntity(authoring.postCollisionPrefab, TransformUsageFlags.Dynamic),
                    secondaryCollisionEntity = GetEntity(authoring.residualEffectPrefab, TransformUsageFlags.Dynamic),
                    effectScaler = authoring.spawnedEffectScalar,
                    spawnCount = authoring.spawnCount
                });
            }
        }
    }
}