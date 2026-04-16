using AKG.Core.Model;
using AKG.Model;
using AKG.Model.Vertex;
using AKG.Render.Constants;
using AKG.Render.Culling;
using AKG.Render.Rasterization;
using AKG.Render.States;
using AKG.Render.Texture;
using AKG.Render.Validation;

namespace AKG.Render.Renderers;

public sealed class FaceRenderer : IDisposable
{
    private readonly TriangleRasterizer _triangleRasterizer;
    
    public FaceRenderer(BitmapRenderer bitmapRenderer, CameraState cameraState)
    {
        RenderTextureMaps renderMaps = TextureMapLoader.LoadDefaultMaps();
        
        _triangleRasterizer = new TriangleRasterizer(bitmapRenderer, cameraState, renderMaps);
    }

    
    public void RenderFaces(IReadOnlyList<FaceIndices[]> faces, ReadOnlyMemory<VertexData> vertices)
    {
        _triangleRasterizer.ClearZBuffer();

        if (faces.Count > RenderConstants.LineDrawingThreshold)
        {
            RenderFacesInParallel(faces, vertices);
        }
        else
        {
            RenderFacesSequentially(faces, vertices.Span);
        }
    }

    private void RenderFacesInParallel(
        IReadOnlyList<FaceIndices[]> faces, 
        ReadOnlyMemory<VertexData> vertices)
    {
        Parallel.For(0, faces.Count, body: (int index) =>
        {
            RenderSingleFace(faces[index], vertices.Span);
        });
    }

    private void RenderFacesSequentially(
        IReadOnlyList<FaceIndices[]> faces, 
        ReadOnlySpan<VertexData> vertices)
    {
        foreach (FaceIndices[] face in faces)
        {
            RenderSingleFace(face, vertices);
        }
    }

    private void RenderSingleFace(
        FaceIndices[] face, 
        ReadOnlySpan<VertexData> vertices)
    {
        if (face.Length > 2)
        {
            RenderTriangle(face, vertices);
        }
    }

    private void RenderTriangle(FaceIndices[] face, ReadOnlySpan<VertexData> vertices)
    {
        for (int i = 0; i < face.Length - 2; i++)
        {
            int index0 = face[0].VertexIndex;
            int index1 = face[i + 1].VertexIndex;
            int index2 = face[i + 2].VertexIndex;

            if (TriangleValidator.AreVertexIndicesValid(index0, index1, index2, vertices.Length))
            {
                Triangle triangle = new Triangle(vertices[index0], vertices[index1], vertices[index2]);
                ProcessTriangle(triangle);
            }
        }
    }

    private void ProcessTriangle(Triangle triangle)
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

        _triangleRasterizer.Rasterize(triangle);
    }

    public void Dispose()
    {
        _triangleRasterizer.ClearZBuffer();
    }
}
/*
         * Вершины в пространстве экрана по трем вершинам найти два ребра (B - A) (C - A) (определитель, перпендикулярное скалярное произведение) ax * by - by *ax
         */