using System;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal static class Macros
	{
		internal static int Offset(uint address)
		{
			return (int)(address & 1048575U);
		}

		internal static int BlockIndex(uint address)
		{
			return (int)((address & 4293918720U) >> 20);
		}

		internal static uint Address(int blockIndex, int offset)
		{
			return (uint)(blockIndex << 20 | offset);
		}

		internal static int Left(int i)
		{
			return 2 * i + 1;
		}

		internal static int Right(int i)
		{
			return 2 * (i + 1);
		}

		internal static int Parent(int i)
		{
			return (i - 1) / 2;
		}
	}
}
