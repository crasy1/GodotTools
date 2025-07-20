using Microsoft.CodeAnalysis;

namespace Godot.SourceGenerators;

[Generator]
public class SceneTreeGenerator : AttributeGenerator<SceneTreeDataModel>
{
    protected override string FullyQualifiedMetadataName() => "Godot.SceneTreeAttribute";

    protected override string TemplatePath() => "Godot.SourceGenerators.SceneTree.SceneTreeTemplate.sbncs";

    protected override SceneTreeDataModel DataModel(INamedTypeSymbol symbol, IEnumerable<AttributeData> attributes,
        SyntaxNode node, SemanticModel semanticModel)
    {
        return new SceneTreeDataModel(symbol, symbol);
    }
}