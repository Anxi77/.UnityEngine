using UnityEngine;
using Unity.Entities;
using MapGeneration.Systems;
using MapGeneration.Components;

public class MapGenerationController : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject corridorPrefab;
    [SerializeField] private int maxDepth = 4;
    [SerializeField] private float minRoomSize = 5f;
    [SerializeField] private Vector3 mapSize = new Vector3(50f, 10f, 50f);
    [SerializeField] private int walkerCount = 3;
    [SerializeField] private int walkSteps = 100;
    [SerializeField] private float stepSize = 1f;

    private World world;
    private MapSettingsSystem settingsSystem;

    private void Start()
    {
        world = World.DefaultGameObjectInjectionWorld;
        settingsSystem = world.GetOrCreateSystem<MapSettingsSystem>();

        // 설정 업데이트
        var settings = new MapSettingsSystem.Settings
        {
            MaxDepth = maxDepth,
            MinRoomSize = minRoomSize,
            MapSize = new Unity.Mathematics.float3(mapSize.x, mapSize.y, mapSize.z),
            WalkerCount = walkerCount,
            WalkSteps = walkSteps,
            StepSize = stepSize,
            RoomPrefab = roomPrefab,
            CorridorPrefab = corridorPrefab
        };

        settingsSystem.UpdateSettings(settings);
    }

    public void GenerateMap()
    {
        settingsSystem.StartGeneration();
    }

    private void Update()
    {
        // 진행 상황 모니터링
        var entityManager = world.EntityManager;
        var stateEntity = entityManager.CreateEntityQuery(typeof(MapGenerationState)).GetSingletonEntity();
        var state = entityManager.GetComponentData<MapGenerationState>(stateEntity);

        if (state.IsGenerating)
        {
            Debug.Log($"Map Generation Progress: {state.Progress * 100}%");
        }
    }
} 