using System.Windows;
using AKG.Model;

namespace AKG.Render.Validation;


public sealed class TriangleValidator
{
    public bool AreVertexIndicesValid(int index0, int index1, int index2, int vertexCount)
    {
        return IsValidIndex(index0, vertexCount) &&
               IsValidIndex(index1, vertexCount) &&
               IsValidIndex(index2, vertexCount);
    }

    public bool IsTriangleValid(VertexData vertex0, VertexData vertex1, VertexData vertex2)
    {
        return !IsPointInvalid(vertex0.ScreenPoint) &&
               !IsPointInvalid(vertex1.ScreenPoint) &&
               !IsPointInvalid(vertex2.ScreenPoint);
    }

    private bool IsValidIndex(int index, int maxCount)
    {
        return index >= 0 && index < maxCount;
    }

    private bool IsPointInvalid(Point point)
    {
        return point.X < 0 || point.Y < 0;
    }
}