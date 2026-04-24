using AKG.Core.Model;
using AKG.Core.Parser.ObjPartParsers.Abstraction;
using AKG.Model;

namespace AKG.Core.Parser.ObjPartParsers.LineParsers;

public class FaceLineTypeParser : ILineTypeParser
{
    private readonly FaceLineParser _faceLineParser = new();

    public LineType SupportedLineType => LineType.Face;

    public void ParseLine(string line, PartialModelData partialModelData)
    {
        string dataString = ExtractDataSubstringFromLine(line);
        try
        {
            Face face = _faceLineParser.ParseFaceLineString(dataString);
            partialModelData.Faces.Add(face);
        }
        finally
        {
            partialModelData.TotalFacesProcessed++;
        }
    }

    private string ExtractDataSubstringFromLine(string line)
    {
        return line.Substring(1).TrimStart();
    }
}