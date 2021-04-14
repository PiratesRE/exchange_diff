using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct ValueRange
	{
		public ValueRange(MinValue minValue, MaxValue maxValue, Column column, CompareInfo compareInfo)
		{
			this.MinValue = minValue;
			this.MaxValue = maxValue;
		}

		public bool IsEmpty
		{
			get
			{
				return !this.MaxValue.IsValid;
			}
		}

		public bool IsFull
		{
			get
			{
				return this.MinValue.IsInfinity && this.MaxValue.IsInfinity;
			}
		}

		public static bool Equal(ValueRange range1, ValueRange range2, CompareInfo compareInfo)
		{
			return (range1.IsEmpty && range2.IsEmpty) || (!range1.IsEmpty && !range2.IsEmpty && MinValue.Equal(range1.MinValue, range2.MinValue, compareInfo) && MaxValue.Equal(range1.MaxValue, range2.MaxValue, compareInfo));
		}

		public static ValueRange Intersect(ValueRange range1, ValueRange range2, Column column, CompareInfo compareInfo)
		{
			if (!ValueRange.AreOverlapping(range1, range2, column, compareInfo))
			{
				return ValueRange.Empty;
			}
			MinValue minValue = (MinValue.Compare(range1.MinValue, range2.MinValue, column, compareInfo) < 0) ? range2.MinValue : range1.MinValue;
			MaxValue maxValue = (MaxValue.Compare(range1.MaxValue, range2.MaxValue, column, compareInfo) > 0) ? range2.MaxValue : range1.MaxValue;
			return new ValueRange(minValue, maxValue, column, compareInfo);
		}

		public static bool AreOverlappingOrAdjacent(ValueRange range1, ValueRange range2, Column column, CompareInfo compareInfo)
		{
			return range1.IsEmpty || range2.IsEmpty || (ValueRange.CompareMinMax(range1.MinValue, range2.MaxValue, column, compareInfo, true) <= 0 && ValueRange.CompareMinMax(range2.MinValue, range1.MaxValue, column, compareInfo, true) <= 0);
		}

		public static ValueRange UnionOverlappingOrAdjacent(ValueRange range1, ValueRange range2, Column column, CompareInfo compareInfo)
		{
			if (range1.IsEmpty)
			{
				return range2;
			}
			if (range2.IsEmpty)
			{
				return range1;
			}
			MinValue minValue = (MinValue.Compare(range1.MinValue, range2.MinValue, column, compareInfo) < 0) ? range1.MinValue : range2.MinValue;
			MaxValue maxValue = (MaxValue.Compare(range1.MaxValue, range2.MaxValue, column, compareInfo) > 0) ? range1.MaxValue : range2.MaxValue;
			return new ValueRange(minValue, maxValue, column, compareInfo);
		}

		private static bool AreOverlapping(ValueRange range1, ValueRange range2, Column column, CompareInfo compareInfo)
		{
			return !range1.IsEmpty && !range2.IsEmpty && ValueRange.CompareMinMax(range1.MinValue, range2.MaxValue, column, compareInfo, false) < 0 && ValueRange.CompareMinMax(range2.MinValue, range1.MaxValue, column, compareInfo, false) < 0;
		}

		private static int CompareMinMax(MinValue min, MaxValue max, Column column, CompareInfo compareInfo, bool adjacentSameAsOverlapping)
		{
			if (min.IsInfinity || max.IsInfinity)
			{
				return -1;
			}
			int num = ValueHelper.ValuesCompare(min.Value, max.Value, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
			if (num < 0)
			{
				if (!adjacentSameAsOverlapping && !min.IsInclusive && !max.IsInclusive && ValueRange.AdjacentValues(min.Value, max.Value, column, compareInfo))
				{
					return 0;
				}
				return -1;
			}
			else if (num == 0)
			{
				if (!min.IsInclusive)
				{
					if (!max.IsInclusive)
					{
						return 1;
					}
					return 0;
				}
				else
				{
					if (!max.IsInclusive)
					{
						return 0;
					}
					return -1;
				}
			}
			else
			{
				if (adjacentSameAsOverlapping && min.IsInclusive && max.IsInclusive && ValueRange.AdjacentValues(max.Value, min.Value, column, compareInfo))
				{
					return 0;
				}
				return 1;
			}
		}

		private static bool AdjacentValues(object lhs, object rhs, Column column, CompareInfo compareInfo)
		{
			switch (column.ExtendedTypeCode)
			{
			case ExtendedTypeCode.Boolean:
				if (lhs != null)
				{
					return !(bool)lhs && (bool)rhs;
				}
				return !(bool)rhs;
			case ExtendedTypeCode.Int16:
				if (lhs != null)
				{
					return (short)lhs + 1 == (short)rhs;
				}
				return (short)rhs == short.MinValue;
			case ExtendedTypeCode.Int32:
				if (lhs != null)
				{
					return (int)lhs + 1 == (int)rhs;
				}
				return (int)rhs == int.MinValue;
			case ExtendedTypeCode.Int64:
				if (lhs != null)
				{
					return (long)lhs + 1L == (long)rhs;
				}
				return (long)rhs == long.MinValue;
			case ExtendedTypeCode.Single:
				return lhs == null && (float)rhs == float.MinValue;
			case ExtendedTypeCode.Double:
				return lhs == null && (double)rhs == double.MinValue;
			case ExtendedTypeCode.DateTime:
				if (lhs != null)
				{
					return ((DateTime)lhs).Ticks + 1L == ((DateTime)rhs).Ticks;
				}
				return (DateTime)rhs == DateTime.MinValue;
			case ExtendedTypeCode.Guid:
				return lhs == null && (Guid)rhs == Guid.Empty;
			case ExtendedTypeCode.String:
				return lhs == null && ((string)rhs).Length == 0;
			case ExtendedTypeCode.Binary:
				return lhs == null && ((Array)rhs).Length == 0;
			case ExtendedTypeCode.MVInt16:
			case ExtendedTypeCode.MVInt32:
			case ExtendedTypeCode.MVInt64:
			case ExtendedTypeCode.MVSingle:
			case ExtendedTypeCode.MVDouble:
			case ExtendedTypeCode.MVDateTime:
			case ExtendedTypeCode.MVGuid:
			case ExtendedTypeCode.MVString:
			case ExtendedTypeCode.MVBinary:
				return lhs == null && ((Array)rhs).Length == 0;
			}
			return false;
		}

		public static readonly ValueRange Empty = default(ValueRange);

		public static readonly ValueRange Full = new ValueRange(MinValue.Infinity, MaxValue.Infinity, null, null);

		public readonly MinValue MinValue;

		public readonly MaxValue MaxValue;
	}
}
