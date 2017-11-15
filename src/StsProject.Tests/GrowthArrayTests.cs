using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StsProject.Tests
{
    public class GrowthArrayTests : AbstractArrayCollectionTests<GrowthArray<int>>
    {
        protected override GrowthArray<int> CreateCollection()
        {
            return new GrowthArray<int>();
        }

        protected override void ValidateContents(GrowthArray<int> collection, IEnumerable<int> expected)
        {
            base.ValidateContents(collection, expected);

            void ValidateBlocks()
            {
                int elementIndex = 0;

                for (int i = 0; i < collection.NumberOfBuffers; i++)
                {
                    var span = collection.GetBufferSpan(i);
                    Assert.Equal(expected.Skip(elementIndex).Take(span.Size), span);
                    elementIndex += span.Size;
                }
            }

            ValidateBlocks();
        }

        protected override void ValidateEmpty(GrowthArray<int> collection)
        {
            base.ValidateEmpty(collection);

            var emptySpan = collection.GetBufferSpan(0);
            Assert.Empty(emptySpan);
        }

        protected override void ValidateSize(GrowthArray<int> collection, int size)
        {
            base.ValidateSize(collection, size);

            int g = GrowthArray.GrowthFactor;
            int c0 = GrowthArray.InitialCapacity;

            int ExpectedNumberOfBuffers()
            {
                int expected = 1;
                for (int i = c0; i < size; i *= g)
                {
                    expected++;
                }
                return expected;
            }

            Assert.Equal(ExpectedNumberOfBuffers(), collection.NumberOfBuffers);
        }
    }
}