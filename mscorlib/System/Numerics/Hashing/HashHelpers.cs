using System;

namespace System.Numerics.Hashing
{
	internal static class HashHelpers
	{
		public static int Combine(int h1, int h2)
		{
			uint num = (uint)(h1 << 5 | (int)((uint)h1 >> 27));
			return (int)(num + (uint)h1 ^ (uint)h2);
		}
	}
}
