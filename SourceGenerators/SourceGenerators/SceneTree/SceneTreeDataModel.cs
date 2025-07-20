using Microsoft.CodeAnalysis;

namespace Godot.SourceGenerators;

public class SceneTreeDataModel(ISymbol symbol, INamedTypeSymbol @class) : DataModel(symbol, @class)
{
    public bool HasOnInstantiateAttribute { get; } =
        @class.GetMembers().Any(m => m.GetAttributes()
            .Any(attr => attr.AttributeClass.Name == "OnInstantiateAttribute"));

    public override string GeneratorClassFileName { get; } = $"{symbol}.Scene.g.cs";
}