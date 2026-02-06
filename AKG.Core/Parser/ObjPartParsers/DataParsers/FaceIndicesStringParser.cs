using AKG.Core.Model;
using AKG.Core.Parser.Numeric;

namespace AKG.Core.Parser.ObjPartParsers.DataParsers;

public class FaceIndicesStringParser
{
    private readonly IntegerStringParser _integerStringParser = new();

    public FaceIndices ParseVertexIndicesFromString(string vertexDataString)
    {
        string[] subParts = vertexDataString.Split('/');

        int vertexIndex = ParseVertexIndexFromSubParts(subParts);
        
        int textureIndex = ParseTextureIndexFromSubParts(subParts);
        
        int normalIndex = ParseNormalIndexFromSubParts(subParts);

        FaceIndices faceIndices = new(vertexIndex, textureIndex, normalIndex);
        
        return faceIndices;
    }

    private int ParseVertexIndexFromSubParts(string[] subParts)
    {
        int vertexIndex = 0;
        if (HasVertexSubPart(subParts))
        {
             _integerStringParser.ParseAndAdjustIndexFromString(subParts[0], out vertexIndex);
        }
        
        return vertexIndex;
    }

    private int ParseTextureIndexFromSubParts(string[] subParts)
    {
        int textureIndex = 0;
        if (HasTextureSubPart(subParts))
        {
             _integerStringParser.ParseAndAdjustIndexFromString(subParts[1], out textureIndex);
        }
        
        return textureIndex;
    }

    private int ParseNormalIndexFromSubParts(string[] subParts)
    {
        int normalIndex = 0;
        if (HasNormalSubPart(subParts))
        {
            _integerStringParser.ParseAndAdjustIndexFromString(subParts[2], out normalIndex);
        }
        
        return normalIndex;
    }

    private bool HasVertexSubPart(string[] subParts)
    {
        return subParts.Length > 0 && !string.IsNullOrEmpty(subParts[0]);
    }

    private bool HasTextureSubPart(string[] subParts)
    {
        return subParts.Length > 1 && !string.IsNullOrEmpty(subParts[1]);
    }

    private bool HasNormalSubPart(string[] subParts)
    {
        return subParts.Length > 2 && !string.IsNullOrEmpty(subParts[2]);
    }
}