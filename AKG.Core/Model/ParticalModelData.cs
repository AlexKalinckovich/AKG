namespace AKG.Core.Model;

public class ParticalModelData
{
    public List<System.Numerics.Vector4> Vertices { get; } = new();
    public List<FaceIndices[]> Faces { get; } = new();
    public List<System.Numerics.Vector3> Normals { get; } = new();
    public List<System.Numerics.Vector2> TextureCoords { get; } = new();
    
    public bool IsComplete { get; set; }
    
    public int TotalVerticesProcessed { get; set; }
    
    public int TotalFacesProcessed { get; set; }
}