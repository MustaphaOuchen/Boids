using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;

namespace ECS
{
    public partial class BoidLimitInSpaceSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithBurst().ForEach((ref Translation trans, in BoidData boidData) =>
            {
                float xInterval = boidData.xInterval;
                float yInterval = boidData.yInterval;
                float zInterval = boidData.zInterval;
                if (trans.Value.x > xInterval)
                {
                    trans.Value.x = -xInterval;
                }
                else if (trans.Value.x < -xInterval)
                {
                    trans.Value.x = xInterval;
                }
                if (trans.Value.y > yInterval)
                {
                    trans.Value.y = -yInterval;
                }
                else if (trans.Value.y < -yInterval)
                {
                    trans.Value.y = yInterval;
                }
                //Added z-Axis
                if (boidData.livesIn3DSpace)
                {
                    if (trans.Value.z > zInterval)
                    {
                        trans.Value.z = -zInterval;

                    }
                    else if (trans.Value.z < -zInterval)
                    {
                        trans.Value.z = zInterval;
                    }
                }
            }).ScheduleParallel();
        }
    }
}