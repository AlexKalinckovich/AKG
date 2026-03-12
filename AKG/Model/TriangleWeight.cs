namespace AKG.Model;

using System.Numerics;
using System.Windows;


public readonly struct TriangleWeight
{
    public readonly float Weight0;
    public readonly float Weight1;
    public readonly float Weight2;
    
    public TriangleWeight(float weight0, float weight1, float weight2)
    {
        Weight0 = weight0;
        Weight1 = weight1;
        Weight2 = weight2;
    }
}