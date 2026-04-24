using System.Numerics;
using System.Windows;

namespace AKG.Model.Vertex;

public readonly struct VertexData(
    Vector3 worldPosition,
    Vector3 viewPosition,
    Vector4 clipPosition,
    Point screenPoint,
    float depth,
    Vector3 normal,
    Vector2 uv)
{
    public Vector3 WorldPosition { get; } = worldPosition;
    public Vector3 ViewPosition { get; } = viewPosition;
    public Vector4 ClipPosition { get; } = clipPosition;
    public Point ScreenPoint { get; } = screenPoint;
    public float Depth { get; } = depth;
    public Vector3 Normal { get; } = normal;
    public Vector2 UV { get; } = uv;
}