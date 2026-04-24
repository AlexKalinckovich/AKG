using System.Collections.Concurrent;
using AKG.Core.Model;
using AKG.Model;
using AKG.Model.Vertex;
using AKG.Render.Constants;
using AKG.Render.Culling;
using AKG.Render.Rasterization;
using AKG.Render.States;
using AKG.Render.Texture;
using AKG.Render.Validation;
using System;

namespace AKG.Render.Renderers;

public sealed class FaceRenderer : IDisposable
{
    private readonly TriangleRasterizer _triangleRasterizer;

    public FaceRenderer(BitmapRenderer bitmapRenderer, CameraState cameraState)
    {
        RenderTextureMaps renderMaps = TextureMapLoader.LoadDefaultMaps();
        _triangleRasterizer = new TriangleRasterizer(bitmapRenderer, cameraState, renderMaps);
    }

    public void RenderFaces(IReadOnlyList<Face> faces, ReadOnlyMemory<VertexData> vertices)
    {
        _triangleRasterizer.ClearZBuffer();
        int faceCount = faces.Count;
        if (faceCount > RenderConstants.LineDrawingThreshold)
        {
            RenderFacesInParallel(faces, vertices, faceCount);
        }
        else
        {
            RenderFacesSequentially(faces, vertices.Span);
        }
    }

    private void RenderFacesInParallel(IReadOnlyList<Face> faces, ReadOnlyMemory<VertexData> vertices, int faceCount)
    {
        int[] offsets = CalculateFaceOffsets(faces, faceCount);
        ExecuteParallelRendering(faces, vertices, offsets, faceCount);
    }

    private static int[] CalculateFaceOffsets(IReadOnlyList<Face> faces, int faceCount)
    {
        int[] offsets = new int[faceCount];
        int currentOffset = 0;
        for (int i = 0; i < faceCount; i++)
        {
            offsets[i] = currentOffset;
            currentOffset += faces[i].ActualCount;
        }
        return offsets;
    }

    private void ExecuteParallelRendering(
        IReadOnlyList<Face> faces, 
        ReadOnlyMemory<VertexData> vertices,  
        int[] offsets, 
        int faceCount)
    {
        var partitioner = Partitioner.Create(0, faceCount, RenderConstants.PartitionBatchSize);
        Parallel.ForEach(partitioner, range =>
        {
            ReadOnlySpan<VertexData> span = vertices.Span;
            RenderFaceRangeSequentially(faces, span, offsets, range.Item1, range.Item2);
        });
    }

    private void RenderFaceRangeSequentially(IReadOnlyList<Face> faces, ReadOnlySpan<VertexData> vertices, int[] offsets, int startIndex, int endIndex)
    {
        for (int i = startIndex; i < endIndex; i++)
        {
            RenderSingleFace(faces[i], vertices, offsets[i]);
        }
    }

    private void RenderFacesSequentially(IReadOnlyList<Face> faces, ReadOnlySpan<VertexData> vertices)
    {
        int vertexOffset = 0;
        int faceCount = faces.Count;
        for (int i = 0; i < faceCount; i++)
        {
            RenderSingleFace(faces[i], vertices, vertexOffset);
            vertexOffset += faces[i].ActualCount;
        }
    }

    private void RenderSingleFace(Face face, ReadOnlySpan<VertexData> vertices, int vertexOffset)
    {
        int vertexCount = face.ActualCount;
        if (vertexCount < 3)
            return;
        RenderTriangulatedFace(vertices, vertexOffset, vertexCount);
    }

    private void RenderTriangulatedFace(ReadOnlySpan<VertexData> vertices, int vertexOffset, int vertexCount)
    {
        for (int i = 0; i < vertexCount - 2; i++)
        {
            Triangle triangle = CreateTriangleFromFan(vertices, vertexOffset, i);
            ProcessTriangle(triangle);
        }
    }

    private static Triangle CreateTriangleFromFan(ReadOnlySpan<VertexData> vertices, int vertexOffset, int index)
    {
        int idx0 = vertexOffset;
        int idx1 = vertexOffset + index + 1;
        int idx2 = vertexOffset + index + 2;
        return new Triangle(vertices[idx0], vertices[idx1], vertices[idx2]);
    }

    private void ProcessTriangle(Triangle triangle)
    {
        if (!TriangleValidator.IsTriangleValid(triangle))
            return;
        ProcessFrustumCulling(triangle);
    }

    private void ProcessFrustumCulling(Triangle triangle)
    {
        if (!FrustumCuller.IsTriangleInFrustum(triangle))
            return;
        ProcessFaceCulling(triangle);
    }

    private void ProcessFaceCulling(Triangle triangle)
    {
        if (!FaceCullingStrategy.IsTriangleVisible(triangle))
            return;
        RasterizeTriangle(triangle);
    }

    private void RasterizeTriangle(Triangle triangle)
    {
        _triangleRasterizer.Rasterize(triangle);
    }

    public void Dispose()
    {
        _triangleRasterizer.ClearZBuffer();
    }
}