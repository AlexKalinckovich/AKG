namespace AKG.Core.Model;

public readonly struct FaceIndices
{
    public readonly int VertexIndex;  
    public readonly int TextureIndex; 
    public readonly int NormalIndex; 

    
    public FaceIndices(int vertexIndex, int textureIndex, int normalIndex)
    {
        VertexIndex = vertexIndex;
        TextureIndex = textureIndex;
        NormalIndex = normalIndex;
    }
        
    
    public bool IsEmpty => VertexIndex == 0 && TextureIndex == 0 && NormalIndex == 0;
}