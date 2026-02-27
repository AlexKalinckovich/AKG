using System.Numerics;

namespace AKG.Render.Painting;

public class BackFaceCulling
{
    public bool IsVisible(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 u = B - A;
        Vector3 v = C - A;
    
        Vector3 n = Vector3.Cross(u, v);
        
        double dotProduct = Vector3.Dot(n, -A); 
    
        return dotProduct > 0; 
    }
}