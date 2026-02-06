using System.Runtime.CompilerServices;
using System.Windows;
using AKG.Core.Model;
using AKG.Render.Constants;

namespace AKG.Render.Renderers;

public sealed class FaceRenderer
{
    private readonly BitmapRenderer _bitmapRenderer;

    public FaceRenderer(BitmapRenderer bitmapRenderer)
    {
        _bitmapRenderer = bitmapRenderer;
    }

    public void RenderFaces(IReadOnlyList<FaceIndices[]> faces, Point[] screenPoints, int screenPointsCount)
    {
        if (faces.Count > RenderConstants.LineDrawingThreshold)
        {
            RenderFacesInParallel(faces, screenPoints, screenPointsCount);
        }
        else
        {
            RenderFacesSequentially(faces, screenPoints, screenPointsCount);
        }
    }

    private void RenderFacesInParallel(IReadOnlyList<FaceIndices[]> faces, Point[] screenPoints, int screenPointsCount)
    {
        Parallel.For(0, faces.Count, index =>
        {
            RenderSingleFace(faces[index], screenPoints, screenPointsCount);
        });
    }

    private void RenderFacesSequentially(IReadOnlyList<FaceIndices[]> faces, Point[] screenPoints, int screenPointsCount)
    {
        foreach (FaceIndices[] face in faces)
        {
            RenderSingleFace(face, screenPoints, screenPointsCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RenderSingleFace(FaceIndices[] face, Point[] screenPoints, int screenPointsCount)
    {
        if (face.Length >= 2)
        {
            for (int vertexIndex = 0; vertexIndex < face.Length; vertexIndex++)
            {
                int nextVertexIndex = (vertexIndex + 1) % face.Length;
                DrawFaceEdge(face[vertexIndex], face[nextVertexIndex], screenPoints, screenPointsCount);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawFaceEdge(FaceIndices startVertex, FaceIndices endVertex, Point[] screenPoints, int screenPointsCount)
    {
        if (AreVertexIndicesValid(startVertex.VertexIndex, endVertex.VertexIndex, screenPointsCount))
        {
            Point startPoint = screenPoints[startVertex.VertexIndex];
            
            Point endPoint = screenPoints[endVertex.VertexIndex];

            if (IsPointInvalid(startPoint) || IsPointInvalid(endPoint))
            {
                return;
            }

            _bitmapRenderer.DrawLine(startPoint, endPoint);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool AreVertexIndicesValid(int index1, int index2, int screenPointsLength)
    {
        return index1 >= 0 && index1 < screenPointsLength &&
               index2 >= 0 && index2 < screenPointsLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPointInvalid(Point point)
    {
        return point.X < 0 || point.Y < 0;
    }
}