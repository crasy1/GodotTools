namespace Godot;

public static class ListExtensions
{
    /// <summary>
    /// 随机取一个元素
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T PeekRandom<T>(this IList<T> list)
    {
        return list[GD.RandRange(0, list.Count - 1)];
    }

    /// <summary>
    /// 随机取一个元素并移除
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T PollRandom<T>(this IList<T> list)
    {
        var item = PeekRandom(list);
        list.Remove(item);
        return item;
    }

    /// <summary>
    /// 随机打乱
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    public static void Shuffle<T>(this IList<T> list)
    {
        if (list.Count <= 1)
        {
            return;
        }

        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = GD.RandRange(0, i);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    /// <summary>
    /// 复制
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IList<T> Duplicate<T>(this IEnumerable<T> list)
    {
        return list.ToList();
    }
}