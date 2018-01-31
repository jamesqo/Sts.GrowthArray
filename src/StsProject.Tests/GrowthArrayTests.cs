using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StsProject.Tests.TestInternal;
using Xunit;
using static StsProject.GrowthArray;

namespace StsProject.Tests
{
    public class GrowthArrayTests
    {
        private GrowthArray<int> CreateCollection()
        {
            return new GrowthArray<int>();
        }

        private GrowthArray<int> CreateCollection(IEnumerable<int> items)
        {
            var collection = CreateCollection();
            foreach (var item in items)
            {
                collection.Append(item);
            }
            return collection;
        }

        [Fact]
        public void Constructor()
        {
            var collection = CreateCollection();
            ValidateEmpty(collection);
        }

        [Theory]
        [ClassData(typeof(TestEnumerables_Data))]
        public void IsFull(IEnumerable<int> items)
        {
            bool IsWholeNumber(double value)
            {
                return value >= 0 && value % 1 == 0;
            }

            var collection = CreateCollection(items);
            int g = GrowthFactor;
            int c0 = InitialCapacity;

            // The collection is full iff n = g^k * c0 for some whole number k.
            bool expected = items.Any() && IsWholeNumber(Math.Log((double)items.Count() / c0, g));

            Assert.Equal(expected, collection.IsFull);
        }

        [Theory]
        [ClassData(typeof(TestEnumerables_Data))]
        public void Append(IEnumerable<int> items)
        {
            var collection = CreateCollection();

            foreach (var item in items)
            {
                collection.Append(item);
            }

            ValidateContents(collection, items);
        }

        [Theory]
        [ClassData(typeof(TestEnumerables_Data))]
        public void GetItem(IEnumerable<int> items)
        {
            var collection = CreateCollection(items);
            var array = items.ToArray();
            int newValue = checked(collection.MaxOrDefault() + 1);

            for (int i = 0; i < collection.Size; i++)
            {
                Assert.Equal(array[i], collection[i]);
                collection[i] = newValue;
                Assert.Equal(newValue, collection[i]);
                collection[i] = array[i];
                Assert.Equal(array[i], collection[i]);
            }
        }

        [Theory]
        [ClassData(typeof(TestEnumerables_Data))]
        public void GetItemLogarithmic(IEnumerable<int> items)
        {
            var collection = CreateCollection(items);
            var array = items.ToArray();
            int newValue = checked(collection.MaxOrDefault() + 1);

            for (int i = 0; i < collection.Size; i++)
            {
                Assert.Equal(array[i], collection.GetItemLogarithmic(i));
                collection.GetItemLogarithmic(i) = newValue;
                Assert.Equal(newValue, collection.GetItemLogarithmic(i));
                collection.GetItemLogarithmic(i) = array[i];
                Assert.Equal(array[i], collection.GetItemLogarithmic(i));
            }
        }

        private void ValidateContents(GrowthArray<int> collection, IEnumerable<int> expected)
        {
            void ValidateCopyTo()
            {
                var buffer = new int[collection.Size];
                collection.CopyTo(buffer, 0);
                Assert.Equal(expected, buffer);
            }

            void ValidateExplicitGetEnumerator()
            {
                Assert.Equal(expected, collection); // IEnumerable<T>.GetEnumerator()
                Assert.Equal(expected, (IEnumerable)collection); // IEnumerable.GetEnumerator()
            }

            void ValidateGetEnumerator()
            {
                var list = new List<int>();
                foreach (var item in collection)
                {
                    list.Add(item);
                }
                Assert.Equal(expected, list);
            }

            void ValidateToRawArray()
            {
                Assert.Equal(expected, collection.ToRawArray());
            }

            ValidateSize(collection, expected.Count());

            ValidateCopyTo();
            ValidateExplicitGetEnumerator();
            ValidateGetEnumerator();
            ValidateToRawArray();

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

        private void ValidateEmpty(GrowthArray<int> collection)
        {
            ValidateContents(collection, Array.Empty<int>());

            var emptySpan = collection.GetBufferSpan(0);
            Assert.Empty(emptySpan);
        }

        private void ValidateSize(GrowthArray<int> collection, int size)
        {
            int g = GrowthFactor;
            int c0 = InitialCapacity;

            int ExpectedCapacity()
            {
                int expected = c0;
                while (expected < size)
                {
                    expected *= g;
                }
                return expected;
            }

            Assert.Equal(size, collection.Size);
            Assert.Equal(ExpectedCapacity(), collection.Capacity);

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