using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using StsProject.Internal;
using StsProject.Internal.Diagnostics;
using static StsProject.GrowthArray;

namespace StsProject
{
    public static class GrowthArray
    {
        public const int GrowthFactor = 2;
        public const int InitialCapacity = 8;
    }

    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    public partial class GrowthArray<T> : IEnumerable<T>
    {
        private const int Log2InitialCapacity = 3;

        private T[] _head;
        private SmallDynamicArray<T[]> _tail; // This is a mutable struct field; do not make it readonly.
        private int _size;
        private int _capacity;
        private int _lastCapacity; // Equals (Size - HeadSize) and (Capacity - HeadCapacity).

        public GrowthArray()
        {
            _head = new T[InitialCapacity];
            _tail = SmallDynamicArray<T[]>.Create();
            _capacity = InitialCapacity;
        }

        public int Capacity => _capacity;

        // PERF: Consider _capacity - _lastCapacity.
        public int HeadCapacity => _head.Length;

        public int HeadSize => _size - _lastCapacity;

        public bool IsFull => _size == _capacity;

        public int NumberOfBuffers => _tail.Size + 1;

        public int Size => _size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(T item)
        {
            // We want to minimize the number of field accesses here, as this is a
            // hot codepath. 4 field reads and 1 field write is the best I could do it.

            // IsFull is inlined so we need not make another field access to _size.
            int size = _size;
            if (size == _capacity)
            {
                Grow();
            }

            // HeadSize is also inlined.
            int hsize = size - _lastCapacity;
            _head[hsize] = item;
            _size = size + 1;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow()
        {
            Debug.Assert(IsFull);

            _tail.Append(_head);
            var newHcap = Capacity == InitialCapacity ?
                (GrowthFactor - 1) * InitialCapacity :
                GrowthFactor * HeadCapacity;
            _head = new T[newHcap];
            _lastCapacity = _capacity;
            _capacity += newHcap;
        }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < _size);

                int bufferIndex, elementIndex;
                if (index >= InitialCapacity)
                {
                    bufferIndex = Math.Max(MathHelpers.CeilLog2(index + 1) - Log2InitialCapacity, 0);
                    elementIndex = index - (1 << (bufferIndex + Log2InitialCapacity - 1));
                }
                else
                {
                    bufferIndex = 0;
                    elementIndex = index;
                }

                return ref GetBuffer(bufferIndex)[elementIndex];
            }
        }

        public T[] GetBuffer(int bufferIndex)
        {
            Debug.Assert(bufferIndex >= 0 && bufferIndex <= _tail.Size);

            return bufferIndex < _tail.Size ? _tail[bufferIndex] : _head;
        }

        public ref T GetItemLogarithmic(int index)
        {
            Debug.Assert(index >= 0 && index < _size);

            int i = index;
            foreach (T[] buf in _tail)
            {
                if (i < buf.Length)
                {
                    return ref buf[i];
                }
                i -= buf.Length;
            }

            Debug.Assert(i < _head.Length);
            return ref _head[i];
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public BufferSpan<T> GetBufferSpan(int bufferIndex)
        {
            Debug.Assert(bufferIndex >= 0 && bufferIndex <= _tail.Size);

            return bufferIndex < _tail.Size ?
                new BufferSpan<T>(_tail[bufferIndex]) :
                new BufferSpan<T>(_head, HeadSize);
        }

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

            foreach (T[] buf in _tail)
            {
                Array.Copy(buf, 0, array, arrayIndex, buf.Length);
                arrayIndex += buf.Length;
            }

            Array.Copy(_head, 0, array, arrayIndex, HeadSize);
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"Size = {_size}";

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
