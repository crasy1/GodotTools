using Godot;

namespace GodotTools.extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// 随机取字典中的键
    /// </summary>
    /// <param name="dictionary"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static TKey RandomKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        return dictionary.Keys.ToList().PeekRandom();
    }

    /// <summary>
    /// 随机取字典中的值
    /// </summary>
    /// <param name="dictionary"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static TValue RandomValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        return dictionary.Values.ToList().PeekRandom();
    }
}