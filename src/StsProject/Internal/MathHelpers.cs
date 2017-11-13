using System.Diagnostics;

namespace StsProject.Internal
{
    internal static class MathHelpers
    {
        private const uint DeBruijn32 = 0x07C4ACDDU;

        // Generated via https://gist.github.com/jamesqo/71addc9c7db9ca4fa44da2240f7976bb
        private static readonly int[] DeBruijn32Table =
        {
            0, 1, 10, 2, 11, 14, 22, 3, 30, 12, 15, 17, 19, 23, 26, 4, 31, 9, 13, 21, 29, 16, 18, 25, 8, 20, 28, 24, 7, 27, 6, 5
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
            
            return DeBruijn32Table[(DeBruijn32 * value) >> 27];
        }
    }
}