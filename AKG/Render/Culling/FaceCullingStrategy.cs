using System.Numerics;
using AKG.Model;

namespace AKG.Render.Culling;


public static class FaceCullingStrategy
{
    public static bool IsTriangleVisible(Triangle triangle)
    {
        Vector3 edge1 = triangle.Vertex1.ViewPosition - triangle.Vertex2.ViewPosition;
        Vector3 edge2 = triangle.Vertex2.ViewPosition - triangle.Vertex0.ViewPosition;
        Vector3 normal = Vector3.Cross(edge1, edge2);
        
        double dotProduct = Vector3.Dot(normal, -triangle.Vertex0.ViewPosition);
        
        return dotProduct > 0;
    }
}