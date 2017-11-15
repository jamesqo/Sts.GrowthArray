namespace StsProject
{
    // Shared interface between DynamicArray/GrowthArray. For testing purposes only.
    public interface IArrayCollectionSettings
    {
        int GrowthFactor { get; }

        int InitialCapacity { get; }
    }
}