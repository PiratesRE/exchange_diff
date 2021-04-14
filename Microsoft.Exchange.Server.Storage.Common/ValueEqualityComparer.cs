using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public sealed class ValueEqualityComparer : IEqualityComparer<object>
	{
		public ValueEqualityComparer(CompareInfo compareInfo)
		{
			this.compareInfo = compareInfo;
		}

		bool IEqualityComparer<object>.Equals(object x, object y)
		{
			return ValueHelper.ValuesEqual(x, y, this.compareInfo, CompareOptions.None);
		}

		int IEqualityComparer<object>.GetHashCode(object obj)
		{
			if (obj is string)
			{
				return obj.GetHashCode();
			}
			return ValueHelper.GetHashCode(obj);
		}

		private readonly CompareInfo compareInfo;
	}
}
