using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StsProject.Tests.TestInternal;
using Xunit;

namespace StsProject.Tests
{
    public abstract class AbstractArrayCollectionTests<TCollection>
        where TCollection : IArrayCollection<int>
    {
        protected abstract TCollection CreateCollection();

        protected virtual TCollection CreateCollection(IEnumerable<int> items)
        {
            var collection = CreateCollection();
            collection.AppendRange(items);
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
            var collection = CreateCollection(items);
            int g = collection.Settings.GrowthFactor;
            int c0 = collection.Settings.InitialCapacity;

            // The collection is full iff n = g^k * c0 for some integer k.
            bool expected = Math.Log((double)items.Count() / c0, g) % 1 == 0;

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

        protected virtual void ValidateContents(TCollection collection, IEnumerable<int> expected)
        {
            // TODO
            //void ValidateBlocks()
            //{
            //    int elementIndex = 0;

            //    for (int i = 0; i < growthArray.NumberOfBuffers; i++)
            //    {
            //        var span = growthArray.GetBufferSpan(i);
            //        Assert.Equal(expected.Skip(elementIndex).Take(span.Size), span);
            //        elementIndex += span.Size;
            //    }
            //}

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

            //ValidateBlocks();
            ValidateCopyTo();
            ValidateExplicitGetEnumerator();
            ValidateGetEnumerator();
            ValidateToRawArray();
        }

        protected virtual void ValidateEmpty(TCollection collection)
        {
            ValidateContents(collection, Array.Empty<int>());

            // TODO
            // var emptySpan = growthArray.GetBufferSpan(0);
            // Assert.Empty(emptySpan);
        }

        private static void ValidateSize(TCollection collection, int size)
        {
            int g = collection.Settings.GrowthFactor;
            int c0 = collection.Settings.InitialCapacity;

            int ExpectedCapacity()
            {
                int expected = c0;
                while (expected < size)
                {
                    expected *= g;
                }
                return expected;
            }

            // TODO:
            //int ExpectedNumberOfBuffers()
            //{
            //    int expected = 1;
            //    for (int i = c0; i < size; i *= g)
            //    {
            //        expected++;
            //    }
            //    return expected;
            //}

            Assert.Equal(size, collection.Size);
            // TODO: Assert.Equal(ExpectedNumberOfBuffers(), growthArray.NumberOfBuffers);
            Assert.Equal(ExpectedCapacity(), collection.Capacity);
        }

        //private static IEnumerable<int> GetTestIndices(int listCount, bool exclusive)
        //{
        //    Debug.Assert(listCount >= 0);

        //    var indices = new[]
        //    {
        //        0,
        //        1,
        //        listCount / 4,
        //        listCount / 4 + 1,
        //        listCount / 2,
        //        listCount / 2 + 1,
        //        3 * listCount / 4,
        //        3 * listCount / 4 + 1,
        //        listCount - 1,
        //        listCount
        //    };

        //    return indices.Where(
        //        index => index >= 0 && (exclusive ? index < listCount : index <= listCount));
        //}
    }
}