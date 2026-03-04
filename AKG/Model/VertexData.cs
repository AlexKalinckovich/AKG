using System.Numerics;
using System.Windows;

namespace AKG.Model;

public struct VertexData
{
    public Vector3 WorldPosition;
    public Vector3 ViewPosition;
    public Vector4 ClipPosition;
    public Point ScreenPoint;
    public float Depth;
    public Vector3 Normal;    
    
    public static VertexData CreateDefault()
    {
        return new VertexData
        {
            ScreenPoint = new Point(-1, -1),
            WorldPosition = Vector3.Zero,
            ViewPosition = Vector3.Zero,
            ClipPosition = Vector4.Zero,
            Depth = 0,
            Normal = Vector3.UnitZ
        };
    }
}
