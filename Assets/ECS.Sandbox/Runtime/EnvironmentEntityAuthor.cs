using Unity.Entities;
using UnityEngine;

namespace ECS.Sandbox.Runtime
{
    public class EnvironmentEntityAuthor : MonoBehaviour
    {
        class Baker : Baker<EnvironmentEntityAuthor>
        {
            public override void Bake(EnvironmentEntityAuthor authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<EnvironmentEntity>(entity);
            }
        }
    }
}