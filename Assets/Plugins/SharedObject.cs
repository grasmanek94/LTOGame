using System;
using System.Collections.Generic;

public class SharedObject
{
    private static Dictionary<string, Dictionary<Type, object>> collections = new Dictionary<string, Dictionary<Type, object>>();
    
    public static void Set<T>(string key, T value)
    {
        if (!collections.ContainsKey(key))
        {
            collections.Add(key, new Dictionary<Type, object>());
        }

        Type t = typeof(T);
        if (!collections[key].ContainsKey(t))
        {
            collections[key].Add(t, value);
        }
        else
        {
            collections[key][t] = value;
        }
    }

    public static T Get<T>(string key)
    {
        if (collections.ContainsKey(key))
        {
            Type t = typeof(T);
            if (collections[key].ContainsKey(t))
            {
                return (T)collections[key][t];
            }
        }
        return default(T);
    }
}