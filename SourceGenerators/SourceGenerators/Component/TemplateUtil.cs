using System.Reflection;
using Scriban;

namespace Godot.SourceGenerators;

internal static class TemplateUtil
{
    public static Template GetTemplate(string path)
    {
        return Template.Parse(GetTemplateString(path));
    }

    private static string GetTemplateString(string path)
    {
        return Assembly.GetExecutingAssembly().GetEmbeddedResource(path);
    }
}