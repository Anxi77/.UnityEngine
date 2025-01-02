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
    [UpdateAfter(typeof(RoomGenerationSystem))]
    public partial class WalkerSystem : SystemBase
    {
        private EntityCommandBufferSystem commandBufferSystem;
        private MapSettingsSystem settingsSystem;
        private BlobAssetStore blobAssetStore;
        private Entity corridorPrefab;

        protected override void OnCreate()
        {
            commandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            settingsSystem = World.GetOrCreateSystem<MapSettingsSystem>();
            blobAssetStore = new BlobAssetStore();

            var settings = settingsSystem.GetSettings();
            var conversionSettings = GameObjectConversionSettings.FromWorld(World, blobAssetStore);
            corridorPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                settings.CorridorPrefab,
                conversionSettings
            );
        }

        protected override void OnDestroy()
        {
            blobAssetStore.Dispose();
        }

        [BurstCompile]
        private struct WalkerJob : IJob
        {
            [ReadOnly] public NativeArray<Entity> RoomEntities;
            [ReadOnly] public ComponentDataFromEntity<RoomComponent> RoomFromEntity;
            [ReadOnly] public ComponentDataFromEntity<Translation> TranslationFromEntity;
            public EntityCommandBuffer CommandBuffer;
            public Entity CorridorPrefab;
            public int WalkerCount;
            public int WalkSteps;
            public float StepSize;
            public float3 MapSize;
            public Unity.Mathematics.Random Random;

            public void Execute()
            {
                for (int i = 0; i < WalkerCount; i++)
                {
                    // 시작 방 선택
                    int startRoomIndex = Random.NextInt(0, RoomEntities.Length);
                    Entity startRoom = RoomEntities[startRoomIndex];
                    float3 position = TranslationFromEntity[startRoom].Value;

                    for (int step = 0; step < WalkSteps; step++)
                    {
                        float3 direction = GetRandomDirection();
                        float3 nextPosition = position + direction * StepSize;

                        if (IsPositionValid(nextPosition))
                        {
                            CreateCorridor(position, nextPosition);
                            position = nextPosition;
                        }
                    }
                }
            }

            private float3 GetRandomDirection()
            {
                float random = Random.NextFloat();
                if (random < 0.25f) return new float3(0, 0, 1);
                if (random < 0.5f) return new float3(0, 0, -1);
                if (random < 0.75f) return new float3(1, 0, 0);
                return new float3(-1, 0, 0);
            }

            private bool IsPositionValid(float3 position)
            {
                return position.x >= 0 && position.x <= MapSize.x &&
                       position.z >= 0 && position.z <= MapSize.z;
            }

            private void CreateCorridor(float3 start, float3 end)
            {
                var entity = CommandBuffer.CreateEntity();
                CommandBuffer.AddComponent(entity, new CorridorComponent
                {
                    StartPosition = start,
                    EndPosition = end,
                    Length = math.distance(start, end)
                });

                CommandBuffer.AddComponent(entity, new Translation
                {
                    Value = start + (end - start) * 0.5f
                });

                CommandBuffer.AddComponent(entity, new Rotation
                {
                    Value = quaternion.LookRotation(end - start, math.up())
                });
            }
        }

        protected override void OnUpdate()
        {
            var settings = settingsSystem.GetSettings();
            var state = GetSingleton<MapGenerationState>();

            if (!state.IsGenerating || state.CurrentPhase != 2) return;

            // 모든 방 엔티티 수집
            EntityQuery roomQuery = GetEntityQuery(typeof(RoomComponent));
            var roomEntities = roomQuery.ToEntityArray(Allocator.TempJob);

            var commandBuffer = commandBufferSystem.CreateCommandBuffer();

            var job = new WalkerJob
            {
                RoomEntities = roomEntities,
                RoomFromEntity = GetComponentDataFromEntity<RoomComponent>(true),
                TranslationFromEntity = GetComponentDataFromEntity<Translation>(true),
                CommandBuffer = commandBuffer,
                CorridorPrefab = corridorPrefab,
                WalkerCount = settings.WalkerCount,
                WalkSteps = settings.WalkSteps,
                StepSize = settings.StepSize,
                MapSize = settings.MapSize,
                Random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100000))
            };

            var handle = job.Schedule();
            handle.Complete();

            roomEntities.Dispose();

            // 맵 생성 완료
            state.CurrentPhase = 3;
            state.Progress = 1f;
            state.IsGenerating = false;
            SetSingleton(state);
        }
    }
} 