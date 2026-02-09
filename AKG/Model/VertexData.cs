using System.Numerics;
using System.Windows;

namespace AKG.Model;

public struct VertexData
{
    public Vector3 WorldPosition;
    public Vector3 ViewPosition;
    public Vector3 ClipPosition;
    public Point ScreenPoint;
    public float Depth;
}
