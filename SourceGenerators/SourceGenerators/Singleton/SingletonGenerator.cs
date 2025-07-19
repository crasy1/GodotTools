using Microsoft.CodeAnalysis;

namespace Godot.SourceGenerators;

[Generator]
public class SingletonGenerator : AttributeGenerator<SingletonDataModel>
{
    protected override string FullyQualifiedMetadataName() => "Godot.SingletonAttribute";

    protected override string TemplatePath() => "Godot.SourceGenerators.Singleton.SingletonTemplate.sbncs";

    protected override SingletonDataModel DataModel(INamedTypeSymbol symbol, IEnumerable<AttributeData> attributes,
        SyntaxNode node, SemanticModel semanticModel)
    {
        return new SingletonDataModel(symbol, symbol);
    }
}