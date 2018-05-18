using System;
using System.Collections.Generic;

namespace rbt.container.util
{
    internal class HashSet
    {
        private readonly Dictionary<string, Boolean> stringDic = new Dictionary<string, bool>();

        public Boolean ContainsKey(string key)
        {
            return stringDic.ContainsKey(key);
        }

        public void Add(string key)
        {
            if (!stringDic.ContainsKey(key))
            {
                stringDic.Add(key, true);
            }
        }

        public void Remove(string key)
        {
            if (!stringDic.ContainsKey(key))
            {
                stringDic.Remove(key);
            }
        }
    }
}