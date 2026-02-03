using System.Buffers.Text;
using ACG.Parser.NumericParser;

namespace AKG.Parser.NumericParser;

public class IntParser : BaseParser
{
    private static bool ParseDelegate(ReadOnlySpan<byte> source, out int val, out int consumed) => 
        Utf8Parser.TryParse(source, out val, out consumed);

    public static bool Parse(ref ReadOnlySpan<byte> span, out int value)
    {
        
        return ParseValue(ref span, out value, ParseDelegate);
    }
}