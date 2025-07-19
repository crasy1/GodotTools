using Microsoft.CodeAnalysis;

namespace Godot.SourceGenerators;

public class SingletonDataModel(ISymbol symbol, INamedTypeSymbol @class) : DataModel(symbol, @class)
{
    public override string GeneratorClassFileName { get; } = $"{symbol}.Singleton.g.cs";
}