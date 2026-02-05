using AKG.Core.Model;

namespace AKG.Core.Parser;

using System.Globalization;
using System.Numerics;


public class LineParser
{
    private ParticalModelData _partialData;
    private readonly FaceParser _faceParser;

    public LineParser()
    {
        _partialData = new ParticalModelData();
        _faceParser = new FaceParser();
    }

    public ParticalModelData ParseLines(string[] lines)
    {
        _partialData = new ParticalModelData();
        
        foreach (string line in lines)
        {
            ParseLine(line);
        }
        
        return _partialData;
    }
    public ParticalModelData GetPartialModel()
    {
        return _partialData;
    }

    public void ParseLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line) || line.Length < 2)
            return;

        ParseLine2(line);
    }

    private void ParseLine2(string line)
    {
        line = line.TrimStart();
        char first = line[0];
        char second = line.Length > 1 ? line[1] : ' ';

        if (first == 'v')
        {
            HandleVertexType(line, second);
        }
        else if (first == 'f')
        {
            ParseFace(line);
            _partialData.TotalFacesProcessed++;
        }
    }

    private void HandleVertexType(string line, char second)
    {
        string data = line.Substring(2).TrimStart();
        
        if (second == ' ' || second == '\t') 
        {
            ParseVertex(data);
            _partialData.TotalVerticesProcessed++;
        }
        else if (line.Length > 2 && line[2] == ' ')
        {
            if (second == 't') 
            {
                ParseTexture(data);
            }
            else if (second == 'n') 
            {
                ParseNormal(data);
            }
        }
    }

    private void ParseVertex(string data)
    {
        string[] parts = SplitData(data);
        
        if (parts.Length < 3) return;

        if (TryParseFloat(parts[0], out float x) &&
            TryParseFloat(parts[1], out float y) &&
            TryParseFloat(parts[2], out float z))
        {
            float w = 1.0f;
            if (parts.Length > 3 && TryParseFloat(parts[3], out float parsedW))
                w = parsedW;

            _partialData.Vertices.Add(new Vector4(x, y, z, w));
        }
    }

    private void ParseTexture(string data)
    {
        string[] parts = SplitData(data);
        if (parts.Length < 2) return;

        if (TryParseFloat(parts[0], out float u) &&
            TryParseFloat(parts[1], out float v))
        {
            _partialData.TextureCoords.Add(new Vector2(u, v));
        }
    }

    private void ParseNormal(string data)
    {
        string[] parts = SplitData(data);
        if (parts.Length < 3) return;

        if (TryParseFloat(parts[0], out float x) &&
            TryParseFloat(parts[1], out float y) &&
            TryParseFloat(parts[2], out float z))
        {
            _partialData.Normals.Add(new Vector3(x, y, z));
        }
    }

    private void ParseFace(string line)
    {
        
        string faceData = line.Substring(1).TrimStart();
        FaceIndices[] faceIndices = _faceParser.ParseFaceLine(faceData);
        
        if (faceIndices.Length >= 3) 
        {
            _partialData.Faces.Add(faceIndices);
        }
    }

    private bool TryParseFloat(string str, out float value)
    {
        return float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private string[] SplitData(string data)
    {
        return data.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    }
}