namespace StsProject.Internal
{
    internal static class Strings
    {
        public const string First_EmptyCollection =
            "Cannot get first item of an empty collection.";

        public const string Last_EmptyCollection =
            "Cannot get last item of an empty collection.";

        public const string MoveToBlock_NotContiguous =
            "Cannot move a non-contiguous block list.";

        public static string Remove_CursorAtEnd { get; } =
            "Cannot remove an item when the cursor is at the end of the block list.";
    }
}