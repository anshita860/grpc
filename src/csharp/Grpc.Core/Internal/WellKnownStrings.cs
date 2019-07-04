using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Grpc.Core.Internal
{
    /// <summary>
    /// Utility type for identifying "well-known" strings (i.e. headers/keys etc that
    /// we expect to see frequently, and don't want to allocate lots of copies of)
    /// </summary>
    internal static class WellKnownStrings
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong Coerce64(byte* value)
        {
            return *(ulong*)value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint Coerce32(byte* value)
        {
            return *(uint*)value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ushort Coerce16(byte* value)
        {
            return *(ushort*)value;
        }

        /// <summary>
        /// Test whether the provided byte sequence is recognized as a well-known string; if
        /// so, return a shared instance of that string; otherwise, return null
        /// </summary>
        public static unsafe string TryIdentify(byte* source, int length)
        {
            // note: the logic here is hard-coded to constants for optimal processing;
            // refer to an ASCII/hex converter (and remember to reverse **segments** for little-endian)
            if (BitConverter.IsLittleEndian) // this is a JIT intrinsic; branch removal happens on modern runtimes
            {
                switch (length)
                {
                    case 0: return "";
                    case 10:
                        switch(Coerce64(source))
                        {
                            case 0x6567612d72657375: return Coerce16(source + 8) == 0x746e ? "user-agent" : null;
                        }
                        break;
                }
            }
            else
            {
                switch (length)
                {
                    case 0: return "";
                    case 10:
                        switch (Coerce64(source))
                        {
                            case 0x757365722d616765: return Coerce16(source + 8) == 0x6e74 ? "user-agent" : null;
                        }
                        break;
                }
            }
            return null;
        }
    }
}
