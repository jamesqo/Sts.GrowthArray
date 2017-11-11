using System;
using System.Collections;
using System.Collections.Generic;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    public partial struct GrowthArray<T>
    {
        // In C#, 'foreach (var item in collection) { ..code.. }' is translated by the compiler to:
        //
        // var e = collection.GetEnumerator();
        // while (e.MoveNext())
        // {
        //     var item = e.Current;
        //     ..code..
        // }
        //
        // Under the above call pattern, the following type defines an algorithm equivalent to the one
        // presented in Section 5.3 for growth arrays.

        public struct Enumerator : IEnumerator<T>
        {
            private readonly GrowthArray<T> _growthArray;

            private BufferSpan<T> _currentSpan;
            private int _bufferIndex;
            private int _elementIndex;

            internal Enumerator(GrowthArray<T> growthArray)
            {
                _growthArray = growthArray;
                _currentSpan = growthArray.GetBufferSpan(0);
                _bufferIndex = 0;
                _elementIndex = -1;
            }

            public T Current => _currentSpan[_elementIndex];

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_elementIndex + 1 == _currentSpan.Size)
                {
                    if (_bufferIndex == _growthArray._tail.Size)
                    {
                        return false;
                    }

                    _currentSpan = _growthArray.GetBufferSpan(++_bufferIndex);
                    _elementIndex = -1;
                }

                _elementIndex++;
                return true;
            }

            [ExcludeFromCodeCoverage]
            object IEnumerator.Current => Current;

            [ExcludeFromCodeCoverage]
            void IEnumerator.Reset() => throw new NotSupportedException();
        }
    }
}
