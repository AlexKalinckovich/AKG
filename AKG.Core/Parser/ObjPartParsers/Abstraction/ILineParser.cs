using AKG.Core.Model;

namespace AKG.Core.Parser.ObjPartParsers.Abstraction;



public interface ILineTypeParser
{
    LineType SupportedLineType { get; }
    void ParseLine(string line, PartialModelData partialModelData);
}