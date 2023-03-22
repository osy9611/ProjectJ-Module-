using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Core.Systems.Collections.Generic
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        IReadOnlyList<T> list;

        public ReadOnlyList(IReadOnlyList<T> list)
        {
            if (list == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.list);
            }
            this.list = list;
        }

        public int Count
        {
            get { return list.Count; }
        }

        public T this[int index]
        {
            get { return list[index]; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
    }
}
