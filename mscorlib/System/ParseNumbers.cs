using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System
{
	internal static class ParseNumbers
	{
		[SecuritySafeCritical]
		public static long StringToLong(string s, int radix, int flags)
		{
			return ParseNumbers.StringToLong(s, radix, flags, null);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern long StringToLong(string s, int radix, int flags, int* currPos);

		[SecuritySafeCritical]
		public unsafe static long StringToLong(string s, int radix, int flags, ref int currPos)
		{
			fixed (int* ptr = &currPos)
			{
				return ParseNumbers.StringToLong(s, radix, flags, ptr);
			}
		}

		[SecuritySafeCritical]
		public static int StringToInt(string s, int radix, int flags)
		{
			return ParseNumbers.StringToInt(s, radix, flags, null);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern int StringToInt(string s, int radix, int flags, int* currPos);

		[SecuritySafeCritical]
		public unsafe static int StringToInt(string s, int radix, int flags, ref int currPos)
		{
			fixed (int* ptr = &currPos)
			{
				return ParseNumbers.StringToInt(s, radix, flags, ptr);
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string IntToString(int l, int radix, int width, char paddingChar, int flags);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string LongToString(long l, int radix, int width, char paddingChar, int flags);

		internal const int PrintAsI1 = 64;

		internal const int PrintAsI2 = 128;

		internal const int PrintAsI4 = 256;

		internal const int TreatAsUnsigned = 512;

		internal const int TreatAsI1 = 1024;

		internal const int TreatAsI2 = 2048;

		internal const int IsTight = 4096;

		internal const int NoSpace = 8192;
	}
}
