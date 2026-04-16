using AKG.Core.Model;

namespace AKG.Model;

public readonly struct Face
{
    public FaceIndices First { get; init; }
    public FaceIndices Second { get; init; }
    public FaceIndices Third { get; init; }

    public Face(FaceIndices first, FaceIndices second, FaceIndices third)
    {
        First = first;
        Second = second;
        Third = third;
    }
}