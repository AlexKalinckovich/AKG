using AKG.Core.Model;
using AKG.Core.Parser.ObjPartParsers.Abstraction;

namespace AKG.Core.Parser.ObjPartParsers.LineParsers;

public class FaceLineTypeParser : ILineTypeParser
{
    private readonly FaceLineParser _faceLineParser = new();

    public LineType SupportedLineType => LineType.Face;

    public void ParseLine(string line, PartialModelData partialModelData)
    {
        string dataString = ExtractDataSubstringFromLine(line);
        FaceIndices[] faceIndices = _faceLineParser.ParseFaceLineString(dataString);
        
        if (faceIndices.Length >= 3)
        {
            partialModelData.Faces.Add(faceIndices);
        }
        
        partialModelData.TotalFacesProcessed++;
    }

    private string ExtractDataSubstringFromLine(string line)
    {
        return line.Substring(1).TrimStart();
    }
}