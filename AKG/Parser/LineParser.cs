using System.Numerics;
using ACG.Model;
using AKG.Parser.NumericParser;

namespace AKG.Parser;

public class LineParser
{
    private readonly ObjModel _model;
    private readonly FaceParser _faceParser;

    public LineParser()
    {
        _model = new ObjModel();
        _faceParser = new FaceParser();
    }

    public ObjModel GetCompleteModel()
    {
        return _model;
    }

    public void Parse(ReadOnlySpan<byte> line)
    {
        if (line.Length < 2) return;
    
        byte first = line[0];
        byte second = line[1];

        if (first == (byte)'v')
        {
            HandleVertexType(line, second);
        }
        else if (first == (byte)'f')
        {
            ParseFace(line.Slice(2)); 
        }
    }

    private void HandleVertexType(ReadOnlySpan<byte> line, byte second)
    {
        if (second == (byte)' ')
        {
            ParseVertex(line.Slice(2));
            return;
        }
    
        if (line.Length < 3) return;
        if (line[2] != (byte)' ') return;
    
        if (second == (byte)'t')
        {
            if (line.Length >= 3)
            {
                ParseTexture(line.Slice(3));
            }
        }
        else if (second == (byte)'n')
        {
            if (line.Length >= 3)
            {
                ParseNormal(line.Slice(3));
            }
        }
    }

    private void ParseVertex(ReadOnlySpan<byte> data)
    {
        if (!FloatParser.Parse(ref data, out float x)) return;
        if (!FloatParser.Parse(ref data, out float y)) return;
        if (!FloatParser.Parse(ref data, out float z)) return;

        float w = 1.0f;
        FloatParser.Parse(ref data, out w); 
    
        _model.AddVertices([new Vector4(x, y, z, w)]);
    }

    private void ParseTexture(ReadOnlySpan<byte> data)
    {
        if (!FloatParser.Parse(ref data, out float u)) return;
        if (!FloatParser.Parse(ref data, out float v)) return;
            
        _model.AddTextureCoords([new Vector2(u, v)]);
    }

    private void ParseNormal(ReadOnlySpan<byte> data)
    {
        if (!FloatParser.Parse(ref data, out float x)) return;
        if (!FloatParser.Parse(ref data, out float y)) return;
        if (!FloatParser.Parse(ref data, out float z)) return;

        _model.AddNormals([new Vector3(x, y, z)]);
    }

    private void ParseFace(ReadOnlySpan<byte> data)
    {
        IReadOnlyList<FaceIndices> faceIndicesList = _faceParser.Parse(data);
    
        if (faceIndicesList.Count >= 3)
        {
            _model.AddFaces(new[] { faceIndicesList.ToArray() });
        }
    }
}