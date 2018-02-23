using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace StsProject.Internal
{
    internal static class MathHelpers
    {
        private const uint DeBruijn32 = 0x07C4ACDDU;

        // Generated via https://gist.github.com/jamesqo/71addc9c7db9ca4fa44da2240f7976bb
        private static readonly int[] DeBruijn32Table =
        {
             0,  1, 10,  2, 11, 14, 22,  3,
            30, 12, 15, 17, 19, 23, 26,  4,
            31,  9, 13, 21, 29, 16, 18, 25,
             8, 20, 28, 24,  7, 27,  6,  5,
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilLog2(uint value)
        {
            uint v = value - 1;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;

            uint index = (DeBruijn32 * v) >> 27;
            return DeBruijn32Table[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilLog2(int value) => CeilLog2((uint)value);
    }
}
