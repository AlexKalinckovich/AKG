using System.Numerics;
using AKG.Core.Model;

namespace AKG.Matrix;

public class Temp
{
    
    public static ObjModel CreateIcosahedron()
    {
        ObjModel currentObjModel = new ObjModel();
        
        float t = (1f + (float)Math.Sqrt(5)) / 2f;
        
        List<Vector4> vertices = new List<Vector4>
        {
            new Vector4(-1,  t,  0, 1),
            new Vector4( 1,  t,  0, 1),
            new Vector4(-1, -t,  0, 1),
            new Vector4( 1, -t,  0, 1),
            
            new Vector4( 0, -1,  t, 1),
            new Vector4( 0,  1,  t, 1),
            new Vector4( 0, -1, -t, 1),
            new Vector4( 0,  1, -t, 1),
            
            new Vector4( t,  0, -1, 1),
            new Vector4( t,  0,  1, 1),
            new Vector4(-t,  0, -1, 1),
            new Vector4(-t,  0,  1, 1)
        };
        
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector4 v = vertices[i];
            float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            vertices[i] = new Vector4(v.X/length, v.Y/length, v.Z/length, 1);
        }
        
        List<FaceIndices[]> faces = new List<FaceIndices[]>
        {
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(12, 0, 0), new FaceIndices(6, 0, 0) },
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(6, 0, 0), new FaceIndices(2, 0, 0) },
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(2, 0, 0), new FaceIndices(8, 0, 0) },
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(8, 0, 0), new FaceIndices(11, 0, 0) },
            new[] { new FaceIndices(1, 0, 0), new FaceIndices(11, 0, 0), new FaceIndices(12, 0, 0) },
            
            new[] { new FaceIndices(2, 0, 0), new FaceIndices(6, 0, 0), new FaceIndices(10, 0, 0) },
            new[] { new FaceIndices(6, 0, 0), new FaceIndices(12, 0, 0), new FaceIndices(5, 0, 0) },
            new[] { new FaceIndices(12, 0, 0), new FaceIndices(11, 0, 0), new FaceIndices(3, 0, 0) },
            new[] { new FaceIndices(11, 0, 0), new FaceIndices(8, 0, 0), new FaceIndices(7, 0, 0) },
            new[] { new FaceIndices(8, 0, 0), new FaceIndices(2, 0, 0), new FaceIndices(9, 0, 0) },
            
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(10, 0, 0), new FaceIndices(5, 0, 0) },
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(5, 0, 0), new FaceIndices(3, 0, 0) },
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(3, 0, 0), new FaceIndices(7, 0, 0) },
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(7, 0, 0), new FaceIndices(9, 0, 0) },
            new[] { new FaceIndices(4, 0, 0), new FaceIndices(9, 0, 0), new FaceIndices(10, 0, 0) },
            
            new[] { new FaceIndices(5, 0, 0), new FaceIndices(10, 0, 0), new FaceIndices(6, 0, 0) },
            new[] { new FaceIndices(3, 0, 0), new FaceIndices(5, 0, 0), new FaceIndices(12, 0, 0) },
            new[] { new FaceIndices(7, 0, 0), new FaceIndices(3, 0, 0), new FaceIndices(11, 0, 0) },
            new[] { new FaceIndices(9, 0, 0), new FaceIndices(7, 0, 0), new FaceIndices(8, 0, 0) },
            new[] { new FaceIndices(10, 0, 0), new FaceIndices(9, 0, 0), new FaceIndices(2, 0, 0) }
        };
        
        currentObjModel.AddVertices(vertices);
        currentObjModel.AddFaces(faces);

        return currentObjModel;
    }
}