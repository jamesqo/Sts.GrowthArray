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
        private int _lastCapacity;

        public GrowthArray()
        {
            _head = new T[InitialCapacity];
            _tail = SmallDynamicArray<T[]>.Create();
            _capacity = InitialCapacity;
        }

        public int Capacity => _capacity;

        public int HeadCapacity => _head.Length;

        public int HeadSize => _size - _lastCapacity;

        public bool IsFull => _size == _capacity;

        public int NumberOfBuffers => _tail.Size + 1;

        public int Size => _size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(T item)
        {
            T[] head = _head;
            int size = _size;
            int hsize = size - _lastCapacity;

            if ((uint)hsize < (uint)head.Length)
            {
                head[hsize] = item;
                _size = size + 1;
            }
            else
            {
                AppendWithGrow(item);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendWithGrow(T item)
        {
            Grow();
            _head[0] = item;
            _size++;
        }

        private void Grow()
        {
            Debug.Assert(IsFull);

            _tail.Append(_head);
            var newHcap = _capacity == InitialCapacity ?
                (GrowthFactor - 1) * InitialCapacity :
                GrowthFactor * HeadCapacity;

            _head = new T[newHcap];
            _lastCapacity = _capacity;
            _capacity += newHcap;
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int bufferIndex = MathHelpers.CeilLog2(index + 1) - Log2InitialCapacity;
                // Check within range [1, _tail.Size - 1]. Common case for large growth arrays.
                // Since 'bufferIndex > 0' is not commonly false, use single &.
                var tail = _tail;
                if ((bufferIndex > 0) & (bufferIndex != tail.Size))
                {
                    T[] buffer = tail[bufferIndex];
                    int elementIndex = index - (1 << (bufferIndex + Log2InitialCapacity - 1));
                    return buffer[elementIndex];
                }
                else
                {
                    return GetItemUncommon(index);
                }
            }
            // set can be impl'd in a similar fashion.
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private T GetItemUncommon(int index)
        {
            if (index < InitialCapacity)
            {
                return GetBuffer(0)[index];
            }

            // bufferIndex == tail.Size, so the head is the target.
            int elementIndex = index - (1 << (_tail.Size + Log2InitialCapacity - 1));
            return _head[elementIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] GetBuffer(int bufferIndex)
        {
            Debug.Assert(bufferIndex >= 0 && bufferIndex <= _tail.Size);

            var tail = _tail;
            return bufferIndex < tail.Size ? tail[bufferIndex] : _head;
        }

        // SetItemLogarithmic can be done in a similar fashion.
        public T GetItemLogarithmic(int index)
        {
            Debug.Assert(index >= 0 && index < _size);

            int i = index;
            var tail = _tail;

            for (int j = 0, n = tail.Size; j < n; j++)
            {
                T[] buf = tail[j];
                if ((uint)i < (uint)buf.Length)
                {
                    return buf[i];
                }
                i -= buf.Length;
            }

            Debug.Assert(i < _head.Length);
            return _head[i];
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
