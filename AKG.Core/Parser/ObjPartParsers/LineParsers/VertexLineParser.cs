using System.Numerics;
using AKG.Core.Model;
using AKG.Core.Parser.ObjPartParsers.Abstraction;
using AKG.Core.Parser.ObjPartParsers.DataParsers;

namespace AKG.Core.Parser.ObjPartParsers.LineParsers;

public class VertexLineParser : ILineTypeParser
{
    private readonly VertexDataParser _vertexDataParser = new();

    public LineType SupportedLineType => LineType.Vertex;

    public void ParseLine(string line, PartialModelData partialModelData)
    {
        string dataString = ExtractDataSubstringFromLine(line);
        TryParseVertexData(partialModelData, dataString);
    }

    private void TryParseVertexData(PartialModelData partialModelData, string dataString)
    {
        try
        {
            ParseVertexDataAndAddParsedDataToParticalModel(partialModelData, dataString);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void ParseVertexDataAndAddParsedDataToParticalModel(PartialModelData partialModelData, string dataString)
    {
        Vector4 vertex = _vertexDataParser.ParseVertexDataString(dataString);
            
        partialModelData.Vertices.Add(vertex);
            
        partialModelData.TotalVerticesProcessed++;
    }

    private string ExtractDataSubstringFromLine(string line)
    {
        return line.Substring(2).TrimStart();
    }
}