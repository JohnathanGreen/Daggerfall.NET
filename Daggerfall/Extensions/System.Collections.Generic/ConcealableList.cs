using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    /// <summary>
    /// Simply a <see cref="List&ltT&gt;"/> that can be implicitly casted to a <see cref="ReadOnlyCollection&lt;T&gt;"/> view of the <see cref="ConcealableList"/>, which is also accessible with <see cref="ReadOnlyView"/>.
    /// </summary>
    class ConcealableList<T> : List<T>
    {
        public ConcealableList() : base() { }
        public ConcealableList(int capacity) : base(capacity) { }
        public ConcealableList(IEnumerable<T> collection) : base(collection) { }

        ReadOnlyCollection<T> readOnlyView;

        public ReadOnlyCollection<T> ReadOnlyView{
            get { lock(this) return readOnlyView ?? (readOnlyView = new ReadOnlyCollection<T>(this)); }
        }

        public static implicit operator ReadOnlyCollection<T>(ConcealableList<T> list) { return list.ReadOnlyView; }
    }
}
