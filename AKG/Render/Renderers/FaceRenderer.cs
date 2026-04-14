using System.Numerics;
using AKG.Core.Model;
using AKG.Model;
using AKG.Render.Constants;
using AKG.Render.Culling;
using AKG.Render.Rasterization;
using AKG.Render.Texture;
using AKG.Render.Validation;

namespace AKG.Render.Renderers;

public sealed class FaceRenderer : IDisposable
{
    private readonly TriangleRasterizer _triangleRasterizer;

    public FaceRenderer(BitmapRenderer bitmapRenderer)
    {
        RenderTextureMaps renderMaps = TextureMapLoader.LoadDefaultMaps();
        
        _triangleRasterizer = new TriangleRasterizer(
            bitmapRenderer,
            renderMaps);
        
    }

    
    public void RenderFaces(
        IReadOnlyList<FaceIndices[]> faces, 
        VertexData[] vertices, 
        int verticesCount,
        Vector3 cameraPosition)
    {
        _triangleRasterizer.ClearZBuffer();

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

            if (TriangleValidator.AreVertexIndicesValid(index0, index1, index2, verticesCount))
            {
                Triangle triangle = new Triangle(vertices[index0], vertices[index1], vertices[index2]);
                ProcessTriangle(triangle, cameraPosition);
            }
        }
    }

    private void ProcessTriangle(Triangle triangle, Vector3 cameraPosition)
    {
        if (!TriangleValidator.IsTriangleValid(triangle))
        {
            return;
        }
        
        if (!FrustumCuller.IsTriangleInFrustum(triangle))
        {
            return;
        }

        if (!FaceCullingStrategy.IsTriangleVisible(triangle))
        {
            return;
        }

        _triangleRasterizer.Rasterize(triangle, cameraPosition);
    }

    public void Dispose()
    {
        _triangleRasterizer.ClearZBuffer();
    }
}
/*
         * Вершины в пространстве экрана по трем вершинам найти два ребра (B - A) (C - A) (определитель, перпендикулярное скалярное произведение) ax * by - by *ax
         */