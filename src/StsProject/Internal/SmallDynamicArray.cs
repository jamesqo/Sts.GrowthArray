using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    /// <summary>
    /// A list optimized for a small number of items.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    internal partial struct SmallDynamicArray<T> : IEnumerable<T>
    {
        /// <summary>
        /// The size of this list's buffer after the first item is added.
        /// </summary>
        private const int InitialCapacity = 4;

        /// <summary>
        /// The buffer where this list's items are stored.
        /// </summary>
        private T[] _buf;

        /// <summary>
        /// The number of items in this list.
        /// </summary>
        private int _size;

        /// <summary>
        /// Gets the number of items this list can hold before resizing.
        /// </summary>
        public int Capacity => _buf?.Length ?? 0;

        /// <summary>
        /// Gets the number of items in this list.
        /// </summary>
        public int Count => _size;

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"{nameof(Count)} = {Count}";

        /// <summary>
        /// Gets a value indicating whether this list is empty.
        /// </summary>
        public bool IsEmpty => _size == 0;

        /// <summary>
        /// Gets a value indicating whether this list is full.
        /// </summary>
        private bool IsFull => _size == Capacity;

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < _size);
                return ref _buf[index];
            }
        }

        public void Append(T item)
        {
            if (IsFull)
            {
                Grow();
            }

            _buf[_size++] = item;
        }

        /// <summary>
        /// Gets an enumerator that iterates through this list.
        /// </summary>
        public Enumerator GetEnumerator() => new Enumerator(_buf, _size);

        /// <summary>
        /// Removes the last item of this list.
        /// </summary>
        /// <returns>The removed item.</returns>
        public T RemoveLast()
        {
            Debug.Assert(!IsEmpty);

            return _buf[--_size];
        }

        /// <summary>
        /// Resizes this list when it is full.
        /// </summary>
        private void Grow()
        {
            Debug.Assert(IsFull);

            T[] newBuf;
            if (IsEmpty)
            {
                newBuf = new T[InitialCapacity];
            }
            else
            {
                newBuf = new T[_size * 2];
                Array.Copy(_buf, 0, newBuf, 0, _size);
            }
            
            _buf = newBuf;
        }

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
