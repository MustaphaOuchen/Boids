using Common;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Time = UnityEngine.Time;

namespace ECS
{
    public partial class BoidMovementSystemSpatialHashing : SystemBase
    {
        private NativeMultiHashMap<int, OtherBoidData> boidToNeighbors; //for mapping boid positions to neighbors

        public void InitRandomData()
        {
            Entities.ForEach((Entity e, int entityInQueryIndex, ref RandomData randomData) =>
            {
                randomData.Value = Random.CreateFromIndex((uint)entityInQueryIndex);
            }).Run();
        }

        protected override void OnUpdate()
        {
            boidToNeighbors.Clear();
            float deltaTime = Time.DeltaTime;
            int amountBoids = BoidsManagerDOTS.Instance.amountBoids;
            BoidsManagerDOTS boidsManagerDOTS = BoidsManagerDOTS.Instance as BoidsManagerDOTS;
            
            float conversionFactor = boidsManagerDOTS.conversionFactor;
            if (amountBoids > boidToNeighbors.Capacity)
            {
                boidToNeighbors.Capacity = amountBoids;               
            }
            //Caching the current entities
            NativeMultiHashMap<int, OtherBoidData>.ParallelWriter boidToNeighborsParallelWriter = boidToNeighbors.AsParallelWriter(); //Writing parallel to datastructure possible
            //Save dependency 
            var inputDeps = Dependency;
            var createDataStructureJobHandle = Entities.ForEach((ref BoidData boidData, ref Translation translation) =>
            {
                //Duplicate data of entity
                float3 position = new float3(translation.Value.x, translation.Value.y, translation.Value.z);
                OtherBoidData boidToAdd = new OtherBoidData
                {
                    velocity = boidData.velocity,
                    position = position
                };
                int key = SpatialHasher.HashPositionFloat3(position, amountBoids,conversionFactor);
                boidToNeighborsParallelWriter.Add(key, boidToAdd);
            }).ScheduleParallel(inputDeps);

            //Structure which is used for the job
            NativeMultiHashMap<int, OtherBoidData> boidToNeighborsReading = boidToNeighbors;
            var updateBoidsJobHandle = Entities.WithReadOnly(boidToNeighborsReading).ForEach(( 
                ref Unity.Transforms.Translation translation, 
                ref BoidData boidData, 
                ref Unity.Transforms.Rotation rotation,
                ref RandomData randomData) => 
            {
                int key = SpatialHasher.HashPositionFloat3(translation.Value, amountBoids,conversionFactor);
                NativeMultiHashMapIterator<int> iterator;
                //mate/aka neighbor
                OtherBoidData otherBoidData;
                float3 alignmentVector = float3.zero; //steering alignment
                float3 cohesionVector = float3.zero;
                float3 separationVector = float3.zero;
                float3 averageAlignmentVector = float3.zero;
                float3 averageCohesionVector = float3.zero;
                int total = 0;          
                if (boidToNeighborsReading.TryGetFirstValue(key, out otherBoidData, out iterator))
                {
                    do
                    {
                        float3 otherBoidPosition = otherBoidData.position;
                        if (translation.Value.Equals(otherBoidPosition))
                            continue;
                        float sqaurdDistance = math.distancesq(translation.Value, otherBoidPosition);
                        if (sqaurdDistance < boidData.perceptionRadiusSquared)
                        {
                            averageAlignmentVector += otherBoidData.velocity;

                            averageCohesionVector += otherBoidData.position;

                            float3 difference = translation.Value - otherBoidPosition;
                            difference /= sqaurdDistance; //Closer Boid <-> greater Force
                            separationVector += difference;

                            total++;
                        }

                    } while (boidToNeighborsReading.TryGetNextValue(out otherBoidData, ref iterator)); //Next neighbour in spartial hashmap, get neighbours in O(1)

                    if (total > 0)
                    {
                        //Alignement
                        averageAlignmentVector /= total;
                        //normalize 
                        averageAlignmentVector = (averageAlignmentVector / math.length(averageAlignmentVector) * boidData.maxSpeed);
                        alignmentVector = averageAlignmentVector - boidData.velocity;
                        //No limiting of alignment                    
                        //Cohesion
                        averageCohesionVector /= total;
                        float3 vecToCom = averageCohesionVector - translation.Value;                     
                        cohesionVector = vecToCom - boidData.velocity;
                        cohesionVector = StaticECSHelperMethods.ClampMagnitude(cohesionVector, boidData.maxForce);
                        //Separation
                        separationVector = StaticECSHelperMethods.ClampMagnitude(separationVector, boidData.maxForce);
                    }

                    alignmentVector *= boidData.alignmentScale;
                    cohesionVector *= boidData.cohesionScale;
                    separationVector *= boidData.separationScale;
                    boidData.accelerationForce = alignmentVector + cohesionVector + separationVector;             
                    //Gravity
                    if (boidData.allowAdditionalBehaviour)
                    {
                        //Gravity
                        float3 gravityVector = math.down() * deltaTime;
                        gravityVector = StaticECSHelperMethods.ClampMagnitude(gravityVector, boidData.maxForce) * boidData.gravityFactor;
                        boidData.accelerationForce += gravityVector;
                        //Randomness
                        Random random = randomData.Value;
                        float randX = random.NextFloat(-1, 1);
                        float randY = random.NextFloat(-1, 1);
                        float randZ = boidData.livesIn3DSpace ? random.NextFloat(-1f, 1f) : 0f;
                        float3 randomForce = new float3(randX, randY, randZ);
                        randomForce = math.normalize(randomForce);
                        randomForce = StaticECSHelperMethods.ClampMagnitude(randomForce, boidData.maxForce) * boidData.randomScale;
                        boidData.accelerationForce += randomForce;
                    }
                    //Apply and reset force
                    boidData.velocity += boidData.accelerationForce;
                    boidData.velocity = StaticECSHelperMethods.ClampMagnitude(boidData.velocity, boidData.maxSpeed);
                    boidData.accelerationForce = float3.zero;
                    //Movement
                    translation.Value += boidData.velocity * deltaTime;
                }
            }).ScheduleParallel(createDataStructureJobHandle);
            this.Dependency = updateBoidsJobHandle;
        }

        protected override void OnCreate()
        {
            boidToNeighbors = new NativeMultiHashMap<int, OtherBoidData>(0,Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            boidToNeighbors.Dispose();
        }        
    }
}