using System.Numerics;
using AKG.Core.Model;
using AKG.Model;
using AKG.Model.Vertex;

namespace AKG.Matrix;

public sealed class VertexTransformer
{
    private const uint VertexCountParallelThreshold = 64;
    
    private readonly VertexTransformCalculator _transformCalculator;
    private readonly VertexDataBufferManager _bufferManager;

    public VertexTransformer(TransformationMatrixManager matrixManager, int viewportWidth, int viewportHeight)
    {
        _transformCalculator = new VertexTransformCalculator(matrixManager, viewportWidth, viewportHeight);
        _bufferManager = new VertexDataBufferManager();
    }

    public ReadOnlyMemory<VertexData> TransformVertices(in ObjModel model)
    {
        int totalCorners = CalculateTotalCorners(model);
        
        VertexData[] vertexBuffer = ProcessVertices(model, totalCorners);
        
        return new ReadOnlyMemory<VertexData>(vertexBuffer, 0, totalCorners);
    }

    private static int CalculateTotalCorners(ObjModel model)
    {
        int totalCorners = 0;
        int faceCount = model.Faces.Count;
        for (int i = 0; i < faceCount; i++)
        {
            totalCorners += model.Faces[i].ActualCount;
        }
        return totalCorners;
    }

    private VertexData[] ProcessVertices(ObjModel model, int totalCorners)
    {
        VertexData[] buffer = _bufferManager.GetOrCreateBuffer(totalCorners);
        
        int faceCount = model.Faces.Count;
        if (faceCount < VertexCountParallelThreshold)
        {
            ProcessVerticesSequentially(model, buffer);
        }
        else
        {
            ProcessVerticesInParallel(model, buffer, faceCount);
        }
        return buffer;
    }

    private void ProcessVerticesSequentially(ObjModel model, VertexData[] buffer)
    {
        int bufferIndex = 0;
        int faceCount = model.Faces.Count;
        for (int i = 0; i < faceCount; i++)
        {
            bufferIndex = ProcessFaceVerticesSequentially(model.Faces[i], model, buffer, bufferIndex);
        }
    }

    private int ProcessFaceVerticesSequentially(Face face, ObjModel model, VertexData[] buffer, int bufferIndex)
    {
        int vertexCount = face.ActualCount;
        for (int i = 0; i < vertexCount; i++)
        {
            FaceIndices fi = face[i];
            
            buffer[bufferIndex] = TransformSingle(fi, model);
            
            bufferIndex++;
        }
        return bufferIndex;
    }

    private void ProcessVerticesInParallel(ObjModel model, VertexData[] buffer, int faceCount)
    {
        int[] offsets = CalculateVertexOffsets(model, faceCount);
        ParallelProcessFaceVertices(model, buffer, offsets, faceCount);
    }

    private static int[] CalculateVertexOffsets(ObjModel model, int faceCount)
    {
        int[] offsets = new int[faceCount];
        int currentOffset = 0;
        for (int i = 0; i < faceCount; i++)
        {
            offsets[i] = currentOffset;
            currentOffset += model.Faces[i].ActualCount;
        }
        return offsets;
    }

    private void ParallelProcessFaceVertices(ObjModel model, VertexData[] buffer, int[] offsets, int faceCount)
    {
        Parallel.For(0, faceCount, i =>
        {
            ProcessSingleFaceInParallel(model, buffer, offsets, i);
        });
    }

    private void ProcessSingleFaceInParallel(ObjModel model, VertexData[] buffer, int[] offsets, int faceIndex)
    {
        Face face = model.Faces[faceIndex];
        int bufferStart = offsets[faceIndex];
        int vertexCount = face.ActualCount;
        for (int j = 0; j < vertexCount; j++)
        {
            FaceIndices fi = face[j];
            buffer[bufferStart + j] = TransformSingle(fi, model);
        }
    }

    private VertexData TransformSingle(FaceIndices fi, ObjModel model)
    {
        Vector3 normal = GetNormal(model.Normals, fi.NormalIndex);
        
        Vector4 vertex = GetVertex(model.Vertices, fi.VertexIndex);
        
        Vector2 texCoord = GetTextureCoordinate(model.TextureCoords, fi.TextureIndex);
        
        return _transformCalculator.Transform(vertex, normal, texCoord);
    }
    
    private static Vector3 GetNormal(IReadOnlyList<Vector3> normals, int index)
    {
        if (index < 0 || index >= normals.Count)
            return Vector3.UnitZ;
        return normals[index];
    }

    private static Vector4 GetVertex(IReadOnlyList<Vector4> vertices, int index)
    {
        if (index < 0 || index >= vertices.Count)
            return Vector4.UnitW;
        return vertices[index];
    }

    private static Vector2 GetTextureCoordinate(IReadOnlyList<Vector2> texCoords, int index)
    {
        if (index < 0 || index >= texCoords.Count)
            return Vector2.Zero;
        return texCoords[index];
    }
}