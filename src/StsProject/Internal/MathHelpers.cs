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

            // CREDIT: https://graphics.stanford.edu/~seander/bithacks.html, "Round up to the next highest power of 2".

            // What the middle 5 lines are doing are turning all of the bits after the most significant
            // set bit to 1.
            // e.g. If we have 00000000000010000000000000000000 after value--, it becomes
            //                 00000000000011000000000000000000
            //                 00000000000011110000000000000000
            //                 00000000000011111111000000000000
            //                 00000000000011111111111111110000
            //                 00000000000011111111111111111111

            // To get to the next power of 2, we essentially want the value with 1 bit set right before
            // the most significant set bit in 'value'.
            // e.g. If 'value' (before 'value--') was 00000000000010000000000000000001,
            // then we want                           00000000000100000000000000000000.
            // Once we have the value                 00000000000011111111111111111111,
            // we get to this by simply adding 1.

            // The value-- at the beginning prevents powers of 2 from being doubled, making them
            // result in themselves instead. It does not affect other numbers, since for non-powers
            // of 2 there is at least 1 set bit after the most significant set bit. Thus, subtracting
            // 1 will not change the position of the most significant set bit.

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

            // CREDIT: This idea is attributed to @mburbea at https://github.com/dotnet/corefx/issues/12425,
            // where I first learned of this idea. I modified some of code from a file (s)he posted to use here:
            // https://gist.github.com/mburbea/c9a71ac1b1a25762c38c9fee7de0ddc2#file-bits-cs-L120

            // The algorithm for this involves the concept of De Bruijn sequences.
            // See https://en.wikipedia.org/wiki/De_Bruijn_sequence for the concept.
            // When expressed as a 32-digit number in binary, 'DeBruijn32' is a string of 1s and 0s with the property that
            // every substring of length 5, including ones that wrap around the end of the number and include digits at the
            // beginning, is different.
            // See it for yourself: the binary representation of DeBruijn32 is 00000111110001001010110011011101, and all
            // 5-digit substrings (starting from the left and moving towards the right) are
            // 00000, 00001, 00011, 00111, 01111, 11111, 11110, 11100, 11000, 10001, 00010, 00100, 01001, 10010, 00101, 01010, 10101, 01011, 10110, 01100, 11001, 10011, 00110, 01101, 11011, 10111, 01110, 11101, 11010, 10100, 01000, 10000.

            // Now, I will discuss how this concept is applied. In this method, value = 2^i for some whole number 0 <= i < 32,
            // and the goal is to find i. First, consider that (DeBruijn32 * value) is equivalent to (DeBruijn32 << i).
            // Next, >> 27 means we are grabbing the 5 most significant digits (*), or the substring of length 5 at the beginning
            // of the binary representation. Since DeBruijn32 is a De Bruijn sequence, shifting by different amounts will result
            // in different substrings. In other words, f(i) = (DeBruijn32 << i) >> 27 is an injective function.

            // If f is an injective function, then it has an inverse! This is the purpose of DeBruijn32Table: it maps (DeBruijn32 << i) >> 27 back to i.

            // (*) It is important that an unsigned/logical right shift, as opposed to an signed/arithmetic right shift is done.
            //     In Java, the syntax for an unsigned right shift is >>> 27. There is no such operator in C#, however; >> does
            //     an unsigned right shift when the left operand is uint, and a signed right shift when the left operand is int.

            uint index = (DeBruijn32 * value) >> 27;
            return DeBruijn32Table[index];
        }
    }
}