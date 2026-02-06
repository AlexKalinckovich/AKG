// ================ VertexDataParser.cs ================

using System.Numerics;
using AKG.Core.Parser.Numeric;

namespace AKG.Core.Parser.ObjPartParsers.DataParsers;

public class VertexDataParser
{
    private readonly FloatStringParser _floatStringParser = new();

    public Vector4 ParseVertexDataString(string dataString)
    {
        string[] dataParts = SplitDataString(dataString);

        if (!HasEnoughVertexDataParts(dataParts))
        {
            throw new FormatException("Vertex data part is not enough to parse vertex data string");
        }
        
        
        return CreateVector4FromDataParts(dataParts);
    }

    private string[] SplitDataString(string dataString)
    {
        return dataString.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private bool HasEnoughVertexDataParts(string[] dataParts)
    {
        return dataParts.Length >= 3;
    }

    private Vector4 CreateVector4FromDataParts(string[] dataParts)
    {
        float x = _floatStringParser.ParseFloatFromString(dataParts[0]);
        float y = _floatStringParser.ParseFloatFromString(dataParts[1]);
        float z = _floatStringParser.ParseFloatFromString(dataParts[2]);
        float w = ParseWComponentFromDataParts(dataParts);

        return new Vector4(x, y, z, w);
    }

    private float ParseWComponentFromDataParts(string[] dataParts)
    {
        if (dataParts.Length > 3)
        {
            return _floatStringParser.ParseFloatFromString(dataParts[3]);
        }
        
        return 1.0f;
    }
}