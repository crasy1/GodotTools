using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Godot.SourceGenerators;

public static class SymbolExtensions
{
    public static string FullName(this ISymbol symbol)
    {
        var ns = symbol.NamespaceOrNull();
        return ns is null ? $"global::{symbol.Name}" : $"{ns}.{symbol.Name}";
    }

    public static string? NamespaceOrNull(this ISymbol symbol)
        => symbol.ContainingNamespace.IsGlobalNamespace ? null : string.Join(".", symbol.ContainingNamespace.ConstituentNamespaces);

    public static string? GetNamespaceDeclaration(this ISymbol symbol)
    {
        var ns = symbol.NamespaceOrNull();
        return ns is null ? null : $"namespace {ns};";
    }

    public static INamedTypeSymbol? OuterType(this ISymbol symbol)
        => symbol.ContainingType?.OuterType() ?? symbol as INamedTypeSymbol;

    public static string ClassName(this INamedTypeSymbol symbol)
        => symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

    public static string? ClassPath(this INamedTypeSymbol symbol)
        => symbol.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree?.FilePath ;
    

    public static string GetDeclaredAccessibility(this ISymbol symbol)
        => SyntaxFacts.GetText(symbol.DeclaredAccessibility);
}
