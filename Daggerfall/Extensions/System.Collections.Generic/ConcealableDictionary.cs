using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    /// <summary>
    /// Simply a <see cref="Dictionary&ltT&gt;"/> that can be implicitly casted to a <see cref="ReadOnlyDictionary&lt;T&gt;"/> view of the <see cref="ConcealableDictionary"/>, which is also accessible with <see cref="ReadOnlyView"/>.
    /// </summary>
    class ConcealableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public ConcealableDictionary() : base() { }
        public ConcealableDictionary(int capacity) : base(capacity) { }
        public ConcealableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public ConcealableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        public ConcealableDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ConcealableDictionary(int capacity, EqualityComparer<TKey> comparer) : base(capacity, comparer) { }
        public ConcealableDictionary(IDictionary<TKey, TValue> dictionary, EqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

        ReadOnlyDictionary<TKey, TValue> readOnlyView;

        public ReadOnlyDictionary<TKey, TValue> ReadOnlyView
        {
            get { lock (this) return readOnlyView ?? (readOnlyView = new ReadOnlyDictionary<TKey, TValue>(this)); }
        }

        public static implicit operator ReadOnlyDictionary<TKey, TValue>(ConcealableDictionary<TKey, TValue> list) { return list.ReadOnlyView; }
    }
}
