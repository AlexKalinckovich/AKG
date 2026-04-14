using System.Numerics;
using AKG.Model;

namespace AKG.Render.Culling;


public static class FrustumCuller
{
    public static bool IsTriangleInFrustum(Triangle triangle)
    {
        Vector4 clip0 = triangle.Vertex0.ClipPosition;
        Vector4 clip1 = triangle.Vertex1.ClipPosition;
        Vector4 clip2 = triangle.Vertex2.ClipPosition;
        
        if (AreAllVerticesOutsideLeft(clip0, clip1, clip2)) return false;
        
        if (AreAllVerticesOutsideRight(clip0, clip1, clip2)) return false;
        
        if (AreAllVerticesOutsideBottom(clip0, clip1, clip2)) return false;
        
        if (AreAllVerticesOutsideTop(clip0, clip1, clip2)) return false;
        
        if (AreAllVerticesOutsideNear(clip0, clip1, clip2)) return false;
        
        if (AreAllVerticesOutsideFar(clip0, clip1, clip2)) return false;
        
        return true;
    }

    private static bool AreAllVerticesOutsideLeft(Vector4 v0, Vector4 v1, Vector4 v2)
    {
        return v0.X < -v0.W && v1.X < -v1.W && v2.X < -v2.W;
    }

    private static bool AreAllVerticesOutsideRight(Vector4 v0, Vector4 v1, Vector4 v2)
    {
        return v0.X > v0.W && v1.X > v1.W && v2.X > v2.W;
    }

    private static bool AreAllVerticesOutsideBottom(Vector4 v0, Vector4 v1, Vector4 v2)
    {
        return v0.Y < -v0.W && v1.Y < -v1.W && v2.Y < -v2.W;
    }

    private static bool AreAllVerticesOutsideTop(Vector4 v0, Vector4 v1, Vector4 v2)
    {
        return v0.Y > v0.W && v1.Y > v1.W && v2.Y > v2.W;
    }

    private static bool AreAllVerticesOutsideNear(Vector4 v0, Vector4 v1, Vector4 v2)
    {
        return v0.Z < 0 && v1.Z < 0 && v2.Z < 0;
    }

    private static bool AreAllVerticesOutsideFar(Vector4 v0, Vector4 v1, Vector4 v2)
    {
        return v0.Z > v0.W && v1.Z > v1.W && v2.Z > v2.W;
    }
}