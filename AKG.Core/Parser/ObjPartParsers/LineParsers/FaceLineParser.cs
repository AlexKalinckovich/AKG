using AKG.Core.Model;
using AKG.Core.Parser.ObjPartParsers.DataParsers;

namespace AKG.Core.Parser.ObjPartParsers.LineParsers;


public class FaceLineParser
{
    private readonly FaceIndicesStringParser _faceIndicesStringParser = new();

    public FaceIndices[] ParseFaceLineString(string faceLineString)
    {
        if (string.IsNullOrWhiteSpace(faceLineString))
        {
            return [];
        }

        string[] faceLineParts = SplitFaceLineIntoParts(faceLineString);
        
        List<FaceIndices> faceIndicesList = new();

        ProcessEachFaceLinePart(faceLineParts, faceIndicesList);

        return faceIndicesList.ToArray();
    }

    private string[] SplitFaceLineIntoParts(string faceLineString)
    {
        return faceLineString.Trim().Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
    }

    private void ProcessEachFaceLinePart(string[] faceLineParts, List<FaceIndices> faceIndicesList)
    {
        foreach (string faceLinePart in faceLineParts)
        {
            FaceIndices faceIndices = _faceIndicesStringParser.ParseVertexIndicesFromString(faceLinePart);
            
            faceIndicesList.Add(faceIndices);
        }
    }
}