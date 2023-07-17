using Unity.Entities;
using UnityEngine;

namespace ECS.Sandbox.Runtime
{
    public class SpawnableEntityAuthor : MonoBehaviour
    {
        class Baker : Baker<SpawnableEntityAuthor>
        {
            public override void Bake(SpawnableEntityAuthor authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<SpawnedEntity>(entity);
            }
        }
    }
}