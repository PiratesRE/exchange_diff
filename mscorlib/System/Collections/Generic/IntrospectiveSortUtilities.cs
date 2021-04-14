using System;
using System.Runtime.Versioning;

namespace System.Collections.Generic
{
	internal static class IntrospectiveSortUtilities
	{
		internal static int FloorLog2(int n)
		{
			int num = 0;
			while (n >= 1)
			{
				num++;
				n /= 2;
			}
			return num;
		}

		internal static void ThrowOrIgnoreBadComparer(object comparer)
		{
			if (BinaryCompatibility.TargetsAtLeast_Desktop_V4_5)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_BogusIComparer", new object[]
				{
					comparer
				}));
			}
		}

		internal const int IntrosortSizeThreshold = 16;

		internal const int QuickSortDepthThreshold = 32;
	}
}
