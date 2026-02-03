namespace ACG.Model;

public struct FaceIndices
{
    public int VertexIndex;  
    public int TextureIndex; 
    public int NormalIndex; 

    
    public FaceIndices(int vertexIndex, int textureIndex, int normalIndex)
    {
        VertexIndex = vertexIndex;
        TextureIndex = textureIndex;
        NormalIndex = normalIndex;
    }
        
    
    public bool IsEmpty => VertexIndex == 0 && TextureIndex == 0 && NormalIndex == 0;
}