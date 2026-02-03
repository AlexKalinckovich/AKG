using AKG.Core.Model;

namespace AKG.Core.Parser;

using System.Globalization;


public class FaceParser
{
    public FaceIndices[] ParseFaceLine(string faceLine)
    {
        if (string.IsNullOrWhiteSpace(faceLine))
            return [];

        var parts = faceLine.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        var faceIndices = new List<FaceIndices>();

        foreach (var part in parts)
        {
            var indices = ParseVertexIndices(part);
            faceIndices.Add(indices);
        }

        return faceIndices.ToArray();
    }

    private FaceIndices ParseVertexIndices(string vertexData)
    {
        var indices = new FaceIndices();
        var subParts = vertexData.Split('/');

        
        if (subParts.Length > 0 && !string.IsNullOrEmpty(subParts[0]))
        {
            indices.VertexIndex = ParseAndAdjustIndex(subParts[0]);
        }

        
        if (subParts.Length > 1 && !string.IsNullOrEmpty(subParts[1]))
        {
            indices.TextureIndex = ParseAndAdjustIndex(subParts[1]);
        }

        
        if (subParts.Length > 2 && !string.IsNullOrEmpty(subParts[2]))
        {
            indices.NormalIndex = ParseAndAdjustIndex(subParts[2]);
        }

        return indices;
    }

    private int ParseAndAdjustIndex(string indexStr)
    {
        if (int.TryParse(indexStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
        {
            if (index > 0)
                return index - 1;
            
            return index;
        }
        return 0; 
    }
}