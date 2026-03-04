using System.Numerics;

namespace AKG.Render.Culling;


public sealed class FaceCullingStrategy
{
    public bool IsVisible(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
    {
        Vector3 edge1 = vertex1 - vertex0;
        Vector3 edge2 = vertex2 - vertex0;
        Vector3 normal = Vector3.Cross(edge1, edge2);
        
        double dotProduct = Vector3.Dot(normal, -vertex0);
        
        return dotProduct > 0;
    }
}