using System;
using System.Collections.Generic;

namespace CMDR
{
    public class ConfigDictionary<T1, T2> : Dictionary<T1,T2>
    {
        public T2 GetValue(T1 key)
        {
            TryGetValue(key, out T2 value);
            return value;
        }

        public T2 Put(T1 key, T2 value)
        {
            if (ContainsKey(key))
                Remove(key);
            Add(key, value);
            return value;
        }

        public T2 AddIfAbsent(T1 key, T2 value)
        {
            if (!ContainsKey(key))
                Add(key, value);
            return value;
        }
    }
}