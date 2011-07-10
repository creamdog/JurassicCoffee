using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace JurassicCoffee.Web.Collections.Concurrent
{
    class ConcurrentMonoPolyMap<TK, TV>
    {
        private readonly ConcurrentDictionary<TK, ConcurrentBag<TV>> _map;

        public ConcurrentMonoPolyMap()
        {
            _map = new ConcurrentDictionary<TK, ConcurrentBag<TV>>();
        }

        public TK[] GetAllKeysForValue(TV value)
        {
            return _map.Where(row => row.Value.Contains(value)).Select(row => row.Key).ToArray();
        }

        public bool ContainsKey(TK key)
        {
            return _map.ContainsKey(key);
        }

        public void Add(TK key, TV value)
        {
            if(!_map.ContainsKey(key))
                _map[key] = new ConcurrentBag<TV>();

            _map[key].Add(value);
        }

        public void RemoveKey(TK key)
        {
            ConcurrentBag<TV> bag;
            if (_map.ContainsKey(key))
                _map.TryRemove(key, out bag);
        }

    }
}
