using System.Numerics;

namespace AKG.Render.Painting;

public class BackFaceCulling
{
    public bool IsVisible(Vector3 A, Vector3 B, Vector3 C, Vector3 target)
    {
        Vector3 u = B - A;
        Vector3 v = C - A;
    
        Vector3 n = Vector3.Cross(u, v);
    
        Vector3 toTarget = target - A;
    
        double dotProduct = Vector3.Dot(n, toTarget); 
    
        return dotProduct > 0; 
    }
}