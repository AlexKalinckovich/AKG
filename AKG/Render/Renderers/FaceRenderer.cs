using System.Numerics;
using AKG.Core.Model;
using AKG.Model;
using AKG.Render.Constants;
using AKG.Render.Culling;
using AKG.Render.Drawing;
using AKG.Render.Lighting;
using AKG.Render.Rasterization;
using AKG.Render.Validation;

namespace AKG.Render.Renderers;

public sealed class FaceRenderer : IDisposable
{
    private readonly FaceCullingStrategy _faceCullingStrategy;
    private readonly FrustumCuller _frustumCuller;
    private readonly TriangleValidator _triangleValidator;
    private readonly TriangleRasterizer _triangleRasterizer;
    private readonly ZBufferManager _zBufferManager;

    public FaceRenderer(
        BitmapRenderer bitmapRenderer, 
        int width, 
        int height)
    {
        _zBufferManager = new ZBufferManager(width, height);
        
        LightingCalculator lightingCalculator = LightCalculatorBuilder.CreateLightingCalculator();
        
        BarycentricCalculator barycentricCalculator = new BarycentricCalculator();
        
        PixelDrawer pixelDrawer = new PixelDrawer(bitmapRenderer, width, height);
        
        _triangleRasterizer = new TriangleRasterizer(
            width, height, _zBufferManager, barycentricCalculator, 
            lightingCalculator, pixelDrawer);
        
        _faceCullingStrategy = new FaceCullingStrategy();
        _frustumCuller = new FrustumCuller();
        _triangleValidator = new TriangleValidator();
    }

    
    public void RenderFaces(
        IReadOnlyList<FaceIndices[]> faces, 
        VertexData[] vertices, 
        int verticesCount,
        Vector3 cameraPosition)
    {
        _zBufferManager.Clear();

        if (faces.Count > RenderConstants.LineDrawingThreshold)
        {
            RenderFacesInParallel(faces, vertices, verticesCount, cameraPosition);
        }
        else
        {
            RenderFacesSequentially(faces, vertices, verticesCount, cameraPosition);
        }
    }

    private void RenderFacesInParallel(
        IReadOnlyList<FaceIndices[]> faces, 
        VertexData[] vertices, 
        int verticesCount,
        Vector3 cameraPosition)
    {
        Parallel.For(0, faces.Count, index =>
        {
            RenderSingleFace(faces[index], vertices, verticesCount, cameraPosition);
        });
    }

    private void RenderFacesSequentially(
        IReadOnlyList<FaceIndices[]> faces, 
        VertexData[] vertices, 
        int verticesCount,
        Vector3 cameraPosition)
    {
        foreach (FaceIndices[] face in faces)
        {
            RenderSingleFace(face, vertices, verticesCount, cameraPosition);
        }
    }

    private void RenderSingleFace(
        FaceIndices[] face, 
        VertexData[] vertices, 
        int verticesCount,
        Vector3 cameraPosition)
    {
        if (face.Length > 2)
        {
            RenderTriangle(face, vertices, verticesCount, cameraPosition);
        }
    }

    private void RenderTriangle(FaceIndices[] face, VertexData[] vertices, int verticesCount, Vector3 cameraPosition)
    {
        for (int i = 0; i < face.Length - 2; i++)
        {
            int index0 = face[0].VertexIndex;
            int index1 = face[i + 1].VertexIndex;
            int index2 = face[i + 2].VertexIndex;

            if (_triangleValidator.AreVertexIndicesValid(index0, index1, index2, verticesCount))
            {
                ProcessTriangle(vertex0: vertices[index0],
                                vertex1: vertices[index1], 
                                vertex2: vertices[index2], 
                                cameraPosition);
            }
        }
    }

    private void ProcessTriangle(VertexData vertex0, VertexData vertex1, VertexData vertex2, Vector3 cameraPosition)
    {
        if (!_triangleValidator.IsTriangleValid(vertex0, vertex1, vertex2))
        {
            return;
        }
        
        if (!_frustumCuller.IsTriangleInFrustum(vertex0.ClipPosition, vertex1.ClipPosition, vertex2.ClipPosition))
        {
            return;
        }

        if (!_faceCullingStrategy.IsVisible(vertex0.ViewPosition, vertex1.ViewPosition, vertex2.ViewPosition))
        {
            return;
        }

        _triangleRasterizer.Rasterize(vertex0, vertex1, vertex2, cameraPosition);
    }

    public void Dispose()
    {
        _zBufferManager.Dispose();
    }
}
/*
         * Вершины в пространстве экрана по трем вершинам найти два ребра (B - A) (C - A) (определитель, перпендикулярное скалярное произведение) ax * by - by *ax
         */