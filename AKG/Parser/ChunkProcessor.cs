using ACG.Model;
using AKG.FileHelper;
using AKG.Parser;

namespace ACG.Parser;

public class ChunkProcessor
{
    private readonly LineParser _lineParser;
    private readonly byte[] _leftover = new byte[1024];
    private int _leftoverSize = 0;

    public ChunkProcessor()
    {
        _lineParser = new LineParser();
    }

    public ObjModel Process(byte[] buffer, int count)
    {
        ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(buffer, 0, count);

        if (_leftoverSize > 0)
        {
            HandleLeftoverWrap(span);
        }
        else
        {
            ScanAndParse(span);
        }

        return _lineParser.GetCompleteModel();
    }

    private void HandleLeftoverWrap(ReadOnlySpan<byte> buffer)
    {
        int newlineIdx = buffer.IndexOf((byte)'\n');

        if (newlineIdx >= 0)
        {
            ReadOnlySpan<byte> endOfLine = buffer.Slice(0, newlineIdx);
            
            AppendToLeftover(endOfLine);

            ParseLeftoverPartAndScanPartAfterLeftover(buffer, newlineIdx);
        }
        else
        {
            AppendToLeftover(buffer);
        }
    }

    private void ParseLeftoverPartAndScanPartAfterLeftover(ReadOnlySpan<byte> buffer, int newlineIdx)
    {
        ParseLeftoverPart();
        
        int indexAfterLeftoverPart = newlineIdx + 1;
        
        ScanAndParse(buffer.Slice(indexAfterLeftoverPart));
    }

    private void ParseLeftoverPart()
    {
        _lineParser.Parse(_leftover.AsSpan(0, _leftoverSize));
        _leftoverSize = 0;
    }

    private void AppendToLeftover(ReadOnlySpan<byte> data)
    {
        data.CopyTo(_leftover.AsSpan(_leftoverSize));
        _leftoverSize += data.Length;
    }

    private void ScanAndParse(ReadOnlySpan<byte> data)
    {
        int offset = 0;
        bool hasReachedEndOfData = false;
    
        while (offset < data.Length && !hasReachedEndOfData)
        {
            ReadOnlySpan<byte> remaining = data.Slice(offset);
            int newlineIdx = remaining.IndexOf((byte)'\n');

            if (newlineIdx < 0)
            {
                AppendToLeftover(remaining);
                hasReachedEndOfData = true;
            }
            else
            {
                ReadOnlySpan<byte> line = remaining.Slice(0, newlineIdx);
                
                _lineParser.Parse(line);
                
                offset += newlineIdx + 1;
            }
        }
    }
}