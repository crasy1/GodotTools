using Microsoft.CodeAnalysis;

namespace Godot.SourceGenerators;

public abstract class DataModel(ISymbol symbol, INamedTypeSymbol @class)
{
    /// <summary>
    /// 命名空间
    /// </summary>
    public string Namespace { get; } = symbol.GetNamespaceDeclaration();
    
    /// <summary>
    /// 类名
    /// </summary>
    public string ClassName { get; } = @class.ClassName();
    private string DefaultGeneratorClassFileName => $"{symbol}.g.cs";
    /// <summary>
    /// 生成文件名
    /// </summary>
    public virtual string GeneratorClassFileName => DefaultGeneratorClassFileName;
}