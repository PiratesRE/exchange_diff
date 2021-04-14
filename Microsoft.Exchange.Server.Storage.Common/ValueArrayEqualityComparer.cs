using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Server.Storage.Common
{
	internal class ValueArrayEqualityComparer : IEqualityComparer<object[]>
	{
		bool IEqualityComparer<object[]>.Equals(object[] x, object[] y)
		{
			return ValueHelper.ArraysEqual(x, y, CultureInfo.InvariantCulture.CompareInfo, CompareOptions.None);
		}

		int IEqualityComparer<object[]>.GetHashCode(object[] x)
		{
			uint num = (uint)x.Length;
			for (int i = 0; i < x.Length; i++)
			{
				num ^= (uint)ValueHelper.GetHashCode(x[i]);
				num = (num << 1 | num >> 31);
			}
			return (int)num;
		}
	}
}
