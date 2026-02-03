using System.Numerics;

namespace ACG.Model;

public class ModelData
{
    // (x, y, z, w)
    public List<Vector4> Vertices { get; } = new List<Vector4>(10000);
        
    // (u, v)
    public List<Vector2> TextureCoords { get; } = new List<Vector2>(10000);
        
    // (x, y, z)
    public List<Vector3> Normals { get; } = new List<Vector3>(10000);
    
    public List<int[]> Faces { get; } = new List<int[]>(5000);

    public void AddPolygon(IReadOnlyList<FaceIndices> indices)
    {
        // A polygon must have at least 3 vertices to form a face
        if (indices.Count < 3) return;

        // Triangle Fan Triangulation:
        // Connect the first vertex (v0) to all subsequent pairs (v1-v2, v2-v3, etc.)
        FaceIndices v0 = indices[0];

        // CRITICAL: The loop must stop at Count - 2.
        // If Count is 3, loop runs for i=1 only. (1 < 2 is true).
        // indices[1] and indices[1+1] are valid.
        for (int i = 1; i < indices.Count - 1; i++)
        {
            AddTriangle(v0, indices[i], indices[i + 1]);
        }
    }

    private void AddTriangle(FaceIndices v1, FaceIndices v2, FaceIndices v3)
    {
        // Validate indices before adding to prevent crashes later
        // Note: Indices are already 0-based from the parser
        if (!IsIndexValid(v1.VertexIndex, Vertices.Count) ||
            !IsIndexValid(v2.VertexIndex, Vertices.Count) ||
            !IsIndexValid(v3.VertexIndex, Vertices.Count))
        {
            // Optionally log error or ignore this broken face
            return;
        }

        // Store the triangle
        Faces.Add(new int[] { v1.VertexIndex, v2.VertexIndex, v3.VertexIndex });
    }

    private bool IsIndexValid(int index, int listCount)
    {
        // In OBJ, index 0 is valid (after our -1 adjustment). 
        // We must ensure it's within the currently loaded vertices.
        return index >= 0 && index < listCount;
    }
}