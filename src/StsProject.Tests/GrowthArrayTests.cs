using System;

namespace StsProject.Tests
{
    public class GrowthArrayTests : AbstractArrayCollectionTests<GrowthArray<int>>
    {
        protected override GrowthArray<int> CreateCollection()
        {
            return new GrowthArray<int>();
        }
    }
}