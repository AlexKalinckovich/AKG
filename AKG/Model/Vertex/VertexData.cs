using System.Numerics;
using System.Windows;

namespace AKG.Model.Vertex;

public readonly struct VertexData
{
    public Vector3 WorldPosition { get; }
    public Vector3 ViewPosition { get; }
    public Vector4 ClipPosition { get; }
    public Point ScreenPoint { get; }
    public float Depth { get; }
    public Vector3 Normal { get; }
    public Vector2 UV { get; }

    public VertexData(
        Vector3 worldPosition,
        Vector3 viewPosition,
        Vector4 clipPosition,
        Point screenPoint,
        float depth,
        Vector3 normal,
        Vector2 uv)
    {
        WorldPosition = worldPosition;
        ViewPosition = viewPosition;
        ClipPosition = clipPosition;
        ScreenPoint = screenPoint;
        Depth = depth;
        Normal = normal;
        UV = uv;
    }
}