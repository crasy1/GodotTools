using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Godot;

public static class StringExtension
{
    /// <summary>
    /// 将 JSON 字符串解析为动态对象
    /// </summary>
    /// <param name="jsonString">有效的 JSON 字符串</param>
    /// <returns>动态类型对象</returns>
    public static dynamic ToDynamicJson(this string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            throw new ArgumentNullException(nameof(jsonString), "JSON 字符串不能为空");
        }

        try
        {
            return JsonConvert.DeserializeObject<dynamic>(jsonString);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"JSON 解析失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 将 JSON 字符串转换为指定类型的对象
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="jsonString">有效的 JSON 字符串</param>
    /// <returns>解析后的对象</returns>
    public static T ToObject<T>(this string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            throw new ArgumentNullException(nameof(jsonString), "JSON 字符串不能为空");
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"JSON 解析失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 将 JSON 字符串转换为 JObject（用于对象类型的 JSON）
    /// </summary>
    public static JObject ToJObject(this string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            throw new ArgumentNullException(nameof(jsonString), "JSON 字符串不能为空");
        }

        try
        {
            return JObject.Parse(jsonString);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"JObject 解析失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 将 JSON 字符串转换为 JArray（用于数组类型的 JSON）
    /// </summary>
    public static JArray ToJArray(this string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            throw new ArgumentNullException(nameof(jsonString), "JSON 字符串不能为空");
        }

        try
        {
            return JArray.Parse(jsonString);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"JArray 解析失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 安全转换 JSON 字符串（不抛异常，返回默认值）
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="jsonString">JSON 字符串</param>
    /// <param name="defaultValue">解析失败时返回的默认值</param>
    /// <returns>解析结果或默认值</returns>
    public static T SafeConvertToJson<T>(this string jsonString, T? defaultValue = default)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return defaultValue;
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// 将 JSON 数组字符串转换为 List<T> 集合
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="jsonArrayString">有效的 JSON 数组字符串</param>
    /// <returns>对象集合</returns>
    public static List<T> ToList<T>(this string jsonArrayString)
    {
        if (string.IsNullOrWhiteSpace(jsonArrayString))
            throw new ArgumentNullException(nameof(jsonArrayString), "JSON 字符串不能为空");

        try
        {
            return JsonConvert.DeserializeObject<List<T>>(jsonArrayString);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"JSON 数组解析失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 安全地将 JSON 数组字符串转换为 List<T> 集合
    /// </summary>
    /// <param name="defaultValue">解析失败时返回的默认值（默认为空列表）</param>
    public static List<T> SafeToList<T>(this string jsonArrayString, List<T>? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(jsonArrayString))
            return defaultValue ?? [];

        try
        {
            return JsonConvert.DeserializeObject<List<T>>(jsonArrayString);
        }
        catch
        {
            return defaultValue ?? [];
        }
    }
}