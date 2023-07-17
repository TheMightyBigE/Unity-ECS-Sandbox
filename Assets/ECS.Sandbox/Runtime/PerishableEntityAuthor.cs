using Unity.Entities;
using UnityEngine;

namespace ECS.Sandbox.Runtime
{
    public class PerishableEntityAuthor : MonoBehaviour
    {
        [Range(0.1f, 10f)]
        public float maxLife = 1f;
        class Baker : Baker<PerishableEntityAuthor>
        {
            public override void Bake(PerishableEntityAuthor author)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PerishableEntity>(entity, new PerishableEntity
                {
                    time = 0,
                    maxLife = author.maxLife
                });
            }
        }
    }
}