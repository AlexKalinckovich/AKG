using AKG.Core.Model;
using AKG.Core.Parser.Numeric;

namespace AKG.Core.Parser.ObjPartParsers.DataParsers;

public class FaceIndicesStringParser
{
    public FaceIndices ParseVertexIndicesFromString(string vertexDataString)
    {
        string[] subParts = vertexDataString.Split('/');

        int vertexIndex = ParseVertexIndexFromSubParts(subParts);
        
        int textureIndex = ParseTextureIndexFromSubParts(subParts);
        
        int normalIndex = ParseNormalIndexFromSubParts(subParts);

        FaceIndices faceIndices = new(vertexIndex, textureIndex, normalIndex);
        
        return faceIndices;
    }

    private static int ParseVertexIndexFromSubParts(string[] subParts)
    {
        int vertexIndex = 0;
        if (HasVertexSubPart(subParts))
        {
             IntegerStringParser.ParseAndAdjustIndexFromString(subParts[0], out vertexIndex);
        }
        
        return vertexIndex;
    }

    private static int ParseTextureIndexFromSubParts(string[] subParts)
    {
        int textureIndex = 0;
        if (HasTextureSubPart(subParts))
        {
             IntegerStringParser.ParseAndAdjustIndexFromString(subParts[1], out textureIndex);
        }
        
        return textureIndex;
    }

    private static int ParseNormalIndexFromSubParts(string[] subParts)
    {
        int normalIndex = 0;
        if (HasNormalSubPart(subParts))
        {
            IntegerStringParser.ParseAndAdjustIndexFromString(subParts[2], out normalIndex);
        }
        
        return normalIndex;
    }

    private static bool HasVertexSubPart(string[] subParts)
    {
        return subParts.Length > 0 && !string.IsNullOrEmpty(subParts[0]);
    }

    private static bool HasTextureSubPart(string[] subParts)
    {
        return subParts.Length > 1 && !string.IsNullOrEmpty(subParts[1]);
    }

    private static bool HasNormalSubPart(string[] subParts)
    {
        return subParts.Length > 2 && !string.IsNullOrEmpty(subParts[2]);
    }
}