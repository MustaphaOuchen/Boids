using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace ECS
{
    public struct RandomData : IComponentData
    {
        public Random Value;
    }
}