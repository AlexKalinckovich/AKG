using AKG.Model.Vertex;

namespace AKG.Model;

public readonly struct Triangle
{
    public readonly VertexData Vertex0;
    public readonly VertexData Vertex1;
    public readonly VertexData Vertex2;

    public Triangle(VertexData vertex0, VertexData vertex1, VertexData vertex2)
    {
        Vertex0 = vertex0;
        Vertex1 = vertex1;
        Vertex2 = vertex2;
    }
}