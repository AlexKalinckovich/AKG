using System.Text;
using ACG.Model;
using ACG.Parser;
using AKG.Parser.NumericParser;

namespace AKG.Parser;

public class FaceParser
{
    private readonly List<FaceIndices> _tempFaceVertices = new List<FaceIndices>(4);

    public IReadOnlyList<FaceIndices> Parse(ReadOnlySpan<byte> line)
    {
        _tempFaceVertices.Clear();
        
        ParseLine(TrimSpaces(line));
        
        return _tempFaceVertices;
    }

    private void ParseLine(ReadOnlySpan<byte> line)
    {
        if (!line.IsEmpty)
        {
            ProcessNextToken(line);
        }
    }

    private void ProcessNextToken(ReadOnlySpan<byte> line)
    {
        ReadOnlySpan<byte> firstPart = GetFirstToken(line);
        
        ReadOnlySpan<byte> rest = GetRemaining(line);
        
        AddParsedToken(firstPart);
        
        ParseLine(TrimSpaces(rest));
    }

    private ReadOnlySpan<byte> GetFirstToken(ReadOnlySpan<byte> line)
    {
        int index = line.IndexOf((byte)' ');
        return index < 0 ? 
            line : line.Slice(0, index);
    }

    private ReadOnlySpan<byte> GetRemaining(ReadOnlySpan<byte> line)
    {
        int index = line.IndexOf((byte)' ');
        return index < 0 ? 
            ReadOnlySpan<byte>.Empty : line.Slice(index + 1);
    }

    private void AddParsedToken(ReadOnlySpan<byte> token)
    {
        FaceIndices indices = ParseToken(token);
        
        indices = AdjustIndices(indices);
        
        _tempFaceVertices.Add(indices);
    }

    private FaceIndices ParseToken(ReadOnlySpan<byte> token)
    {
        FaceIndices indices = new FaceIndices();
        
        indices = ParseVertexIndex(token, indices);

        return indices;
    }

    private FaceIndices ParseVertexIndex(ReadOnlySpan<byte> token, FaceIndices indices)
    {
        ReadOnlySpan<byte> remaining = token;
        
        ParseNumber(ref remaining, out indices.VertexIndex);
        
        
        return remaining.IsEmpty ? 
            indices : ParseAfterVertex(remaining, indices);
    }

    private FaceIndices ParseAfterVertex(ReadOnlySpan<byte> token, FaceIndices indices)
    {
        ReadOnlySpan<byte> remaining = token;
        
        return SkipSlashAndDoAction(remaining, indices, CheckNextPart);
    }

    private FaceIndices SkipSlashAndDoAction(ReadOnlySpan<byte> remaining, FaceIndices indices, Func<ReadOnlySpan<byte>, FaceIndices, FaceIndices> func)
    {
        remaining = SkipSlash(remaining);
        return remaining.IsEmpty ?
            indices : func(remaining, indices);
    }

    private FaceIndices CheckNextPart(ReadOnlySpan<byte> token, FaceIndices indices)
    {
        ReadOnlySpan<byte> remaining = token;
        
        bool nextIsSlash = remaining[0] == (byte)'/';
        
        return nextIsSlash ? 
            ParseNoTexture(remaining, indices) : ParseWithTexture(remaining, indices);
    }

    private FaceIndices ParseNoTexture(ReadOnlySpan<byte> token, FaceIndices indices)
    {
        ReadOnlySpan<byte> remaining = token;
        
        return SkipSlashAndDoAction(remaining, indices, ParseNormalIndex);
    }

    private FaceIndices ParseWithTexture(ReadOnlySpan<byte> token, FaceIndices indices)
    {
        ReadOnlySpan<byte> remaining = token;
        
        ParseNumber(ref remaining, out indices.TextureIndex);
        
        if (!remaining.IsEmpty)
        {
            return SkipSlashAndDoAction(remaining, indices, ParseNormalIndex);
        }

        return indices;
    }

    private FaceIndices ParseNormalIndex(ReadOnlySpan<byte> token, FaceIndices indices)
    {
        ReadOnlySpan<byte> remaining = token;
        
        ParseNumber(ref remaining, out indices.NormalIndex);
        
        return indices;
    }

    private void ParseNumber(ref ReadOnlySpan<byte> span, out int value)
    {
        IntParser.Parse(ref span, out value);
    }

    private ReadOnlySpan<byte> SkipSlash(ReadOnlySpan<byte> span)
    {
        if (!span.IsEmpty)
        {
            return span[0] == (byte)'/' ? 
                span.Slice(1) : span;
        }

        return span;
    }

    private FaceIndices AdjustIndices(FaceIndices indices)
    {
        if (indices.VertexIndex > 0) indices.VertexIndex--;
        if (indices.TextureIndex > 0) indices.TextureIndex--;
        if (indices.NormalIndex > 0) indices.NormalIndex--;
        return indices;
    }

    private static void LogSpan(ReadOnlySpan<byte> span, string message)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in span)
        {
            sb.Append((char)b);
        }
        Console.WriteLine($"{message}: {sb}");
    }
    private ReadOnlySpan<byte> TrimSpaces(ReadOnlySpan<byte> span)
    {
        const int spaceUnicode = 32;
        int index = 0;
        while (index < span.Length && span[index] <= spaceUnicode)
        {
            index++;
        }
        return span.Slice(index);
    }
}