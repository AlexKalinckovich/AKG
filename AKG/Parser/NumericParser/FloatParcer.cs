using System.Buffers.Text;
using ACG.Parser.NumericParser;

namespace AKG.Parser.NumericParser;

public class FloatParser : BaseParser
{
    private static bool ParseDelegate(ReadOnlySpan<byte> source, out float val, out int consumed) => 
        Utf8Parser.TryParse(source, out val, out consumed);

    public static bool Parse(ref ReadOnlySpan<byte> span, out float value)
    {
        
        return ParseValue(ref span, out value, ParseDelegate);
    }
}