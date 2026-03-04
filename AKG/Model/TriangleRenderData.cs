namespace AKG.Model;

using System.Numerics;
using System.Windows;


public struct TriangleRenderData
{
    public Vector3 World0, World1, World2;
    public Point Screen0, Screen1, Screen2;
    public float Depth0, Depth1, Depth2;
    public float Intensity;
    public uint Color;
}