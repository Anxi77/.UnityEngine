using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using MapGeneration.Components;

namespace MapGeneration.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class MapSettingsSystem : ComponentSystem
    {
        public struct Settings
        {
            // BSP 설정
            public int MaxDepth;
            public float MinRoomSize;
            public float3 MapSize;
            
            // Walker 설정
            public int WalkerCount;
            public int WalkSteps;
            public float StepSize;
            
            // 프리팹
            public GameObject RoomPrefab;
            public GameObject CorridorPrefab;
        }

        private Settings currentSettings;
        private Entity stateEntity;

        protected override void OnCreate()
        {
            // 기본 설정 초기화
            currentSettings = new Settings
            {
                MaxDepth = 4,
                MinRoomSize = 5f,
                MapSize = new float3(50f, 10f, 50f),
                WalkerCount = 3,
                WalkSteps = 100,
                StepSize = 1f
            };

            // 상태 엔티티 생성
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            stateEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(stateEntity, new MapGenerationState
            {
                IsGenerating = false,
                CurrentPhase = 0,
                Progress = 0f
            });
        }

        public void StartGeneration()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.SetComponentData(stateEntity, new MapGenerationState
            {
                IsGenerating = true,
                CurrentPhase = 0,
                Progress = 0f
            });
        }

        public Settings GetSettings() => currentSettings;
        public void UpdateSettings(Settings newSettings) => currentSettings = newSettings;

        protected override void OnUpdate() { }
    }
} 