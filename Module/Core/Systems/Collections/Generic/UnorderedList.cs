using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Module.Core.Systems.Collections.Generic
{
    using System;

    [Serializable]
    public class UnorderedList<T> : ICollection<T>, ICollection, IReadOnlyList<T>
    {
        private const int m_DefaultCapacity = 4;

        private T[] m_Items;
        private int m_Size;
        private int m_Version;
        [NonSerialized]
        private Object m_SyncRoot;

        static readonly T[] m_EmptyArray = new T[0];

        public UnorderedList()
        {
            m_Items = m_EmptyArray;
        }

        public UnorderedList(int capacity)
        {
            if (capacity < 0) ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);

            if (capacity == 0)
                m_Items = m_EmptyArray;
            else
                m_Items = new T[capacity];
        }


        public UnorderedList(IEnumerable<T> collection)
        {
            if (collection == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);

            ICollection<T> c = collection as ICollection<T>;
            if (c != null)
            {
                int count = c.Count;
                if (count == 0)
                {
                    m_Items = m_EmptyArray;
                }
                else
                {
                    m_Items = new T[count];
                    c.CopyTo(m_Items, 0);
                    m_Size = count;
                }
            }
            else
            {
                m_Size = 0;
                m_Items = m_EmptyArray;

                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Add(en.Current);
                    }
                }
            }
        }

        public int Capacity
        {
            get => m_Items.Length;
            set
            {
                if (value < m_Size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
                }

                if (value != m_Items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (m_Size > 0)
                        {
                            Array.Copy(m_Items, 0, newItems, 0, m_Size);
                        }
                        m_Items = newItems;
                    }
                    else
                    {
                        m_Items = m_EmptyArray;
                    }
                }
            }
        }

        public int Count
        {
            get => m_Size;
        }

        bool ICollection<T>.IsReadOnly
        {
            get => false;
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get => false;
        }

        // Synchronization root for this object.
        Object System.Collections.ICollection.SyncRoot
        {
            get
            {
                if (m_SyncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<Object>(ref m_SyncRoot, new Object(), null);
                }
                return m_SyncRoot;
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
            return ((value is T) || (value == null && default(T) == null));
        }



        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)m_Size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException();
                }

                return m_Items[index];
            }

            set
            {
                if ((uint)index >= (uint)m_Size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException();
                }

                m_Items[index] = value;
                m_Version++;
            }
        }

        public void Add(T item)
        {
            if (m_Size == m_Items.Length) EnsureCapacity(m_Size + 1);
            m_Items[m_Size++] = item;
            m_Version++;
        }

        int Add(Object item)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);

            try
            {
                Add((T)item);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
            }

            return Count - 1;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            int index = m_Size;

            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }

            if ((uint)index > (uint)m_Size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
            }

            ICollection<T> c = collection as ICollection<T>;
            if(c!=null)
            {
                int count = c.Count;
                if(count > 0)
                {
                    EnsureCapacity(m_Size + count);

                    foreach(var val in c)
                    {
                        Add(val);
                    }
                }
            }
            else
            {
                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Add(en.Current);
                    }
                }
            }

            m_Version++;
        }

        public ReadOnlyList<T> AsReadOnly()
        {
            return new ReadOnlyList<T>(this);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            if (m_Size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

            return Array.BinarySearch<T>(m_Items, index, count, item, comparer);
        }

        public int BinarySearch(T item)
        {
            return BinarySearch(0, Count, item, null);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return BinarySearch(0, Count, item, comparer);
        }

        public void Clear()
        {
            if (m_Size > 0)
            {
                Array.Clear(m_Items, 0, m_Size); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
                m_Size = 0;
            }
            m_Version++;
        }

        public bool Contains(T item)
        {
            if ((Object)item == null)
            {
                for (int i = 0; i < m_Size; i++)
                    if ((Object)m_Items[i] == null)
                        return true;
                return false;
            }
            else
            {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < m_Size; i++)
                {
                    if (c.Equals(m_Items[i], item)) return true;
                }
                return false;
            }
        }

        bool Contains(Object item)
        {
            if (IsCompatibleObject(item))
            {
                return Contains((T)item);
            }
            return false;
        }

        public UnorderedList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.converter);
            }
            // @

            UnorderedList<TOutput> list = new UnorderedList<TOutput>(m_Size);
            for (int i = 0; i < m_Size; i++)
            {
                list.m_Items[i] = converter(m_Items[i]);
            }
            list.m_Size = m_Size;
            return list;
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
        {
            if ((array != null) && (array.Rank != 1))
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }

            try
            {
                // Array.Copy will check for NULL.
                Array.Copy(m_Items, 0, array, arrayIndex, m_Size);
            }
            catch (ArrayTypeMismatchException)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
            }
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (m_Size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            // Delegate rest of error checking to Array.Copy.
            Array.Copy(m_Items, index, array, arrayIndex, count);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            // Delegate rest of error checking to Array.Copy.
            Array.Copy(m_Items, 0, array, arrayIndex, m_Size);
        }

        private void EnsureCapacity(int min)
        {
            if (m_Items.Length < min)
            {
                int newCapacity = m_Items.Length == 0 ? m_DefaultCapacity : m_Items.Length * 2;
                // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                //if ((uint)newCapacity > Array.MaxArrayLength) newCapacity = Array.MaxArrayLength;
                if ((uint)newCapacity > 2146435071) newCapacity = 2146435071;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }

        public bool Exists(Predicate<T> match)
        {
            return FindIndex(match) != -1;
        }

        public T Find(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            for (int i = 0; i < m_Size; i++)
            {
                if (match(m_Items[i]))
                {
                    return m_Items[i];
                }
            }
            return default(T);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            if (match == null)
            {
                throw new Exception("match is null");
            }

            List<T> list = new List<T>();
            for (int i = 0; i < m_Size; i++)
            {
                if (match(m_Items[i]))
                {
                    list.Add(m_Items[i]);
                }
            }
            return list;
        }

        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, m_Size, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, m_Size - startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if ((uint)startIndex > (uint)m_Size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
            }

            if (count < 0 || startIndex > m_Size - count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
            }

            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (match(m_Items[i])) return i;
            }
            return -1;
        }

        public T FindLast(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            for (int i = m_Size - 1; i >= 0; i--)
            {
                if (match(m_Items[i]))
                {
                    return m_Items[i];
                }
            }
            return default(T);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(m_Size - 1, m_Size, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            if (m_Size == 0)
            {
                // Special case for 0 length List
                if (startIndex != -1)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
                }
            }
            else
            {
                // Make sure we're not out of range            
                if ((uint)startIndex >= (uint)m_Size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
                }
            }

            // 2nd have of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
            if (count < 0 || startIndex - count + 1 < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
            }

            int endIndex = startIndex - count;
            for (int i = startIndex; i > endIndex; i--)
            {
                if (match(m_Items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <internalonly/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public UnorderedList<T> GetRange(int index, int count)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (m_Size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            UnorderedList<T> list = new UnorderedList<T>(count);
            Array.Copy(m_Items, index, list.m_Items, 0, count);
            list.m_Size = count;
            return list;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(m_Items, item, 0, m_Size);
        }

        int IndexOf(Object item)
        {
            if (IsCompatibleObject(item))
            {
                return IndexOf((T)item);
            }
            return -1;
        }

        public int IndexOf(T item, int index)
        {
            if (index > m_Size)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);

            return Array.IndexOf(m_Items, item, index, m_Size - index);
        }

        public int IndexOf(T item, int index, int count)
        {
            if (index > m_Size)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);

            if (count < 0 || index > m_Size - count) ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);

            return Array.IndexOf(m_Items, item, index, count);
        }

        public int LastIndexOf(T item)
        {
            if (m_Size == 0)
            {  // Special case for empty list
                return -1;
            }
            else
            {
                return LastIndexOf(item, m_Size - 1, m_Size);
            }
        }

        public int LastIndexOf(T item, int index)
        {
            if (index >= m_Size)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);

            return LastIndexOf(item, index, index + 1);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            if ((Count != 0) && (index < 0))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if ((Count != 0) && (count < 0))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (m_Size == 0)
            {  // Special case for empty list
                return -1;
            }

            if (index >= m_Size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
            }

            if (count > index + 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
            }

            return Array.LastIndexOf(m_Items, item, index, count);
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        void Remove(Object item)
        {
            if (IsCompatibleObject(item))
            {
                Remove((T)item);
            }
        }

        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            int freeIndex = 0;   // the first free slot in items array

            // Find the first item which needs to be removed.
            while (freeIndex < m_Size && !match(m_Items[freeIndex])) freeIndex++;
            if (freeIndex >= m_Size) return 0;

            int current = freeIndex + 1;
            while (current < m_Size)
            {
                // Find the first item which needs to be kept.
                while (current < m_Size && match(m_Items[current])) current++;

                if (current < m_Size)
                {
                    // copy item to the free slot.
                    m_Items[freeIndex++] = m_Items[current++];
                }
            }

            Array.Clear(m_Items, freeIndex, m_Size - freeIndex);
            int result = m_Size - freeIndex;
            m_Size = freeIndex;
            m_Version++;
            return result;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)m_Size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            m_Items[index] = m_Items[m_Size - 1];
            m_Size--;
            m_Items[m_Size] = default(T);
            m_Version++;
        }

        public void Sort()
        {
            Sort(0, Count, null);
        }

        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (m_Size - index < count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);


            Array.Sort<T>(m_Items, index, count, comparer);
            m_Version++;
        }

        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            if (m_Size > 0)
            {
                Array.Sort(new System.ArraySegment<T>(m_Items, 0, m_Size).Array, comparison);
            }
        }

        public T[] ToArray()
        {
            T[] array = new T[m_Size];
            Array.Copy(m_Items, 0, array, 0, m_Size);
            return array;
        }
        public void TrimExcess()
        {
            int threshold = (int)(((double)m_Items.Length) * 0.9);
            if (m_Size < threshold)
            {
                Capacity = m_Size;
            }
        }

        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }

            for (int i = 0; i < m_Size; i++)
            {
                if (!match(m_Items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        [Serializable]
        public struct Enumerator : IEnumerator<T>, System.Collections.IEnumerator
        {
            private UnorderedList<T> list;
            private int index;
            private int version;
            private T current;

            internal Enumerator(UnorderedList<T> list)
            {
                this.list = list;
                index = 0;
                version = list.m_Version;
                current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {

                UnorderedList<T> localList = list;

                if (version == localList.m_Version && ((uint)index < (uint)localList.m_Size))
                {
                    current = localList.m_Items[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (version != list.m_Version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }

                index = list.m_Size + 1;
                current = default(T);
                return false;
            }

            public T Current
            {
                get
                {
                    return current;
                }
            }

            Object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == list.m_Size + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return Current;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                if (version != list.m_Version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                }

                index = 0;
                current = default(T);
            }

        }
    }
}

