using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct MinValue
	{
		public MinValue(bool inclusive, object value)
		{
			this.exclusive = !inclusive;
			this.value = value;
		}

		public bool IsInclusive
		{
			get
			{
				return !this.exclusive;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public bool IsInfinity
		{
			get
			{
				return !this.exclusive && this.value == null;
			}
		}

		public static bool Equal(MinValue value1, MinValue value2, CompareInfo compareInfo)
		{
			return value1.IsInclusive == value2.IsInclusive && ValueHelper.ValuesEqual(value1.Value, value2.Value, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
		}

		public static int Compare(MinValue lhs, MinValue rhs, Column column, CompareInfo compareInfo)
		{
			int num = ValueHelper.ValuesCompare(lhs.Value, rhs.Value, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
			if (num < 0)
			{
				return -1;
			}
			if (num != 0)
			{
				return 1;
			}
			if (!lhs.IsInclusive)
			{
				if (!rhs.IsInclusive)
				{
					return 0;
				}
				return 1;
			}
			else
			{
				if (!rhs.IsInclusive)
				{
					return -1;
				}
				return 0;
			}
		}

		public static readonly MinValue Infinity = default(MinValue);

		private readonly bool exclusive;

		private readonly object value;
	}
}
