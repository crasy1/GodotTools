using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using Godot;
using FileAccess = Godot.FileAccess;

public class JsonUtil
{
    private static readonly JsonSerializerOptions DefaultSerializeOpts = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    /**
     * 序列化
     */
    public static string ToJsonString(object obj)
    {
        try
        {
            return JsonSerializer.Serialize(obj, DefaultSerializeOpts);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return null;
        }
    }

    /**
     * 反序列化
     */
    public static T ToObj<T>(string jsonString)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(jsonString);
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return default;
        }
    }

    /**
     * 反序列化
     */
    public static T ToObj<T>(string jsonString, Type type) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize(jsonString, type) as T;
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return default;
        }
    }

    /**
     * 反序列化
     */
    public static T ToObj<T>(JsonNode jsonNode)
    {
        try
        {
            return jsonNode.AsObject().Deserialize<T>();
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return default;
        }
    }

    /**
     * 反序列化
     */
    public static T FileToObj<T>(string filePath)
    {
        try
        {
            return ToObj<T>(FileToString(filePath));
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return default;
        }
    }

    public static JsonObject ToJsonObject(string jsonObjString)
    {
        try
        {
            return JsonNode.Parse(jsonObjString)!.AsObject();
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return null;
        }
    }

    public static JsonArray ToJsonArray(string jsonObjString)
    {
        try
        {
            return JsonNode.Parse(jsonObjString)!.AsArray();
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return null;
        }
    }

    public static JsonObject FileToJsonObject(string jsonFilePath)
    {
        try
        {
            return ToJsonObject(FileToString(jsonFilePath));
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return null;
        }
    }


    public static Dictionary<TKey, TValue> FileToDictionary<TKey, TValue>(string jsonFilePath)
    {
        try
        {
            return JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(FileToString(jsonFilePath));
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            return null;
        }
    }

    public static void DictionaryToFile<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string jsonFilePath)
    {
        StringToFile(ToJsonString(dictionary), jsonFilePath);
    }

    public static void ObjToFile(object obj, string jsonFilePath)
    {
        StringToFile(ToJsonString(obj), jsonFilePath);
    }

    public static string FileToString(string filePath)
    {
        try
        {
            using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
            return file.GetAsText();
        }
        catch (Exception e)
        {
            GD.PrintErr($"读取文件 {filePath} 错误 {e.Message}");
        }

        return null;
    }

    public static void StringToFile(string text, string filePath)
    {
        try
        {
            if (text is null)
            {
                throw new Exception("写入内容为空");
            }

            using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
            file.StoreString(text);
        }
        catch (Exception e)
        {
            GD.PrintErr($"写入文件 {filePath} 错误 {e.Message}");
        }
    }
}