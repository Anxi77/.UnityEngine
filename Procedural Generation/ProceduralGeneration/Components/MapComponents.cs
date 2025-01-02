using Unity.Entities;
using Unity.Mathematics;

namespace MapGeneration.Components
{
    // 방 컴포넌트
    public struct RoomComponent : IComponentData
    {
        public float3 Size;
        public int RoomId;
        public bool IsConnected;
    }

    // 복도 컴포넌트
    public struct CorridorComponent : IComponentData
    {
        public float3 StartPosition;
        public float3 EndPosition;
        public float Length;
    }

    // BSP 노드 컴포넌트
    public struct BSPNodeComponent : IComponentData
    {
        public float3 Position;
        public float3 Size;
        public int Depth;
        public bool IsLeaf;
        public Entity Left;
        public Entity Right;
    }

    // 맵 생성 상태 컴포넌트
    public struct MapGenerationState : IComponentData
    {
        public bool IsGenerating;
        public int CurrentPhase;
        public float Progress;
    }
} 