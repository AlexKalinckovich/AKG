using System.Numerics;
using System.Windows;

namespace AKG.Model;

public struct Triangle
{
    public Vector3 ViewPositionA;
    public Vector3 ViewPositionB;
    public Vector3 ViewPositionC;
    
    public Vector3 WorldPositionA;
    public Vector3 WorldPositionB;
    public Vector3 WorldPositionC;
    
    public Point ScreenPointA;
    public Point ScreenPointB;
    public Point ScreenPointC;
    
    public float DepthA;
    public float DepthB;
    public float DepthC;
}