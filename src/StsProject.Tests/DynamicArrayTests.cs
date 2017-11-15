namespace StsProject.Tests
{
    public class DynamicArrayTests : AbstractArrayCollectionTests<DynamicArray<int>>
    {
        protected override DynamicArray<int> CreateCollection()
        {
            return new DynamicArray<int>();
        }
    }
}