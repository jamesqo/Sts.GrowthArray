using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using StsProject.Internal.Diagnostics;

namespace StsProject
{
    public partial struct GrowthArray<T>
    {
        public struct Enumerator : IEnumerator<T>
        {
            private readonly BlockView<T> _blocks;

            private Buffer<T> _currentBlock;
            private int _blockIndex;
            private int _elementIndex;

            internal Enumerator(GrowthArray<T> list)
                : this()
            {
                Debug.Assert(list != null);

                _blocks = list.Blocks;
                _currentBlock = _blocks[0];
                _elementIndex = -1;
            }

            public T Current => _currentBlock[_elementIndex];

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_elementIndex + 1 == _currentBlock.Size)
                {
                    if (_blockIndex + 1 == _blocks.Count)
                    {
                        return false;
                    }

                    _currentBlock = _blocks[++_blockIndex];
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
