using System.Runtime.CompilerServices;
using System.Windows;
using AKG.Core.Model;
using AKG.Render.Constants;
using AKG.Render.Painting;
using System.Numerics;
using AKG.Model;
using AKG.Render.States;

namespace AKG.Render.Renderers;

public sealed class FaceRenderer
{
    private readonly BitmapRenderer _bitmapRenderer;
    private readonly double[,] _zBuffer;
    private readonly int _width;
    private readonly int _height;
    private readonly BackFaceCulling _backFaceCulling;
    private readonly Vector3 _lightDirection;
    
    private Vector3 _targetPosition = Vector3.Zero;
    
    public FaceRenderer(BitmapRenderer bitmapRenderer, int width, int height)
    {
        _bitmapRenderer = bitmapRenderer;
        _width = width;
        _height = height;
        _zBuffer = new double[width, height];
        _backFaceCulling = new BackFaceCulling();
        _lightDirection = new Vector3(0, 0, 1);
        
        ClearZBuffer();
    }

    public void RenderFaces(IReadOnlyList<FaceIndices[]> faces, VertexData[] vertices, int verticesCount,
        Vector3 cameraStateTargetPosition)
    {
        ClearZBuffer();
     
        _targetPosition = cameraStateTargetPosition;
        if (faces.Count > RenderConstants.LineDrawingThreshold)
        {
            RenderFacesInParallel(faces, vertices, verticesCount);
        }
        else
        {
            RenderFacesSequentially(faces, vertices, verticesCount);
        }
    }

    private void RenderFacesInParallel(IReadOnlyList<FaceIndices[]> faces, VertexData[] vertices, int verticesCount)
    {
        Parallel.For(0, faces.Count, index =>
        {
            RenderSingleFace(faces[index], vertices, verticesCount);
        });
    }

    private void RenderFacesSequentially(IReadOnlyList<FaceIndices[]> faces, VertexData[] vertices, int verticesCount)
    {
        foreach (FaceIndices[] face in faces)
        {
            RenderSingleFace(face, vertices, verticesCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RenderSingleFace(FaceIndices[] face, VertexData[] vertices, int verticesCount)
    {
        if (face.Length < 3)
            return;

        for (int i = 0; i < face.Length - 2; i++)
        {
            int idx0 = face[0].VertexIndex;
            int idx1 = face[i + 1].VertexIndex;
            int idx2 = face[i + 2].VertexIndex;

            if (AreVertexIndicesValid(idx0, idx1, idx2, verticesCount))
            {
                ProcessTriangle(vertices[idx0], vertices[idx1], vertices[idx2]);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool AreVertexIndicesValid(int idx0, int idx1, int idx2, int verticesCount)
    {
        return idx0 >= 0 && idx0 < verticesCount &&
               idx1 >= 0 && idx1 < verticesCount &&
               idx2 >= 0 && idx2 < verticesCount;
    }

    private void ProcessTriangle(VertexData v0, VertexData v1, VertexData v2)
    {
        if (!IsTriangleValid(v0, v1, v2))
            return;

        if (!IsTriangleInFrustum(v0.ClipPosition, v1.ClipPosition, v2.ClipPosition))
        {
            return;
        }
        
        if (!_backFaceCulling.IsVisible(v0.ViewPosition, v1.ViewPosition, v2.ViewPosition))
            return;

        float intensity = CalculateFlatShading(v0.WorldPosition, v1.WorldPosition, v2.WorldPosition);
        
        uint color = CalculateColor(intensity);
        
        RasterizeTriangle(v0, v1, v2, color);
    }
    
    private bool IsTriangleInFrustum(Vector4 clip0, Vector4 clip1, Vector4 clip2)
    {
        if (clip0.X < -clip0.W && clip1.X < -clip1.W && clip2.X < -clip2.W) return false;
        if (clip0.X >  clip0.W && clip1.X >  clip1.W && clip2.X >  clip2.W) return false;
        if (clip0.Y < -clip0.W && clip1.Y < -clip1.W && clip2.Y < -clip2.W) return false;
        if (clip0.Y >  clip0.W && clip1.Y >  clip1.W && clip2.Y >  clip2.W) return false;
        if (clip0.Z < 0       && clip1.Z < 0       && clip2.Z < 0)          return false;
        if (clip0.Z > clip0.W && clip1.Z > clip1.W && clip2.Z > clip2.W)    return false;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsTriangleValid(VertexData v0, VertexData v1, VertexData v2)
    {
        return !IsPointInvalid(v0.ScreenPoint) &&
               !IsPointInvalid(v1.ScreenPoint) &&
               !IsPointInvalid(v2.ScreenPoint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float CalculateFlatShading(Vector3 worldA, Vector3 worldB, Vector3 worldC)
    {
        Vector3 u = worldB - worldA;
        Vector3 v = worldC - worldA;
        Vector3 normal = Vector3.Cross(u, v);
        
        if (normal.LengthSquared() == 0)
            return 0.2f;
            
        normal = Vector3.Normalize(normal);
        
        float intensity = Vector3.Dot(normal, _lightDirection);
        
        return Math.Max(0.2f, intensity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint CalculateColor(float intensity)
    {
        byte gray = (byte)(255 * intensity);
        return (uint)((255 << 24) | (gray << 16) | (gray << 8) | gray);
    }

    private void RasterizeTriangle(VertexData v0, VertexData v1, VertexData v2, uint color)
    {
        Point p0 = v0.ScreenPoint;
        Point p1 = v1.ScreenPoint;
        Point p2 = v2.ScreenPoint;

        float z0 = v0.Depth;
        float z1 = v1.Depth;
        float z2 = v2.Depth;

        int minX = (int)Math.Max(0, Math.Floor(Math.Min(p0.X, Math.Min(p1.X, p2.X))));
        int maxX = (int)Math.Min(_width - 1, Math.Ceiling(Math.Max(p0.X, Math.Max(p1.X, p2.X))));
        
        int minY = (int)Math.Max(0, Math.Floor(Math.Min(p0.Y, Math.Min(p1.Y, p2.Y))));
        int maxY = (int)Math.Min(_height - 1, Math.Ceiling(Math.Max(p0.Y, Math.Max(p1.Y, p2.Y))));

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (IsPointInTriangle(x, y, p0, p1, p2))
                {
                    float z = InterpolateDepth(x, y, p0, p1, p2, z0, z1, z2);
                    
                    if (z < _zBuffer[x, y])
                    {
                        _zBuffer[x, y] = z;
                        DrawPixel(x, y, color);
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsPointInTriangle(int x, int y, Point p0, Point p1, Point p2)
    {
        float s1 = (float)((p1.X - p0.X) * (y - p0.Y) - (p1.Y - p0.Y) * (x - p0.X));
        float s2 = (float)((p2.X - p1.X) * (y - p1.Y) - (p2.Y - p1.Y) * (x - p1.X));
        float s3 = (float)((p0.X - p2.X) * (y - p2.Y) - (p0.Y - p2.Y) * (x - p2.X));
        
        return (s1 >= 0 && s2 >= 0 && s3 >= 0) || (s1 <= 0 && s2 <= 0 && s3 <= 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float InterpolateDepth(int x, int y, Point p0, Point p1, Point p2, 
                                   float z0, float z1, float z2)
    {
        float denom = (float)((p1.Y - p2.Y) * (p0.X - p2.X) + (p2.X - p1.X) * (p0.Y - p2.Y));
        if (Math.Abs(denom) < 1e-10f)
            return z0;
            
        float w0 = (float)(((p1.Y - p2.Y) * (x - p2.X) + (p2.X - p1.X) * (y - p2.Y)) / denom);
        float w1 = (float)(((p2.Y - p0.Y) * (x - p2.X) + (p0.X - p2.X) * (y - p2.Y)) / denom);
        float w2 = 1 - w0 - w1;
        
        return w0 * z0 + w1 * z1 + w2 * z2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawPixel(int x, int y, uint color)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            _bitmapRenderer.SetPixel(x, y, color);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPointInvalid(Point point)
    {
        return point.X < 0 || point.Y < 0;
    }

    public void ClearZBuffer()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _zBuffer[x, y] = double.MaxValue;
            }
        }
    }
}
