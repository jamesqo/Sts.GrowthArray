using System;
using System.Collections.Generic;
using System.Linq;
using StsProject.Tests.TestInternal;

namespace StsProject.Tests
{
    public class TestEnumerables_Data : ClassData<IEnumerable<int>>
    {
        public override IEnumerable<IEnumerable<int>> GetData()
        {
            IEnumerable<int> CreateEnumerable(int size)
            {
                return size <= 0 ? null : Enumerable.Range(0, size);
            }

            var data = new List<IEnumerable<int>>();
            data.Add(new int[0]);

            for (int i = 0; i <= 7; i++)
            {
                int g = GrowthArray.GrowthFactor;
                int c0 = GrowthArray.InitialCapacity;
                // This is a term in the capacity sequence for sufficiently large n.
                int capacity = (int)Math.Pow(g, i) * c0;

                data.AddIfNotNull(CreateEnumerable(capacity - 7));
                data.AddIfNotNull(CreateEnumerable(capacity - 3));
                data.AddIfNotNull(CreateEnumerable(capacity - 1));
                data.AddIfNotNull(CreateEnumerable(capacity));
                data.AddIfNotNull(CreateEnumerable(capacity + 1));
                data.AddIfNotNull(CreateEnumerable(capacity + 3));
                data.AddIfNotNull(CreateEnumerable(capacity + 7));
            }

            return data;
        }
    }
}