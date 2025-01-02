using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using MapGeneration.Components;
using UnityEngine;

namespace MapGeneration.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(BSPGenerationSystem))]
    public partial class RoomGenerationSystem : SystemBase
    {
        private EntityCommandBufferSystem commandBufferSystem;
        private MapSettingsSystem settingsSystem;
        private BlobAssetStore blobAssetStore;
        private Entity roomPrefab;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            settingsSystem = World.GetOrCreateSystem<MapSettingsSystem>();
            blobAssetStore = new BlobAssetStore();

            // 프리팹을 Entity로 변환
            var settings = settingsSystem.GetSettings();
            var conversionSettings = GameObjectConversionSettings.FromWorld(World, blobAssetStore);
            roomPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                settings.RoomPrefab,
                conversionSettings
            );
        }

        protected override void OnDestroy()
        {
            blobAssetStore.Dispose();
        }

        [BurstCompile]
        private struct RoomCreationJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            public Entity RoomPrefab;
            public float MinRoomSize;
            public Unity.Mathematics.Random Random;

            public void Execute(Entity entity, [EntityInQueryIndex] int index, in BSPNodeComponent node)
            {
                if (!node.IsLeaf) return;

                // 방 크기 계산
                float3 roomSize = new float3(
                    Random.NextFloat(MinRoomSize, node.Size.x - 1),
                    node.Size.y,
                    Random.NextFloat(MinRoomSize, node.Size.z - 1)
                );

                // 방 위치 계산
                float3 roomPosition = node.Position + (node.Size - roomSize) * 0.5f;

                // 방 엔티티 생성
                var roomEntity = CommandBuffer.Instantiate(index, RoomPrefab);

                // 컴포넌트 추가
                CommandBuffer.SetComponent(index, roomEntity, new Translation 
                { 
                    Value = roomPosition 
                });

                CommandBuffer.AddComponent(index, roomEntity, new RoomComponent
                {
                    Size = roomSize,
                    RoomId = index,
                    IsConnected = false
                });
            }
        }

        protected override void OnUpdate()
        {
            var settings = settingsSystem.GetSettings();
            var state = GetSingleton<MapGenerationState>();

            if (!state.IsGenerating || state.CurrentPhase != 1) return;

            var commandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Dependency = new RoomCreationJob
            {
                CommandBuffer = commandBuffer,
                RoomPrefab = roomPrefab,
                MinRoomSize = settings.MinRoomSize,
                Random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000))
            }.ScheduleParallel(Dependency);

            commandBufferSystem.AddJobHandleForProducer(Dependency);

            // 다음 페이즈로 진행
            state.CurrentPhase = 2;
            state.Progress = 0.66f;
            SetSingleton(state);
        }
    }
} 