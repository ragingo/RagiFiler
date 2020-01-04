using System;
using System.Collections.Generic;

namespace RagiFiler.Collections
{
    static class CollectionUtils
    {
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> instance, TKey key, Func<TValue> value)
        {
            if (instance.ContainsKey(key))
            {
                return false;
            }
            instance.Add(key, value());
            return true;
        }
    }
}
