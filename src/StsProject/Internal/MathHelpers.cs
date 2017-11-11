using System.Diagnostics;

namespace StsProject.Internal
{
    internal static class MathHelpers
    {
        private const uint DeBruijn32 = 0x07C4ACDDU;

        private static readonly int[] DeBruijn32Table =
        {
            31, 22, 30, 21, 18, 10, 29,  2,
            20, 17, 15, 13,  9,  6, 28,  1,
            23, 19, 11,  3, 16, 14,  7, 24,
            12,  4,  8, 25,  5, 26, 27,  0
        };

        public static int CeilLog2(int value)
        {
            Debug.Assert(value > 0);

            return CeilLog2((uint)value);
        }

        public static int CeilLog2(uint value)
        {
            Debug.Assert(value > 0);

            uint powerOf2 = NextPowerOf2Inclusive(value);
            return Log2OfPowerOf2(powerOf2);
        }

        private static uint NextPowerOf2Inclusive(uint value)
        {
            Debug.Assert(value > 0);

            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        private static int Log2OfPowerOf2(uint value)
        {
            Debug.Assert(value > 0);
            Debug.Assert((value & (value - 1)) == 0);

            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return DeBruijn32Table[(DeBruijn32 * value) >> 27];
        }
    }
}