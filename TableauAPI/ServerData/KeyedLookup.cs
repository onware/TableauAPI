using System.Collections.Generic;

namespace TableauAPI.ServerData
{
    /// <summary>
    /// Efficent lookup for unevenly distributed sets of data
    /// </summary>
    internal class KeyedLookup<T> where T : IHasSiteItemId
    {
        private readonly Dictionary<string, T> _dictionary = new Dictionary<string, T>();
        public void AddItem(string key, T item)
        {
            _dictionary.Add(key, item);
        }

        /// <summary>
        /// Look up an item by key, return NULL if not found
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T FindItem(string key)
        {
            T outItem;
            var found = _dictionary.TryGetValue(key, out outItem);
            if(!found)
            {
                return default(T);
            }
            return outItem;
        }


        /// <summary>
        /// Add the whole set of items
        /// </summary>
        /// <param name="items"></param>
        public KeyedLookup(IEnumerable<T> items)
        {
            foreach(var thisItem in items)
            {
                AddItem(thisItem.Id, thisItem);
            }
        }
    }
}
