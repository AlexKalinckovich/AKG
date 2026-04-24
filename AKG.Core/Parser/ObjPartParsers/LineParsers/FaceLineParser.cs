using AKG.Core.Model;
using AKG.Core.Parser.ObjPartParsers.DataParsers;
using AKG.Model;

namespace AKG.Core.Parser.ObjPartParsers.LineParsers;


public class FaceLineParser
{
    private readonly FaceIndicesStringParser _faceIndicesStringParser = new();

    public Face ParseFaceLineString(string faceLineString)
    {
        if (string.IsNullOrWhiteSpace(faceLineString))
        {
            throw new ArgumentNullException(nameof(faceLineString));
        }

        string[] faceLineParts = SplitFaceLineIntoParts(faceLineString);
        
        Face faceIndicesList = ProcessEachFaceLinePart(faceLineParts);
        
        return faceIndicesList;
    }

    private static string[] SplitFaceLineIntoParts(string faceLineString)
    {
        return faceLineString.Trim().Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
    }

    private Face ProcessEachFaceLinePart(string[] faceLineParts)
    {
        Span<FaceIndices> indices = stackalloc FaceIndices[faceLineParts.Length];
    
        for (int i = 0; i < faceLineParts.Length; i++)
        {
            indices[i] = _faceIndicesStringParser.ParseVertexIndicesFromString(faceLineParts[i]);
        }
    
        return faceLineParts.Length switch
        {
            3 => new Face(indices[0], indices[1], indices[2]),
            4 => new Face(indices[0], indices[1], indices[2], indices[3]),
            5 => new Face(indices[0], indices[1], indices[2], indices[3], indices[4]),
            _ => throw new ArgumentException($"Invalid face part count: {faceLineParts.Length}")
        };
    }
}