using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Godot.SourceGenerators;

public abstract class AttributeGenerator<D> : IIncrementalGenerator where D : DataModel
{
    private static readonly char[] InvalidFileNameChars =
    [
        '\"', '<', '>', '|', '\0',
        (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
        (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
        (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
        (char)31, ':', '*', '?', '\\', '/'
    ];


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 获取包含 [T] 特性的类
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                FullyQualifiedMetadataName(),
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (context, _) => (
                    Symbol: (INamedTypeSymbol)context.TargetSymbol,
                    Attributes: context.Attributes,
                    Node: context.TargetNode,
                    SemanticModel: context.SemanticModel
                )
            )
            .Where(item => item.Symbol != null); // 过滤
        // 生成代码
        context.RegisterSourceOutput(provider, (productionContext, tuple) =>
        {
            var (symbol, attributes, node, semanticModel) = tuple;
            var dataModel = DataModel(symbol, attributes, node, semanticModel);
            var templateRenderText = TemplateUtil.GetTemplate(TemplatePath()).Render(dataModel, member => member.Name);
            var sourceText = SourceText.From(templateRenderText, Encoding.UTF8);
            productionContext.AddSource(GenerateValidFileName(dataModel.GeneratorClassFileName), sourceText);
        });
    }


    /// <summary>
    ///   生成有效的文件名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected static string GenerateValidFileName(string name)
    {
        var invalidChars = InvalidFileNameChars;
        var sb = new StringBuilder();
        foreach (var c in name)
        {
            if (!invalidChars.Contains(c))
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 特性完整的名字
    /// </summary>
    /// <returns></returns>
    protected abstract string FullyQualifiedMetadataName();

    /// <summary>
    /// sciban模板路径
    /// </summary>
    /// <returns></returns>
    protected abstract string TemplatePath();

    /// <summary>
    /// 构建数据模型
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributes"></param>
    /// <param name="node"></param>
    /// <param name="semanticModel"></param>
    /// <returns></returns>
    protected abstract D DataModel(INamedTypeSymbol symbol, IEnumerable<AttributeData> attributes,
        SyntaxNode node, SemanticModel semanticModel);
}