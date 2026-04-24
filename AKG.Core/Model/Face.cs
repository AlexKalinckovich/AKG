using AKG.Core.Model;

namespace AKG.Model;

public readonly struct Face
{
    public FaceIndices First { get; init; }
    public FaceIndices Second { get; init; }
    public FaceIndices Third { get; init; }

    public FaceIndices Fourth { get; init; }
    
    public FaceIndices Fifth { get; init; }
    
    public int ActualCount { get; init; }
    
    public Face(FaceIndices first, FaceIndices second, FaceIndices third)
    {
        First = first;
        Second = second;
        Third = third;
        ActualCount = 3;
    }

    public Face(FaceIndices first, FaceIndices second, FaceIndices third, FaceIndices fourth)
    {
        First = first;
        Second = second;
        Third = third;
        Fourth = fourth;
        ActualCount = 4;
    }

    public Face(FaceIndices first, FaceIndices second, FaceIndices third, FaceIndices fourth, FaceIndices fifth)
    {
        First = first;
        Second = second;
        Third = third;
        Fourth = fourth;
        Fifth = fifth;
        ActualCount = 5;
    }
    
    public FaceIndices this[int index]
    {
        get
        {
            if (index < 0 || index >= ActualCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range for face with {ActualCount} vertices");
            }

            return index switch
            {
                0 => First,
                1 => Second,
                2 => Third,
                3 => Fourth,
                4 => Fifth,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }
    }
}