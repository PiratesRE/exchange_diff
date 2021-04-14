using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct MaxValue
	{
		public MaxValue(bool inclusive, object value)
		{
			this.inclusive = inclusive;
			this.value = value;
		}

		public bool IsValid
		{
			get
			{
				return this.inclusive || this.value != null;
			}
		}

		public bool IsInclusive
		{
			get
			{
				return this.inclusive;
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
				return object.ReferenceEquals(this.value, MaxValue.Infinity.value);
			}
		}

		public static bool Equal(MaxValue value1, MaxValue value2, CompareInfo compareInfo)
		{
			return value1.IsInfinity == value2.IsInfinity && (value1.IsInfinity || (value1.IsInclusive == value2.IsInclusive && ValueHelper.ValuesEqual(value1.Value, value2.Value, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth)));
		}

		public static int Compare(MaxValue lhs, MaxValue rhs, Column column, CompareInfo compareInfo)
		{
			if (lhs.IsInfinity)
			{
				if (!rhs.IsInfinity)
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (rhs.IsInfinity)
				{
					return -1;
				}
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
					return -1;
				}
				else
				{
					if (!rhs.IsInclusive)
					{
						return 1;
					}
					return 0;
				}
			}
		}

		public static readonly MaxValue Infinity = new MaxValue(false, new object());

		private readonly bool inclusive;

		private readonly object value;
	}
}
