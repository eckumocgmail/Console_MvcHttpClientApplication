using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Расширения коллекций
/// </summary>
public static class CollectionsExtensions
{


    /// <summary>
    /// Потсраничная навигация
    /// </summary>
    public static IEnumerable<T> GetPage<T>(this IEnumerable<T> set, int page, int size = 10)
    {
        return set.Skip((page - 1) * size).Take(size);
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool Remove<T>(this IDictionary<string, T> source, T separator)
    {
        return source.Remove(separator);
    }


    /// <summary>
    /// По каждому элементу
    /// </summary>
    public static string ToText(this IEnumerable<string> source, string separator)
    {
        string result = "";
        foreach(var src in source)
        {
            result += src + separator;
        }
        return result;
    }
    /// <summary>
    /// По каждому элементу
    /// </summary>
    public static void ForEach<TSource>(this ISet<TSource> source, Action<TSource> action)    
        => source.ToList().ForEach(action);
    
    /// <summary>
    /// По каждому элементу
    /// </summary>
    public static void AddAll<TSource>(this IProducerConsumerCollection<TSource> source, IEnumerable<TSource> items) where TSource : class
        => items.ToList().ForEach(item=> source.TryAdd(item));

    /// <summary>
    /// По каждому элементу
    /// </summary>
    public static void EnqueueAll<TSource>(this Queue<TSource> source, IEnumerable<TSource> items) where TSource : class
        => items.ToList().ForEach(source.Enqueue);

    /// <summary>
    /// Печать коллекции
    /// </summary>
    public static void Print(this IEnumerable items)
    {
        foreach (var item in items)
        {
            Api.Utils.Info(item.ToString());
        }
    }

    /// <summary>
    /// По каждому элементу
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> items,Action<T> todo)
    {
        foreach (var item in new List<T>(items))
        {

            todo(item);
        }     
    }

    /// <summary>
    /// Добавление коллекции
    /// </summary>
    public static HashSet<T> AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            set.Add(item);
        }
        return set;
    }

    /// <summary>
    /// Добавление коллекции
    /// </summary>
    public static Dictionary<string, T> AddRange<T>(this Dictionary<string,T> set, Dictionary<string, T> items)
    {
        foreach (var item in items)
        {
            set[item.Key] = item.Value;
        }
        return set;
    }
 
    /// <summary>
    /// Исключение из справочника
    /// </summary>
    public static Dictionary<string, string> Expect(
        this IDictionary<string, string> data, params string[] keys)
    {
        Dictionary<string, string> res = new Dictionary<string, string>();
        var expectedKeySet = new HashSet<string>(keys);
        foreach (var p in data)
        {
            if (expectedKeySet.Contains(p.Key) == false)
            {
                res[p.Key] = p.Value;
            }
        }
        return res;
    }
}
