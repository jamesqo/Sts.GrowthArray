﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// The implementation of this type was copied from the source of ArraySegment<T> at:
// https://github.com/dotnet/coreclr/blob/b3e859cb5777bb68dd15caac75ee861da98489ae/src/mscorlib/src/System/ArraySegment.cs.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using StsProject.Internal;
using StsProject.Internal.Diagnostics;
using SystemArray = System.Array;

namespace StsProject
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(EnumerableDebuggerProxy<>))]
    public partial struct Buffer<T> : IList<T>, IReadOnlyList<T>, IEquatable<Buffer<T>>
    {
        internal static Buffer<T> Empty { get; } = new Buffer<T>(SystemArray.Empty<T>());

        internal Buffer(T[] array)
        {
            Verify.NotNull(array, nameof(array));

            Array = array;
            Count = array.Length;
        }

        internal Buffer(T[] array, int count)
        {
            Verify.NotNull(array, nameof(array));
            Verify.InRange(count >= 0 && count <= array.Length, nameof(count));

            Array = array;
            Count = count;
        }

        public static bool operator ==(Buffer<T> left, Buffer<T> right) => left.Equals(right);

        public static bool operator !=(Buffer<T> left, Buffer<T> right) => !(left == right);

        public T[] Array { get; }

        public int Count { get; }

        public bool IsDefault => Array == null;

        public bool IsDefaultOrEmpty => Count == 0;

        public bool IsEmpty => !IsDefault && Count == 0;

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay => $"{nameof(Count)} = {Count}";

        public ref T this[int index] => ref Array[index];

        public ArraySegment<T> AsArraySegment() => new ArraySegment<T>(Array, 0, Count);

        public bool Contains(T item) => IndexOf(item) != -1;

        public void CopyTo(T[] destination) => CopyTo(destination, 0);

        public void CopyTo(T[] destination, int destinationIndex)
        {
            SystemArray.Copy(Array, 0, destination, destinationIndex, Count);
        }

        public override bool Equals(object obj)
            => obj is Buffer<T> other && Equals(other);

        public bool Equals(Buffer<T> other)
            => Array == other.Array
            && Count == other.Count;

        public T First()
        {
            Verify.ValidState(!IsEmpty, Strings.First_EmptyCollection);

            return Array[0];
        }

        public Enumerator GetEnumerator() => new Enumerator(Array, Count);

        public override int GetHashCode() => throw new NotSupportedException();

        public int IndexOf(T item)
            => SystemArray.IndexOf(Array, item, 0, Count);

        public T Last()
        {
            Verify.ValidState(!IsEmpty, Strings.Last_EmptyCollection);

            return Array[Count - 1];
        }

        public T[] ToArray()
        {
            if (IsEmpty)
            {
                return SystemArray.Empty<T>();
            }

            var array = new T[Count];
            CopyTo(array);
            return array;
        }

        [ExcludeFromCodeCoverage]
        T IList<T>.this[int index]
        {
            get => this[index];
            set => this[index] = value;
        }

        [ExcludeFromCodeCoverage]
        void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

        [ExcludeFromCodeCoverage]
        void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

        [ExcludeFromCodeCoverage]
        T IReadOnlyList<T>.this[int index] => this[index];

        [ExcludeFromCodeCoverage]
        bool ICollection<T>.IsReadOnly => false;

        [ExcludeFromCodeCoverage]
        void ICollection<T>.Add(T item) => throw new NotSupportedException();

        [ExcludeFromCodeCoverage]
        void ICollection<T>.Clear() => throw new NotSupportedException();

        [ExcludeFromCodeCoverage]
        bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

        [ExcludeFromCodeCoverage]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
