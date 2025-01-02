using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using MapGeneration.Components;

namespace MapGeneration.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BSPGenerationSystem : SystemBase
    {
        private EntityCommandBufferSystem commandBufferSystem;
        private MapSettingsSystem settingsSystem;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            settingsSystem = World.GetOrCreateSystem<MapSettingsSystem>();
        }

        [BurstCompile]
        private struct BSPSplitJob : IJob
        {
            public int MaxDepth;
            public float MinRoomSize;
            public float3 MapSize;
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            public Unity.Mathematics.Random Random;

            public void Execute()
            {
                SplitNode(float3.zero, MapSize, 0);
            }

            private void SplitNode(float3 position, float3 size, int depth)
            {
                if (depth >= MaxDepth) return;
                if (size.x < MinRoomSize * 2 || size.z < MinRoomSize * 2) return;

                var entity = CommandBuffer.CreateEntity(0);
                CommandBuffer.AddComponent(0, entity, new BSPNodeComponent
                {
                    Position = position,
                    Size = size,
                    Depth = depth,
                    IsLeaf = true
                });

                bool splitHorizontal = Random.NextFloat() > 0.5f;
                float splitPoint;

                if (splitHorizontal)
                {
                    splitPoint = Random.NextFloat(MinRoomSize, size.x - MinRoomSize);
                    SplitNode(position, new float3(splitPoint, size.y, size.z), depth + 1);
                    SplitNode(
                        new float3(position.x + splitPoint, position.y, position.z),
                        new float3(size.x - splitPoint, size.y, size.z),
                        depth + 1
                    );
                }
                else
                {
                    splitPoint = Random.NextFloat(MinRoomSize, size.z - MinRoomSize);
                    SplitNode(position, new float3(size.x, size.y, splitPoint), depth + 1);
                    SplitNode(
                        new float3(position.x, position.y, position.z + splitPoint),
                        new float3(size.x, size.y, size.z - splitPoint),
                        depth + 1
                    );
                }
            }
        }

        protected override void OnUpdate()
        {
            var settings = settingsSystem.GetSettings();
            var state = GetSingleton<MapGenerationState>();

            if (!state.IsGenerating || state.CurrentPhase != 0) return;

            var commandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            var job = new BSPSplitJob
            {
                MaxDepth = settings.MaxDepth,
                MinRoomSize = settings.MinRoomSize,
                MapSize = settings.MapSize,
                CommandBuffer = commandBuffer,
                Random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000))
            };

            var handle = job.Schedule();
            handle.Complete();

            // 다음 페이즈로 진행
            state.CurrentPhase = 1;
            state.Progress = 0.33f;
            SetSingleton(state);
        }
    }
} 