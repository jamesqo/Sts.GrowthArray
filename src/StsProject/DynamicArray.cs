using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using StsProject.Internal.Diagnostics;
using static StsProject.DynamicArray;

namespace StsProject
{
    public static class DynamicArray
    {
        public const int GrowthFactor = 2;
        public const int InitialCapacity = 8;
    }

    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    public partial class DynamicArray<T> : IArrayCollection<T>, IEnumerable<T>, IArrayCollectionSettings
    {
        private T[] _buf;
        private int _size;

        public DynamicArray()
        {
            _buf = new T[InitialCapacity];
        }

        public int Capacity => _buf.Length;

        public bool IsFull => _size == Capacity;

        public int Size => _size;

        public void Append(T item)
        {
            if (IsFull)
            {
                Grow();
            }

            _buf[_size++] = item;
        }

        private void Grow()
        {
            Debug.Assert(IsFull);

            T[] newBuf = new T[_size * GrowthFactor];
            Array.Copy(_buf, 0, newBuf, 0, _size);

            _buf = newBuf;
        }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < _size);
                return ref _buf[index];
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(_buf, _size);

        public T[] ToRawArray()
        {
            var array = new T[_size];
            CopyTo(array, 0);
            return array;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Debug.Assert(array != null);
            Debug.Assert(arrayIndex >= 0 && array.Length - arrayIndex >= _size);

            Array.Copy(_buf, 0, array, arrayIndex, _size);
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"Size = {_size}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IArrayCollectionSettings IArrayCollection<T>.Settings => this;

        int IArrayCollectionSettings.GrowthFactor => GrowthFactor;

        int IArrayCollectionSettings.InitialCapacity => InitialCapacity;
    }
}
