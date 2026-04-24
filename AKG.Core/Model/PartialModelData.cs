using System.Numerics;
using AKG.Model;

namespace AKG.Core.Model;

public class PartialModelData
{
    public List<Vector4> Vertices { get; } = new();
    public List<Face> Faces { get; } = new();
    public List<Vector3> Normals { get; } = new();
    public List<Vector2> TextureCoords { get; } = new();

    public int TotalVerticesProcessed { get; set; }
    
    public int TotalFacesProcessed { get; set; }
}