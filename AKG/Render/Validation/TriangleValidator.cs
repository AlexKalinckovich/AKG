using System.Windows;
using AKG.Model;

namespace AKG.Render.Validation;


public static class TriangleValidator
{
    public static bool AreVertexIndicesValid(int index0, int index1, int index2, int vertexCount)
    {
        return IsValidIndex(index0, vertexCount) &&
               IsValidIndex(index1, vertexCount) &&
               IsValidIndex(index2, vertexCount);
    }

    public static bool IsTriangleValid(Triangle triangle)
    {
        return !IsPointInvalid(triangle.Vertex0.ScreenPoint) &&
               !IsPointInvalid(triangle.Vertex1.ScreenPoint) &&
               !IsPointInvalid(triangle.Vertex2.ScreenPoint);
    }

    private static bool IsValidIndex(int index, int maxCount)
    {
        return index >= 0 && index < maxCount;
    }

    private static bool IsPointInvalid(Point point)
    {
        return point.X < 0 || point.Y < 0;
    }
}